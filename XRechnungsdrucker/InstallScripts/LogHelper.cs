
using System;
namespace XRechnungsDruckerSetupCustomAction
{
    public static class LogHelper
    {
        public static void Log(string msg)
        {
            var filename = "C:\\XRechnungsDrucker_Installer.txt";
            using (var sw = new System.IO.StreamWriter(filename, true))
            {
                sw.Write(string.Format("{0} - {1}\n", DateTime.Now, msg));
                sw.Flush();
            }
        }
    }
}
