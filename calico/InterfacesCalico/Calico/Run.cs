using Calico;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Net;

namespace InterfacesCalico
{
    public class Run
    {
       
        /* Example select with Entity framework */
        public static DbSet<BIANCHI_PROCESS> findAllProcess()
        {
            using (calicoEntities context = new calicoEntities())
            {
                /* Obtengo todos los registros de la tabla de esta manera */
                var rows = context.Set<BIANCHI_PROCESS>();
                return rows;
            }
        }

        public static void saveProcess(BIANCHI_PROCESS obj)
        {
            using (calicoEntities context = new calicoEntities())
            {
                context.BIANCHI_PROCESS.Add(obj);
                context.SaveChanges();

            }
        }

        public static void deleteProcess(int id)
        {
            using (calicoEntities context = new calicoEntities())
            {
                BIANCHI_PROCESS obj = new BIANCHI_PROCESS { id = id };
                context.BIANCHI_PROCESS.Attach(obj);
                context.BIANCHI_PROCESS.Remove(obj);
                context.SaveChanges();
            }
        }

        public static void updateProcess(BIANCHI_PROCESS obj)
        {
            using (calicoEntities context = new calicoEntities())
            {
                var result = context.BIANCHI_PROCESS.Find(obj.id);
                if (result == null) return;
                context.Entry(result).CurrentValues.SetValues(obj);
                context.SaveChanges();
            }
        }

        public static BIANCHI_PROCESS findById(int id)
        {
            using (calicoEntities context = new calicoEntities())
            {
                return context.BIANCHI_PROCESS.Find(id);
            }
        }

        public static BIANCHI_PROCESS getObjectTest()
        {
            BIANCHI_PROCESS obj = new BIANCHI_PROCESS();
            obj.inicio = new DateTime(2008, 5, 1, 8, 30, 52);
            obj.fin = new DateTime(2008, 5, 1, 9, 30, 52);
            obj.cant_lineas = 20;
            obj.estado = "ok";
            obj.maquina = "windows";
            obj.process_id = 1234;
            obj.fecha_ultima = DateTime.Now;
            obj.@interface = "cliente";

            return obj;
        }


        static void Main(string[] args)
        {
            BIANCHI_PROCESS obj = getObjectTest();
            /* Example find all Entity Framework */
            DbSet<BIANCHI_PROCESS> list = findAllProcess();
            /* Example save Entity Framework */
            saveProcess(obj);
            /* Example find by id Entity Framework */
            BIANCHI_PROCESS process = findById(obj.id);
            /* Example update Entity Framework */
            obj.cant_lineas = 50;
            updateProcess(obj);
            /* Example delete Entity Framework */
            deleteProcess(obj.id);


            InterfaceGeneric programa = new InterfaceCliente();
            List<String> parameters = new List<string>();
            parameters.Add("20190305"); // YYYYMMDD
            String url = "http://localhost:8080/calico/rest/message/";
            programa.sendRequest(url, parameters);
            System.Console.WriteLine("Termino exitosamente");
        }
    }
}
