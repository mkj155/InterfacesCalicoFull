using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.interfaces.anulacionRemito
{
    class AnulacionRemitoDTO
    {
        public string OrderCompany { get; set; }
        public string OrderNumber { get; set; }
        public string OrderType { get; set; }
        public string OrderLineNumber { get; set; }
        public string Lot { get; set; }
        public string ItemNumber { get; set; }
        public string ChgLastStatus { get; set; }
        public string ChgReference { get; set; }
        public string ChgNextStatus { get; set; }
        public string ChgDispatchQuantity { get; set; }
        public string ChgLot { get; set; }
        public string ChgDispatchDate { get; set; }
    }
}
