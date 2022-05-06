using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace Rate.Core.Model.CoindeskPlatform
{
    public class CountryExchangeRate : ICountryExchangeRate
    {
        private static ILogger<CountryExchangeRate> Log;
        private readonly RateEndpoints RateEndpoints;
        private IDictionary<string, CountryRateFormat> LastRate;
        public CountryExchangeRate(ILogger<CountryExchangeRate> log, RateEndpoints rateEndpoints)
        {
            Log = log;
            RateEndpoints = rateEndpoints;
        }
        public IDictionary<string, CountryRateFormat> GetExchangeRate()
        {
            IDictionary<string, CountryRateFormat> exchangeRate = null;
            if (string.IsNullOrEmpty(RateEndpoints.Coindesk.ExchangeRate))
            {
                Log.LogError("Missing exchange rate endpoint");
                return null;
            }
            try
            {
                var client = new RestClient(RateEndpoints.Coindesk.ExchangeRate);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode.Equals(HttpStatusCode.OK))
                {
                    var dto = JsonConvert.DeserializeObject<CountryExchangeRateDto>(response.Content);
                    exchangeRate = dto.Data;
                    LastRate = dto.Data;
                }
                else
                {
                    Log.LogError(response.Content);
                    Log.LogError("Fail to retrieve latest exchange rate. Therefore, get previous exchange rate.");
                    return LastRate;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }
            return exchangeRate;
        }
    }

    public class CountryExchangeRateDto : CoindeskHttpBase
    {
        public IDictionary<string, CountryRateFormat> Data { get; set; }
    }
    public class CountryRateFormat : CoindeskRateBase
    {
        public decimal Rate { get; set; }
    }
}
