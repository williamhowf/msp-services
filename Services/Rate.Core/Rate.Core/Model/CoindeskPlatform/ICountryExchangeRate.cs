using System.Collections.Generic;

namespace Rate.Core.Model.CoindeskPlatform
{
    public interface ICountryExchangeRate
    {
        IDictionary<string, CountryRateFormat> GetExchangeRate();
    }
}
