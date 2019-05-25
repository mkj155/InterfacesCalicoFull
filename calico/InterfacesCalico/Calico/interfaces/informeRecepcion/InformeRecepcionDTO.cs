using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.informeRecepcion
{
    class InformeRecepcionDTO
    {
        public String OrderCompany { get; set; }
        public String OrderType { get; set; }
        public String OrderNumber { get; set; }
        public String OrderLine { get; set; }
        public String QuantityToRecieve { get; set; }
        public String ReceiptDate { get; set; }
        public String Lot { get; set; }
    }
}
