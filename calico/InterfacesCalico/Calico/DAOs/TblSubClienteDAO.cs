using Calico.persistencia;
using System.Data.Entity;

namespace Calico.DAOs
{
    class TblSubClienteDAO : Dao<tblSubCliente>
    {
        public void Delete(int id)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                tblSubCliente obj = new tblSubCliente { subc_proc_id = id };
                context.tblSubCliente.Attach(obj);
                context.tblSubCliente.Remove(obj);
                context.SaveChanges();
            }
        }

        public DbSet<tblSubCliente> FindAll()
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                /* Obtengo todos los registros de la tabla de esta manera */
                var rows = context.Set<tblSubCliente>();
                return rows;
            }
        }

        public tblSubCliente FindById(int id)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                return context.tblSubCliente.Find(id);
            }
        }

        public void Save(tblSubCliente obj)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                context.tblSubCliente.Add(obj);
                context.SaveChanges();
            }
        }

        public void Update(tblSubCliente obj)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                var result = context.tblSubCliente.Find(obj.subc_proc_id);
                if (result == null) return;
                context.Entry(result).CurrentValues.SetValues(obj);
                context.SaveChanges();
            }
        }

        public int CallProcedure(int? tipoProceso, int? tipoMensaje)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                return context.INTERFAZ_CrearProceso(tipoProceso, tipoMensaje);
            }
        }

    }
}
