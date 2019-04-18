using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace InterfacesCalico
{
    public class Run
    {
        static void Main(string[] args)
        {
            InterfaceGeneric programa = new InterfaceCliente();
            List<String> parameters = new List<string>();
            parameters.Add("20190305"); // YYYYMMDD
            String url = "http://localhost:8080/calico/rest/message/";
            programa.sendRequest(url, parameters);
            System.Console.WriteLine("Termino exitosamente");
        }
    }
}
