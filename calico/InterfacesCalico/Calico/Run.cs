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
        public static void selectExample()
        {
            using (calicoEntities context = new calicoEntities())
            {
                /* Obtengo todos los registros de la tabla de esta manera */
                var rows = context.Set<BIANCHI_PROCESS>();
                /* Recorro los registros */
                foreach (var row in rows)
                {
                    decimal? cant_lineas = row.cant_lineas;
                }
            }
        }
        static void Main(string[] args)
        {
            selectExample();

            InterfaceGeneric programa = new InterfaceCliente();
            List<String> parameters = new List<string>();
            parameters.Add("20190305"); // YYYYMMDD
            String url = "http://localhost:8080/calico/rest/message/";
            programa.sendRequest(url, parameters);
            System.Console.WriteLine("Termino exitosamente");
        }
    }
}
