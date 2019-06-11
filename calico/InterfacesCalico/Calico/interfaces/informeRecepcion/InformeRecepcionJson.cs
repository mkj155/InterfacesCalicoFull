using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.informeRecepcion
{
    class InformeRecepcionJson
    {
        public List<InformeRecepcionDTO> ReceiptsArray { get; set; }
        public String ReceiptsVersion { get; set; }

        public InformeRecepcionJson(List<InformeRecepcionDTO> informeRecepcionDTOList, String receiptsVersion)
        {
            ReceiptsArray = informeRecepcionDTOList;
            ReceiptsVersion = receiptsVersion;
        }
    }
}
