using System.Threading.Tasks;

namespace PetShop.Services
{
    public interface IGeminiAnalysisService
    {
        Task<string> GetDietAnalysisAsync(string prompt);
    }
}