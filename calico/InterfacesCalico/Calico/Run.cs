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

            BianchiService service = new BianchiService();
            BIANCHI_PROCESS process = service.findById(1);


            // Validamos la existencia de argumentos
            String message = null;
            if (!Utils.validateArgs(args, out message))
            {
                Console.Error.WriteLine(message);
                return;
            }

            // Se loguea si un argumento es "/l"
            Utils.instanceConsole(args);

            // Validacion de fecha
            DateTime? dateTime = Utils.getDate(args);
            if (dateTime == null)
            {
                Console.WriteLine("Fecha no indicada se tomará de la tabla BIANCHI_PROCESS");
            }

            // Cargamos archivo con parametros propios para cada interface
            IConfigSource source = new IniConfigSource("calico_config.ini");

            // Instanciamos la interface que llego como primer argumento
            InterfaceGeneric interfaz = (args != null && args.Length > 0) ? InterfaceFactory.getInterfaz(args[0]) : null;
            if (interfaz == null)
            {
                Console.Error.WriteLine("interface inexistente");
                return;
            }

            // Procesamos
            interfaz.process(source, dateTime);
            Console.WriteLine("Termino exitosamente");
        }

    }

}
