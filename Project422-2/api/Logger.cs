using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

/*********************************************************************************************************/
//  Logging functionality.
//  Provides three levels of logging: info, error, debug
//  
//  Author: Pravin
//  Created On: 16/10/2012
/*********************************************************************************************************/
namespace Project422_2.api
{
    class Logger
    {
        /*-------------- Private variables ------------------*/
        static string logInfoFile,logErrorFile,logDebugFile;

        #region /********************************** Internal methods *******************************/
     
        /// <summary>
        /// Static constructor
        /// </summary>
        static Logger()
        {
            /*-------------- INIT LOG FILE LOCATIONS ------------------*/
            logInfoFile = "log/info.log";
            logErrorFile = "log/error.log";
            logDebugFile = "log/debug.log";

            // create directories if they do not exist
            if (!Directory.Exists("log/"))
            {
                Directory.CreateDirectory("log/");
            }           

        }

        /// <summary>
        /// logs the given message to the specified file
        /// </summary>
        private static void writeLog(string msg, string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename, true))
            {
                String timestamp = System.DateTime.Now.ToString();
                writer.WriteLine(timestamp + ":" + msg);
                writer.Flush();
            }
        }

        #endregion

        #region /********************************** External methods *******************************/
        /// <summary>
        /// General log that goes to creditcard.log file
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(String msg)
        {
            writeLog(msg, logInfoFile);
        }
     
        /// <summary>
        /// Erro log that goes to creditcard.err file
        /// Called from exception handlers
        /// </summary>
        /// <param name="msg"></param>
        public static void Error(String msg)
        {
            writeLog(msg, logErrorFile);
        }

        /// <summary>
        /// Displays a message box. for debuggin purpose only
        /// Should be removed during deployment
        /// </summary>
        /// <param name="msg"></param>
        public static void Debug(String msg)
        {
            writeLog(msg, logDebugFile);
            System.Console.WriteLine(msg);
        }

        #endregion

    }

}
