using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico.common.mapping
{
    class gridData
    {
        public String id { get; set; }
        public String fullGridId { get; set; }
        public columns columnas { get; set; }
        public List<Rowset> rowsers { get; set; }
        public summary summarys { get; set; }
    }
}
