
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models.Repositorie
{
    public class dbIssuesRepositorie : IRepositorie<Issues>
    {
        public AppDBContext db { get; }
        public dbIssuesRepositorie(AppDBContext _db)
        {
            db = _db;
        }
        public void Add(Issues entity)
        {
            db.Issues.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id, Issues entity)
        {
            var data = db.Issues.Find(id);
            db.Issues.Remove(data);
            db.SaveChanges();
        }

        public Issues Find(int id)
        {
            return db.Issues.SingleOrDefault(x => x.IssuesId == id);
        }

        public void Update(int id, Issues entity)
        {
            db.Issues.Update(entity);
            db.SaveChanges();   
        }

        public IList<Issues> view()
        {
            return db.Issues.Include(x=>x.Users).ToList();
        }
    }
}
