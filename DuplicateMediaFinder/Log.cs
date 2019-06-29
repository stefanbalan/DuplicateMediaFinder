using System;
using System.Runtime.CompilerServices;

namespace DuplicateMediaFinder
{
    internal class Log
    {
        public static void Info(string messsage, [CallerMemberName]string caller = "")
        {
            Write("INFO", messsage, caller);
        }

        public static void Debug(string messsage, [CallerMemberName]string caller = "")
        {
            Write("DEBUG", messsage, caller);
        }

        public static void Error(string messsage, [CallerMemberName]string caller = "")
        {
            Write("ERROR", messsage, caller);
        }

        private static void Write(string level, string message, string caller)
        {
            var l = $"[{level}]";
            Console.WriteLine($"{l.PadRight(7)} {caller} : {message}");
        }
    }
}
