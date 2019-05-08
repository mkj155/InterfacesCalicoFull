using Calico;
using Calico.common;
using Calico.Persistencia;
using Nini.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InterfacesCalico
{
    public class Run
    {
        static void Main(string[] args)
        {
            // Validamos la existencia de argumentos
            String message = null;
            if (!Utils.ValidateArgs(args, out message))
            {
                Console.Error.WriteLine(message);
                return;
            }

            // Se loguea si un argumento es "/l"
            Utils.InstanceConsole(args);
            Console.WriteLine("Logueo activado");

            // Validacion de fecha
            DateTime ? dateTime = Utils.GetDate(args);
            if (dateTime == null)
            {
                Console.WriteLine("Fecha no indicada se tomará de la tabla BIANCHI_PROCESS");
            }

            // Instanciamos la interface que llego como primer argumento
            InterfaceGeneric interfaz = (args != null && args.Length > 0) ? InterfaceFactory.GetInterfaz(args[0]) : null;
            if (interfaz == null)
            {
                Console.Error.WriteLine("Interface inexistente");
                return;
            }

            // Procesamos
            interfaz.Process(dateTime);
        }

    }

}
