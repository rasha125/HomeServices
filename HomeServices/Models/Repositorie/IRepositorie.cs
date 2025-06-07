namespace HomeServices.Models.Repositorie
{
    public interface IRepositorie<TEntity, TKey>
    {

        //The TKey type parameter is used to support different types of primary keys across entities.
        //For example, the Users entity uses a string ID (from IdentityUser),
        //while other entities like Orders or Services use an integer ID.
        //This design makes the interface flexible and reusable for all entity types.

        IList<TEntity> View();
        void Add(TEntity entity);
        void Update(TKey id, TEntity entity);
        void Delete(TKey id, TEntity entity);
        TEntity Find(TKey id);
    }
}
