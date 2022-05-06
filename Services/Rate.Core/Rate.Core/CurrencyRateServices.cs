using Com.GGIT.Common;
using Com.GGIT.Common.Util;
using Com.GGIT.Database;
using Com.GGIT.Database.Domain;
using Com.GGIT.Database.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rate.Core.Model;
using Rate.Core.Model.Base;
using Rate.Core.Model.CoindeskPlatform;
using Rate.Core.Model.CryptoComparePlatform;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Rate.Core
{
    public class CurrencyRateServices : ICurrencyRateServices, IDisposable
    {
        private static ILogger<CurrencyRateServices> Log;
        private readonly RateEndpoints RateEndpoints;
        private readonly ISessionDB SessionDB;
        private readonly ICountryExchangeRate CountryExchangeRate;
        private TimeStampUtil _commUtil;
        public CurrencyRateServices(
            ILogger<CurrencyRateServices> logger
            , ISessionDB sessionDB
            , RateEndpoints rateEndpoints
            , ICountryExchangeRate countryExchangeRate)
        {
            Log = logger;
            SessionDB = sessionDB;
            RateEndpoints = rateEndpoints;
            CountryExchangeRate = countryExchangeRate;
            _commUtil = new TimeStampUtil();
        }

        public void RetrieveRate(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                IList<MSP_Currency> currencies = GetActiveCurrencies();
                bool CryptoCompareExecuted = false;
                string Executing = string.Empty;
                if (currencies.Count == 0)
                {
                    Log.LogWarning("No active currency to pull btc rate.");
                    return;
                }
                try
                {
                    if (RateEndpoints.Coindesk.Enable)
                    {
                        Executing = RateEndpoints.Coindesk.Name;
                        GetCoindeskRate(RateEndpoints.Coindesk, currencies);
                        Log.LogInformation("Get " + RateEndpoints.Coindesk.Name + " rate success");
                    }
                    else if (RateEndpoints.CryptoCompare.Enable)
                    {
                        CryptoCompareExecuted = true;
                        Executing = RateEndpoints.CryptoCompare.Name;
                        GetCryptoCompareRate(RateEndpoints.CryptoCompare, currencies);
                        Log.LogInformation("Get " + RateEndpoints.CryptoCompare.Name + " rate success");
                    }
                }
                catch (Exception)
                {
                    // contingency plan
                    Log.LogInformation("Failed to get " + Executing + " rate...");
                    if (RateEndpoints.CryptoCompare.Enable && !CryptoCompareExecuted)
                    {
                        try
                        {
                            GetCryptoCompareRate(RateEndpoints.CryptoCompare, currencies);
                            Log.LogInformation("Get " + RateEndpoints.CryptoCompare.Name + " rate success");
                        }
                        catch (Exception z)
                        {
                            Log.LogError(z.Message);
                            Log.LogInformation("Failed to get " + RateEndpoints.CryptoCompare.Name + " rate...");
                        }
                    }
                }
                finally
                {
                    _commUtil.InsertLastActivityLogTimestamp("Worker-BtcCurrencyRate"); //voonkeong 20201124 MDT-1756
                }
            }
        }

        private void GetCoindeskRate(Coindesk api, IList<MSP_Currency> currencies)
        {
            if (api.Debug) throw new Exception("Debug flag TRUE."); // when true, it shall move to next Exchange Rate API

            try
            {
                bool success = true;
                string[] patterns = api.Pattern.Split("|");
                string dateStartFormat = patterns[0];
                string dateEndFormat = patterns[1];
                var date = new StartEndDatetime();
                var exchangeRate = CountryExchangeRate.GetExchangeRate();

                if (exchangeRate.IsNull()) success = false;
                else
                {
                    var client = new RestClient(api.Endpoint.Replace(dateStartFormat, date.StartDateTimeToString).Replace(dateEndFormat, date.EndDateTimeToString));
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);
                    if (response.StatusCode.Equals(HttpStatusCode.OK))
                    {
                        //Log.LogInformation(response.Content);
                        var result = JsonConvert.DeserializeObject<CoindeskResponse>(response.Content);
                        IList<CoindeskRateDatetime> Entries = new List<CoindeskRateDatetime>();
                        for (int i = 0; i < result.Data.Entries.Count; i++) // CoinDesk may return 2 BTC rates of start time and end time.
                        {
                            Entries.Add(new CoindeskRateDatetime((long)result.Data.Entries[i][0], (decimal)result.Data.Entries[i][1]));
                        }
                        var coinRate = Entries.Last();
                        foreach (var currency in currencies)
                        {
                            var rate = InsertCurrencyRate(api.Name, coinRate, currency.Code, exchangeRate);
                            InsertCurrencyRateProfile(rate, currency);
                        }
                    }
                    else
                    {
                        Log.LogWarning(api.Name + " response = " + response.StatusCode + ". " + response.Content);
                        success = false;
                    }
                }

                if (!success) throw new Exception("Unable to request Coindesk rate due to some reason.");
            }
            catch (Exception e)
            {
                Log.LogInformation(e.Message);
                throw;
            }
        }

        private void GetCryptoCompareRate(CryptoCompare api, IList<MSP_Currency> currencies)
        {
            string[] patterns = api.Pattern.Split("|");
            string currenciesFormat = patterns[0];
            string apikeyFormat = patterns[1];

            string ccy = string.Empty;
            foreach (var c in currencies) ccy += c.Code + ",";
            ccy = ccy.Substring(0, ccy.Length - 1);

            var client = new RestClient(api.Endpoint.Replace(currenciesFormat, ccy).Replace(apikeyFormat, api.ApiKey));
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                var result = JsonConvert.DeserializeObject<CryptoCompareResponse>(response.Content).Raw.Btc;
                foreach (var currency in currencies)
                {
                    if (!result.ContainsKey(currency.Code))
                    {
                        Log.LogWarning("Currency " + currency.Code + "[" + currency.Description + "]" + " not supported by " + api.Name);
                        continue;
                    }
                    var rate = InsertCurrencyRate(api.Name, result[currency.Code], currency.Code);
                    InsertCurrencyRateProfile(rate, currency);
                }
            }
            else
            {
                Log.LogWarning(api.Name + " response = " + response.StatusCode.ToString() + ". " + response.Content);
            }
        }

        private IList<MSP_Currency> GetActiveCurrencies()
        {
            IList<MSP_Currency> currencies = new List<MSP_Currency>();
            try
            {
                using var db = SessionDB.OpenSession();
                currencies = db.Query<MSP_Currency>().Where(c => c.Status == "A").ToList();
            }
            catch (Exception)
            {
                // Do nothing
            }

            return currencies;
        }

        private MSP_CurrencyRate InsertCurrencyRate<T>(string platform, T value, string currency, IDictionary<string, CountryRateFormat> ExRate = null)
        {
            MSP_CurrencyRate data = null;
            try
            {
                Type t = value.GetType();
                if (t.Equals(typeof(CoindeskRateDatetime)))
                {
                    var coinRate = value as CoindeskRateDatetime;
                    var exchangeRate = ExRate[currency] as CountryRateFormat;
                    var amount = coinRate.Rate * exchangeRate.Rate;
                    BtcDto rate = new BtcDto(platform, currency, amount);
                    data = MSP_CurrencyRateFactory(rate, coinRate.RateOnUtc); 
                    using var db = SessionDB.OpenSession();
                    db.SaveTransaction(data);
                }
                else if (t.Equals(typeof(CcyType)))
                {
                    var coinRate = value as CcyType;
                    var amount = coinRate.Price;
                    string source = platform + "-" + coinRate.LastMarket;
                    BtcDto rate = new BtcDto(source, currency, amount);
                    data = MSP_CurrencyRateFactory(rate, coinRate.RateOnUtc); 
                    using var db = SessionDB.OpenSession();
                    db.SaveTransaction(data);
                }
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
                throw;
            }
            return data;
        }

        private void InsertCurrencyRateProfile(MSP_CurrencyRate value, MSP_Currency currency)
        {
            try
            {
                using var db = SessionDB.OpenSession();
                var record = db.Query<MSP_CurrencyRateProfile>().Where(c => c.Code == currency.Code).FirstOrDefault();
                if (record.IsNotNull())
                {
                    record.CurrencyRateID = value.Id;
                    record.BtcToCcyRate = value.BtcToCcyRate;
                    record.MbtcToCcyRate = value.MbtcToCcyRate;
                    record.CcyToMbtcRate = value.CcyToMbtcRate;
                    record.Source = value.Source;
                    record.RateOnUtc = value.RateOnUtc;
                    record.UpdatedOnUtc = DateTime.UtcNow;
                    db.UpdateTransaction(record);
                }
                else
                {
                    var data = new MSP_CurrencyRateProfile
                    {
                        Code = value.Code,
                        Description = currency.Description,
                        CurrencyRateID = value.Id,
                        BtcToCcyRate = value.BtcToCcyRate,
                        MbtcToCcyRate = value.MbtcToCcyRate,
                        CcyToMbtcRate = value.CcyToMbtcRate,
                        Source = value.Source,
                        RateOnUtc = value.RateOnUtc,
                        UpdatedOnUtc = DateTime.UtcNow
                    };
                    db.SaveTransaction(data);
                }
            }
            catch (Exception e)
            {
                Log.LogError(e.Message);
                throw;
            }
        }

        public void Dispose()
        {
            Log.LogInformation("Disposing...");
            SessionDB.CloseSessions();
        }

        private MSP_CurrencyRate MSP_CurrencyRateFactory(BtcDto rate, DateTime RateOnUtc)
        {
            return new MSP_CurrencyRate
            {
                Code = rate.CcyCode,
                BtcToCcyRate = rate.BtcToCcyRate,
                MbtcToCcyRate = rate.MbtcToCcyRate,
                CcyToMbtcRate = rate.CcyToMbtcRate,
                Source = rate.Name,
                RateOnUtc = RateOnUtc,
                CreatedOnUtc = DateTime.UtcNow
            };
        }
    }
}
