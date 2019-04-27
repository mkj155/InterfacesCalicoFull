using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico
{
    interface Dao<T> where T : class
    {
        DbSet<T> findAll();
        void save(T obj);
        void delete(int id);
        void update(T obj);
        T findById(int id);

    }
}
