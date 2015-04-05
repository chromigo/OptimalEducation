using System.Linq;
using System.Threading.Tasks;
using OptimalEducation.Models;

namespace OptimalEducation.Helpers
{
    public class InfoExtractor : IInfoExtractor
    {
        private readonly IApplicationUserManager _userManager;

        public InfoExtractor(IApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<int> ExtractEntrantId(string userId)
        {
            var currentUser = await _userManager.FindByIdAsync(userId);
            var entrantClaim = currentUser.Claims.FirstOrDefault(p => p.ClaimType == MyClaimTypes.EntityUserId);
            var entrantId = int.Parse(entrantClaim.ClaimValue);
            return entrantId;
        }
    }
}