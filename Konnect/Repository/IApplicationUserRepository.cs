using Konnect.Models;

namespace Konnect.Repository
{
    public interface IApplicationUserRepository:IRepository<ApplicationUser>
    {
        void Update(ApplicationUser user);
    }
}
