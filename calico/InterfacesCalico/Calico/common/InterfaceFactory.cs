using Calico.interfaces.clientes;
using Calico.interfaces.recepcion;
using InterfacesCalico.generic;

namespace Calico.common
{
    public class InterfaceFactory
    {
        private InterfaceFactory() { }

        public static InterfaceGeneric GetInterfaz(string interfaceName)
        {
            InterfaceGeneric interfaz = null;
            if (Constants.INTERFACE_CLIENTES.Equals(interfaceName))
            {
                interfaz = new InterfaceCliente();
            }
            else if (Constants.INTERFACE_RECEPCION.Equals(interfaceName))
            {
                interfaz = new InterfaceRecepcion();
            }
            return interfaz;
        }

    }

}
