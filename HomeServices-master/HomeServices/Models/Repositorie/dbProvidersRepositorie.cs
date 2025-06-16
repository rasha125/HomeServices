
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models.Repositorie
{
    public class dbProvidersRepositorie : IRepositorie<Providers , int>
    {
        public AppDBContext db { get; }
        public dbProvidersRepositorie(AppDBContext _db)
        {
            db = _db;
        }
        public void Add(Providers entity)
        {
            db.Providers.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id, Providers entity)
        {
            var data= db.Providers.Find(id);
            db.Providers.Remove(data);
            db.SaveChanges();
        }

        public Providers Find(int id)
        {
            return db.Providers.SingleOrDefault(x => x.ProvidersId == id);
        }

        public void Update(int id, Providers entity)
        {
            db.Providers.Update(entity);
            db.SaveChanges();
        }

        public IList<Providers> View()
        {
            return db.Providers.Include(x=>x.User).Include(x=>x.Services).ToList();
        }
    }
}
