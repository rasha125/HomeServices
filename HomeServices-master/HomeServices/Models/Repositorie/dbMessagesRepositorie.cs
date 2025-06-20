
namespace HomeServices.Models.Repositorie
{
    public class dbMessagesRepositorie : IRepositorie<Messages, int>
    {
        public AppDBContext db { get; }
        public dbMessagesRepositorie(AppDBContext _db)
        {
            db = _db;
        }

        public IList<Messages> View()
        {
            throw new NotImplementedException();
        }

        public void Add(Messages entity)
        {
            throw new NotImplementedException();
        }

        public void Update(int id, Messages entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id, Messages entity)
        {
            throw new NotImplementedException();
        }

        public Messages Find(int id)
        {
            throw new NotImplementedException();
        }
    }
}