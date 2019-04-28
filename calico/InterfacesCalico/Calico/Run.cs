using Calico;
using Calico.common;
using Nini.Config;
using System;
using System.Collections.Generic;

namespace InterfacesCalico
{
    public class Run
    {

        static void Main(string[] args)
        {
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
                Console.WriteLine("Se va a procesar una interfaz sin especificar fecha");
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
