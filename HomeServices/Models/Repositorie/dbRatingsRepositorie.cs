
namespace HomeServices.Models.Repositorie
{
    public class dbRatingsRepositorie : IRepositorie<Ratings>
    {
        public AppDBContext db { get; }
        public dbRatingsRepositorie(AppDBContext _db)
        {
            db = _db;
        }
        public void Add(Ratings entity)
        {
            db.Ratings.Add(entity);
        }

        public void Delete(int id, Ratings entity)
        {
            var data = db.Ratings.Find(id);
            db.Ratings.Remove(data);
            db.SaveChanges();
        }

        public Ratings Find(int id)
        {
            return db.Ratings.SingleOrDefault(x => x.RatingsId == id);
        }

        public void Update(int id, Ratings entity)
        {
            db.Ratings.Update(entity);
            db.SaveChanges();
        }

        public IList<Ratings> view()
        {
            return db.Ratings.ToList();
        }
    }
}
