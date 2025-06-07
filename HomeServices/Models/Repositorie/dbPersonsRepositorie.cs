
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models.Repositorie
{
    public class dbPersonsRepositorie : IRepositorie<Persons,int>
    {
        public AppDBContext db { get; }
        public dbPersonsRepositorie(AppDBContext _db)
        {
            db = _db;
        }
        public void Add(Persons entity)
        {
            db.Persons.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id, Persons entity)
        {
            var data = db.Persons.Find(id); 
            db.Persons.Remove(data);
            db.SaveChanges();   
        }

        public Persons Find(int id)
        {
            return db.Persons.SingleOrDefault(x => x.PersonsId == id);
        }

        public void Update(int id, Persons entity)
        {
            db.Persons.Update(entity);
            db.SaveChanges();
        }

        public IList<Persons> View()
        {
            return db.Persons.Include(x => x.User).ToList();
        }
    }
}
