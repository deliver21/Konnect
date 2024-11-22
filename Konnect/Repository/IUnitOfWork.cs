namespace Konnect.Repository
{
    public interface IUnitOfWork
    {
        public IApplicationUserRepository ApplicationUser { get; }
        void Save();
    }
}
