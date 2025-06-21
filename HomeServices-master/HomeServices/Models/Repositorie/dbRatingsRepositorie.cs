
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models.Repositorie
{
    public class dbRatingsRepositorie : IRepositorie<Ratings , int>
    {
        public AppDBContext db { get; }
        public dbRatingsRepositorie(AppDBContext _db)
        {
            db = _db;
        }
        public void Add(Ratings entity)
        {
            db.Ratings.Add(entity);
            db.SaveChanges();
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

        public IList<Ratings> View()
        {
          return  db.Ratings.Include(x => x.Orders).ToList();
        }
    }
}
