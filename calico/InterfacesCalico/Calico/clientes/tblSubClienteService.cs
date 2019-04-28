using Calico.Persistencia;
using System;
using System.Data.Entity;

namespace Calico.clientes
{
    class tblSubClienteService
    {
        tblSubClienteDAO dao = new tblSubClienteDAO();
        public void delete(int id)
        {
            dao.delete(id);
        }

        public DbSet<tblSubCliente> findAll()
        {
            return dao.findAll();
        }

        public tblSubCliente findById(int id)
        {
            return dao.findById(id);
        }

        public void save(tblSubCliente obj)
        {
            dao.save(obj);
        }

        public void update(tblSubCliente obj)
        {
            dao.update(obj);
        }

        public void callProcedure(String nombre)
        {
            dao.callProcedure(nombre);
        }

        public void examplePersist()
        {
            tblSubCliente obj = new tblSubCliente();

            obj.subc_proc_id = 2;
            obj.subc_codigo = "123";
            obj.subc_codigoCliente = "456";
            obj.subc_razonSocial = "hola";
            obj.subc_domicilio = "Arana 123";
            obj.subc_localidad = "Moron";
            obj.subc_codigoPostal = "1708";
            obj.subc_areaMuelle = "area 1";
            obj.subc_telefono = "4697-6545";
            obj.subc_diasVencimiento = 14;
            obj.subc_esProveedor = true;
            obj.subc_cuit = "11956789";
            obj.subc_iva = "17";
            obj.subc_correoElectronico = "mkj155@gmail.com";

            save(obj);
            obj = findById(obj.subc_proc_id);
            obj.subc_esProveedor = false;
            update(obj);
            delete(obj.subc_proc_id);
        }
    }
}
