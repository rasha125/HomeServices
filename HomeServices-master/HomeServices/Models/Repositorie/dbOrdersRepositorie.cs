
using Microsoft.EntityFrameworkCore;

namespace HomeServices.Models.Repositorie
{
    public class dbOrdersRepositorie : IRepositorie<Orders , int>
    {
        public AppDBContext db { get; }
        public dbOrdersRepositorie(AppDBContext _db)
        {
            db = _db;
        }
        public void Add(Orders entity)
        {
            db.Orders.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id, Orders entity)
        {
            var data = db.Orders.Find(id);
            db.Orders.Remove(data);
            db.SaveChanges();
        }

        public Orders Find(int id)
        {
            return db.Orders.SingleOrDefault(x => x.OrdersId == id);
        }

        public void Update(int id, Orders entity)
        {
            db.Orders.Update(entity);
            db.SaveChanges();
        }

        public IList<Orders> View()
        {
            return db.Orders.Include(x=>x.Persons).Include(x=>x.Providers).ToList();
        }
    }
}
