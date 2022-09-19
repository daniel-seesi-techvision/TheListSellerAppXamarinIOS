using System.Threading.Tasks;

namespace TheListSellerAppXamariniOS.Services
{
    public interface IRequestProvider
    {
        Task<TResult> GetAsync<TResult>(HttpRequestModel model);
    }
}