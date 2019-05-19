using Calico.DAOs;
using Calico.persistencia;
using System;
using System.Data.Entity;
using System.Diagnostics;

namespace Calico.service
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
            try
            {
                return dao.findByName(interfaz);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public bool LockRow(int id)
        {
            try
            {
                dao.LockRow(id);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public void UnlockRow() {
            dao.UnlockRow();
        }

    }
}
