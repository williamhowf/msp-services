using System.Threading;
using System.Threading.Tasks;

namespace Rate.Core
{
    public interface ICurrencyRateServices
    {
        public void RetrieveRate(CancellationToken stoppingToken);
    }
}
