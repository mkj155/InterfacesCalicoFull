using Calico;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Net;

namespace InterfacesCalico
{
    public class Run
    {

        static void Main(string[] args)
        {
            IConfigSource source = new IniConfigSource("calico_config.ini");
            string urlPath = source.Configs["Clientes"].Get("url");

            BianchiService serviceProcess = new BianchiService();
            serviceProcess.examplePersist();
            tblSubClienteService serviceCliente = new tblSubClienteService();
            serviceCliente.examplePersist();

            InterfaceGeneric programa = new InterfaceCliente();
            List<String> parameters = new List<string>();
            parameters.Add("20190305"); // YYYYMMDD
            programa.sendRequest(urlPath,parameters);
            System.Console.WriteLine("Termino exitosamente");
        }
    }
}
