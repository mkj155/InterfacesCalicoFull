using Calico.Persistencia;
using System.Data.Entity;

namespace Calico.clientes
{
    class tblSubClienteService
    {
        tblSubClienteDAO dao = new tblSubClienteDAO();
        public void Delete(int id)
        {
            dao.Delete(id);
        }

        public DbSet<tblSubCliente> FindAll()
        {
            return dao.FindAll();
        }

        public tblSubCliente FindById(int id)
        {
            return dao.FindById(id);
        }

        public void Save(tblSubCliente obj)
        {
            dao.Save(obj);
        }

        public void Update(tblSubCliente obj)
        {
            dao.Update(obj);
        }

        public int CallProcedure(int? tipoProceso, int? tipoMensaje)
        {
            return dao.CallProcedure(tipoProceso, tipoMensaje);
        }

        public void ExamplePersist()
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

            Save(obj);
            obj = FindById(obj.subc_proc_id);
            obj.subc_esProveedor = false;
            Update(obj);
            Delete(obj.subc_proc_id);
        }
    }
}
