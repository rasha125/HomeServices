
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models.Repositorie
{
    public class dbUsersRepositorie : IRepositorie<Users>
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

        public void Delete(int id, Users entity)
        {
           var data=db.Users.Find(id);
            db.Users.Remove(data);
            db.SaveChanges();

        }

        public Users Find(int id)
        {
            return db.Users.SingleOrDefault(x=>x.UsersId==id);
        }

        public void Update(int id, Users entity)
        {
            db.Users.Update(entity);
            db.SaveChanges();
        }

        public IList<Users> view()
        {
            return db.Users.ToList();
        }
    }
}
