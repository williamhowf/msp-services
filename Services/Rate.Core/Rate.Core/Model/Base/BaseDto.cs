using System;

namespace Rate.Core.Model.Base
{
    public class BaseDto
    {
        public decimal CurrencyToBtcRate(decimal BtcToCcyRate) => Math.Round(1m / BtcToCcyRate, 8);
        public decimal CurrencyToMbtcRate(decimal BtcToCcyRate) => Math.Round(1000m / BtcToCcyRate, 8);
        public decimal MbtcToCurrencyRate(decimal BtcToCcyRate) => Math.Round(BtcToCcyRate / 1000m, 8);
    }
}
