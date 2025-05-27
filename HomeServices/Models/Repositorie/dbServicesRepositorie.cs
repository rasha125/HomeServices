
namespace HomeServices.Models.Repositorie
{
    public class dbServicesRepositorie : IRepositorie<Services>
    {
        public AppDBContext db { get; }
        public dbServicesRepositorie(AppDBContext _db)
        {
            db = _db;
        }

        public void Add(Services entity)
        {
            db.Services.Add(entity);
            db.SaveChanges();   
        }

        public void Delete(int id, Services entity)
        {
            var data = db.Services.Find(id);
            db.Services.Remove(data);
            db.SaveChanges();
        }

        public Services Find(int id)
        {
            return db.Services.SingleOrDefault(x => x.ServicesId == id);
        }

        public void Update(int id, Services entity)
        {
            db.Services.Update(entity);
            db.SaveChanges();
        }

        public IList<Services> view()
        {
            return db.Services.ToList();
        }
    }
}
