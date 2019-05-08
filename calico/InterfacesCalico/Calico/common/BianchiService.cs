using Calico.Persistencia;
using System;
using System.Data.Entity;
using System.Diagnostics;

namespace Calico.common
{
    class BianchiService
    {
        BianchiProcessDAO dao = new BianchiProcessDAO();
        public void Delete(int id)
        {
            dao.Delete(id);
        }

        public DbSet<BIANCHI_PROCESS> FindAll()
        {
            return dao.FindAll();
        }

        public BIANCHI_PROCESS FindById(int id)
        {
            return dao.FindById(id);
        }

        public void Save(BIANCHI_PROCESS obj)
        {
            dao.Save(obj);
        }

        public void Update(BIANCHI_PROCESS obj)
        {
            dao.Update(obj);
        }

        public DateTime? GetProcessDate(string interfaz)
        {
            return dao.getProcessDate(interfaz);
        }


        public BIANCHI_PROCESS GetProcessInit(DateTime? fechaUltima, String interfaceName)
        {
            BIANCHI_PROCESS obj = new BIANCHI_PROCESS();
            obj.inicio = DateTime.Now;
            obj.maquina = Environment.MachineName;
            obj.process_id = Process.GetCurrentProcess().Id;
            obj.fecha_ultima = fechaUltima;
            obj.interfaz = interfaceName;

            return obj;
        }

        public bool UpdateEnCurso(string interfaz)
        {
            return dao.updateEnCurso(interfaz);
        }

        public bool ValidarSiPuedoProcesar(string interfaz)
        {
            return dao.validarSiPuedoProcesar(interfaz);
        }

        public BIANCHI_PROCESS FindByName(String interfaz)
        {
            return dao.findByName(interfaz);
        }

        public void ExamplePersist()
        {
            BIANCHI_PROCESS obj = GetProcessInit(DateTime.Now, "Cliente");

            /* Example save Entity Framework */
            Save(obj);
            /* Example find by id Entity Framework */
            BIANCHI_PROCESS process = FindById(obj.id);
            /* Example update Entity Framework */
            obj.cant_lineas = 50;
            obj.fin = DateTime.Now;
            obj.estado = "ok";
            Update(obj);
            /* Example delete Entity Framework */
            Delete(obj.id);
        }

        public void BlockRow(int id, String interfaz)
        {
            dao.blockRow(id, interfaz);
        }
    }
}
