using Calico.interfaces.clientes;
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
            return interfaz;
        }

    }

}
