﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Calico.common
{
    class Utils
    {
        /// <summary>
        /// TRUE si la fecha esta en un formato correcto
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns>TRUE si la fecha esta en un formato correcto</returns>
        private static Boolean ValidateDate(String date, String format)
        {
            try {
                DateTime.ParseExact(date, format, null);
            } catch (Exception) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Obtenemos la fecha correcta para el procesamiento
        /// </summary>
        /// <param name="dateArg"></param>
        /// <param name="dateProcess"></param>
        /// <returns>retorna la fechacorrecta para el procesamiento de la itnerface</returns>
        public static DateTime GetDateToProcess(DateTime? dateArg, DateTime? dateProcess)
        {
            DateTime dateTime;
            if (dateArg == null)
            {
                dateTime = Convert.ToDateTime(dateProcess);
                Console.WriteLine("Se procesará la interfaz con la fecha pasada como argumentos: " + dateArg);
            }
            else
            {
                dateTime = Convert.ToDateTime(dateArg);
                Console.WriteLine("Se procesará la interfaz con la fecha de BIANCHI_PROCESS: " + dateProcess);   
            }
            return dateTime;
        }

        /// <summary>
        /// Valida que haya una fecha valida para poder procesar la interface
        /// </summary>
        /// <param name="dateArg"></param>
        /// <param name="dateProcess"></param>
        /// <returns>TRUE si no hay una fecha valida para poder procesar la interface</returns>
        public static bool IsInvalidateDates(DateTime? dateArg, DateTime? dateProcess)
        {
            if (dateArg == null && dateProcess == null)
            {
                Console.WriteLine("La fecha de BIANCHI_PROCESS es NULL y no se indico fecha como parametro, no se ejecutara el proceso");
                Console.WriteLine("Se libera la row de BIANCHI_PROCESS");
                return true;
            }
            return false;
        }

        /// <summary>
        /// DateTime con la fecha pasada como String como parametro
        /// </summary>
        /// <param name="date"></param>
        /// <param name="format"></param>
        /// <returns>DateTime con la fecha pasada como String como parametro</returns>
        private static DateTime ParseDate(String date, String format)
        {
            return DateTime.ParseExact(date, format, null);
        }

        /// <summary>
        /// Retorna la fecha en formato String "YYY/MM/DD
        /// </summary>
        /// <param name="possibleDate"></param>
        /// <returns>Retorna la fecha en formato String "YYY/MM/DD"</returns>
        private static String FormatDate(String possibleDate)
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

        /// <summary>
        /// Retorna la fecha en String formato "YYYY/MM/DD"
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>Retorna la fecha en String formato "YYYY/MM/DD"</returns>
        public static String ConvertDateTimeInString(DateTime dateTime)
        {
            String year = dateTime.Year.ToString("D4");
            String month = dateTime.Month.ToString("D2");
            String day = dateTime.Day.ToString("D2");
            return year + month + day;
        }

        /// <summary>
        /// Retorna la fecha (si es posible) segun el argumento ingresado por el usuario o NULL si no está en un formato correcto
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Retorna la fecha (si es posible) segun el argumento ingresado por el usuario o NULL si no está en un formato correcto</returns>
        public static DateTime? GetDate(string[] args)
        {
            DateTime? dateTime = null;
            if (args != null && args.Length >= 2)
            {
                foreach (String arg in args)
                {
                    String date = FormatDate(arg);
                    if (date.Length > 0 && ValidateDate(date, "yyyy/MM/dd"))
                    {
                        return ParseDate(date, "yyyy/MM/dd");
                    }
                }
            }
            return dateTime;
        }

        /// <summary>
        /// TRUE si por lo menos hay un argumento en la ejecucion
        /// </summary>
        /// <param name="args"></param>
        /// <param name="message"></param>
        /// <returns>TRUE si por lo menos hay un argumento en la ejecucion</returns>
        public static bool ValidateArgs(string[] args, out String message)
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

        /// <summary>
        /// Instancia o NULLEA consola logueo
        /// </summary>
        /// <param name="args"></param>
        public static void InstanceConsole(string[] args)
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

        /// <summary>
        /// Retorna un String con la URL hidrata con parametros
        /// </summary>
        /// <param name="urlParam"></param>
        /// <param name="parameters"></param>
        /// <returns>Retorna un String con la URL hidrata con parametros</returns>
        public static String BuildUrl(String urlParam, Dictionary<String, String> parameters)
        {
            String url = String.Empty;
            foreach (KeyValuePair<string, string> entry in parameters)
            {
                url = urlParam.Replace(entry.Key, entry.Value);
            }
            return url;
        }

        public static String SendRequest(string url, String user, String pass, String key)
        {
            String myJsonString = String.Empty;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + pass));
            request.Headers.Add("Authorization", "Basic " + encoded);
            request.Method = Constants.METHOD_GET;
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                StreamReader reader = new StreamReader(response.GetResponseStream());
                myJsonString = reader.ReadToEnd();
                Console.WriteLine("Servicio Rest OK");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            return myJsonString;
        }

    }
}
