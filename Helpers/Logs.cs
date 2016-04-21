using System;
using System.Windows.Media;
using Styx.Common;

namespace ImpMove.Helpers
{
    internal class Logs
    {
        public static void Log(string str)
        {
            var mesage = String.Format(str);
            Logging.Write(Colors.Aqua, "[ImMove]:{0} ",mesage);
        }

        public void Log(string str,  params object[] arg)
        {
             var mesage = String.Format(str, arg);
            Log(mesage);
        }
    
    }
}
