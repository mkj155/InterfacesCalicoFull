using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calico
{
    class Utils
    {
        public static Boolean validateDate(String date){
            try{
                DateTime.ParseExact(date, "YYYYMMDD", null);
            }catch (Exception ex){
                /* Logear en consola */
                return false;
            }

            return true;
            
        }

        public static DateTime? parseDate(String date,String format){
            try{
                return DateTime.ParseExact(date, format, null);
            }catch (Exception ex) { 
                /* Logear en consola */
                return null;
            }
        }
    }
}
