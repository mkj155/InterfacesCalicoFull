using System.Data.Entity;

namespace Calico.DAOs
{
    public interface Dao<T> where T : class
    {
        DbSet<T> FindAll();
        void Save(T obj);
        void Delete(int id);
        void Update(T obj);
        T FindById(int id);

    }
}
