using InterfacesCalico;
using InterfacesCalico.clientes;

namespace Calico.common
{
    public class InterfaceFactory
    {
        private InterfaceFactory() { }

        public static InterfaceGeneric getInterfaz(string interfaceName)
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
