using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WeldDataSetter
{
    public static class LogHelper
    {
        public static void WriteLogEntry(this StreamWriter writer, string level, string message)
        {
            writer.WriteLine($"[{DateTimeOffset.Now.ToString()}]:[{level}] {message}");
        }

        public static void WriteInfo(this StreamWriter writer, string message)
        {
            writer.WriteLogEntry("INFO", message);
        }

        public static void WriteError(this StreamWriter writer, Exception exception)
        {
            string message = $"{exception.Message}\r\n {exception.StackTrace}";
            writer.WriteLogEntry("ERROR", message);
        }
    }
}
