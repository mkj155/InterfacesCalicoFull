using System;
using System.IO;

namespace Calico.common
{
    class Utils
    {
        private static Boolean validateDate(String date)
        {
            try {
                DateTime.ParseExact(date, "yyyy/MM/dd", null);
            } catch (Exception) {
                return false;
            }
            return true;
        }

        private static DateTime? parseDate(String date, String format)
        {
            try {
                return DateTime.ParseExact(date, format, null);
            } catch (Exception) { 
                return null;
            }
        }

        private static String formatDate(String possibleDate)
        {
            String date = String.Empty;
            if (possibleDate.Length == 8)
            {
                String yyyy = possibleDate.Substring(0, 4);
                String mm = possibleDate.Substring(4, 2);
                String dd = possibleDate.Substring(6, 2);
                date = yyyy + "/" + mm + "/" + dd;
            }
            return date;
        }


        public static DateTime? getDate(string[] args)
        {
            DateTime? dateTime = null;
            if (args != null && args.Length >= 2)
            {
                foreach (String arg in args)
                {
                    String date = formatDate(arg);
                    if (date.Length > 0 && validateDate(date))
                    {
                        dateTime = parseDate(date, "yyyy/MM/dd");
                    }
                }
            }
            return dateTime;
        }

        public static bool validateArgs(string[] args, out String message)
        {
            bool isValid = true;
            message = String.Empty;
            if (args == null && args.Length == 0)
            {
                isValid = false;
                message = "Se debe especificar minimamente la interfaz a procesar";
            }
            return isValid;
        }

        public static void instanceConsole(string[] args)
        {
            bool mustWrite = false;
            if (args != null && args.Length >= 2)
            {
                foreach (String arg in args)
                {
                    if (Constants.MUST_LOG.Equals(arg))
                    {
                        mustWrite = true;
                    }
                }
            }
            if(!mustWrite) Console.SetOut(TextWriter.Null);
        }

    }
}
