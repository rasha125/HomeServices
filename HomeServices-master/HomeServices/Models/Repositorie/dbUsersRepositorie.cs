
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models.Repositorie
{
    public class dbUsersRepositorie : IRepositorie<Users, string>
    {
        public AppDBContext db { get; }
        public dbUsersRepositorie(AppDBContext _db)
        {
            db = _db;
        }
        public void Add(Users entity)
        {
            db.Users.Add(entity);
            db.SaveChanges();
        }

        public void Delete(String id, Users entity)
        {
           var data=db.Users.Find(id);
            db.Users.Remove(data);
            db.SaveChanges();

        }

        public Users Find(String id)
        {
            return db.Users.SingleOrDefault(x=>x.Id==id);
        }

        public void Update(String id, Users entity)
        {
            db.Users.Update(entity);
            db.SaveChanges();
        }

        public IList<Users> View()
        {
            return db.Users.ToList();
        }
    }
}
