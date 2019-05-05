using Calico.Persistencia;
using System;
using System.Data.Entity;

namespace Calico
{
    public interface Dao<T> where T : class
    {
        DbSet<T> findAll();
        void save(T obj);
        void delete(int id);
        void update(T obj);
        T findById(int id);

    }
}
