using Calico.Persistencia;
using System;
using System.Data.Entity;

namespace Calico.clientes
{
    class tblSubClienteDAO : Dao<tblSubCliente>
    {
        public void delete(int id)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                tblSubCliente obj = new tblSubCliente { subc_proc_id = id };
                context.tblSubCliente.Attach(obj);
                context.tblSubCliente.Remove(obj);
                context.SaveChanges();
            }
        }

        public DbSet<tblSubCliente> findAll()
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                /* Obtengo todos los registros de la tabla de esta manera */
                var rows = context.Set<tblSubCliente>();
                return rows;
            }
        }

        public tblSubCliente findById(int id)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                return context.tblSubCliente.Find(id);
            }
        }

        public void save(tblSubCliente obj)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                context.tblSubCliente.Add(obj);
                context.SaveChanges();
            }
        }

        public void update(tblSubCliente obj)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                var result = context.tblSubCliente.Find(obj.subc_proc_id);
                if (result == null) return;
                context.Entry(result).CurrentValues.SetValues(obj);
                context.SaveChanges();
            }
        }

        public void callProcedure(int? tipoProceso, int? tipoMensaje)
        {
            using (CalicoEntities context = new CalicoEntities())
            {
                //context.INTERFAZ_TESTPROCEDURE(nombre);
                context.INTERFAZ_CrearProceso(tipoProceso, tipoMensaje);
            }
        }

    }
}
