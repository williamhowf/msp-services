namespace Rate.Core.Model.Base
{
    public class BtcDto : BaseDto
    {
        /// <summary>
        /// Btc Rate
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currency"></param>
        /// <param name="amount"></param>
        public BtcDto(string name, string currency, decimal amount)
        {
            Name = name;
            CcyCode = currency;
            BtcToCcyRate = amount;
        }
        /// <summary>
        /// Platform Name
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// Currency Code
        /// </summary>
        public string CcyCode { get; }
        /// <summary>
        /// e.g. : 1 USD  = 0.00018645 BTC
        /// </summary>
        public decimal CcyToBtcRate => CurrencyToBtcRate(BtcToCcyRate);
        /// <summary>
        /// e.g. : 1 USD = 0.18644738 mBTC
        /// </summary>
        public decimal CcyToMbtcRate => CurrencyToMbtcRate(BtcToCcyRate);
        /// <summary>
        /// e.g. : 1 BTC = 5363.44330000 USD
        /// </summary>
        public decimal BtcToCcyRate { get; }
        /// <summary>
        /// e.g. : 1 mBTC = 5.3634433 USD
        /// </summary>
        public decimal MbtcToCcyRate => MbtcToCurrencyRate(BtcToCcyRate);
    }
}
