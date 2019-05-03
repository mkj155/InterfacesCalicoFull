using Calico.Persistencia;
using System;
using System.Data.Entity;
using System.Diagnostics;

namespace Calico.common
{
    class BianchiService
    {
        BianchiProcessDAO dao = new BianchiProcessDAO();
        public void delete(int id)
        {
            dao.delete(id);
        }

        public DbSet<BIANCHI_PROCESS> findAll()
        {
            return dao.findAll();
        }

        public BIANCHI_PROCESS findById(int id)
        {
            return dao.findById(id);
        }

        public void save(BIANCHI_PROCESS obj)
        {
            dao.save(obj);
        }

        public void update(BIANCHI_PROCESS obj)
        {
            dao.update(obj);
        }

        public DateTime? getProcessDate(string interfaz)
        {
            return dao.getProcessDate(interfaz);
        }


        public BIANCHI_PROCESS getProcessInit(DateTime? fechaUltima, String interfaceName)
        {
            BIANCHI_PROCESS obj = new BIANCHI_PROCESS();
            obj.inicio = DateTime.Now;
            obj.maquina = Environment.MachineName;
            obj.process_id = Process.GetCurrentProcess().Id;
            obj.fecha_ultima = fechaUltima;
            obj.interfaz = interfaceName;

            return obj;
        }

        public bool updateEnCurso(string interfaz)
        {
            return dao.updateEnCurso(interfaz);
        }
        public bool validarSiPuedoProcesar(string interfaz)
        {
            return dao.validarSiPuedoProcesar(interfaz);
        }

        public BIANCHI_PROCESS findByName(String interfaz)
        {
            return dao.findByName(interfaz);
        }

        public void examplePersist()
        {
            BIANCHI_PROCESS obj = getProcessInit(DateTime.Now, "Cliente");

            /* Example save Entity Framework */
            save(obj);
            /* Example find by id Entity Framework */
            BIANCHI_PROCESS process = findById(obj.id);
            /* Example update Entity Framework */
            obj.cant_lineas = 50;
            obj.fin = DateTime.Now;
            obj.estado = "ok";
            update(obj);
            /* Example delete Entity Framework */
            delete(obj.id);
        }
    }
}
