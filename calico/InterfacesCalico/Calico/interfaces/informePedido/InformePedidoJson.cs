using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.informePedido
{
    class InformePedidoJson
    {
        public String P554211I_Version { get; set; }
        public List<InformePedidoDTO> ConfirmArray { get; set; }
        public InformePedidoJson(List<InformePedidoDTO> informePedidoDTOList)
        {
            ConfirmArray = informePedidoDTOList;
        }
    }
}
