namespace HomeServices.Models.Repositorie
{
    public interface IRepositorie<TEntity>
    {
        IList<TEntity> View();
        void Add(TEntity entity);
        void Update(int id, TEntity entity);
        void Delete(int id, TEntity entity);
        TEntity Find(int id);

    }
}
