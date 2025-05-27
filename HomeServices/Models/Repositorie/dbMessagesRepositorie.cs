
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models.Repositorie
{
    public class dbMessagesRepositorie : IRepositorie<Messages>
    {
        public AppDBContext db { get; }
        public dbMessagesRepositorie(AppDBContext _db)
        {
            db = _db;
        }
        public void Add(Messages entity)
        {
            db.Messages.Add(entity);
            db.SaveChanges();   
        }

        public void Delete(int id, Messages entity)
        {
            var data = db.Messages.Find(id);
            db.Messages.Remove(data);
            db.SaveChanges();
        }

        public Messages Find(int id)
        {
            return db.Messages.SingleOrDefault(x => x.MessagesId == id);
        }

        public void Update(int id, Messages entity)
        {
            db.Messages.Update(entity);
            db.SaveChanges();
        }

        public IList<Messages> view()
        {
            return db.Messages.Include(x=>x.Persons).Include(x => x.Providers).ToList();
        }
    }
}
