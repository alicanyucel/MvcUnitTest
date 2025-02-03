namespace WebApplication3.Repositories
{
    public  interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int id);
        Task Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task CreateAsync();
    }
}
