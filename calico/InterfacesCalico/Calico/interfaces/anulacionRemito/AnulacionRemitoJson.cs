using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.anulacionRemito
{
    class AnulacionRemitoJson
    {
        public String P554211I_Version { get; set; }
        public List<AnulacionRemitoDTO> ConfirmArray { get; set; }
        public AnulacionRemitoJson(List<AnulacionRemitoDTO> informePedidoDTOList)
        {
            ConfirmArray = informePedidoDTOList;
        }
    }
}
