using System;
using System.Diagnostics;

namespace AOC2019
{
    public static class HelperFunctions
    {
        public static void Log(string s)
        {
            if (Debugger.IsAttached) Debug.WriteLine(s);
            Console.WriteLine(s);
        }
    }
}
