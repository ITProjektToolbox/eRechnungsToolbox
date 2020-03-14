
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;

namespace XRechnungsDruckerSetupCustomAction
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            string printerName = "XRechnungsDrucker";
            base.Install(stateSaver);
            LogHelper.Log("Install Started.");

                try
                {
                    SpoolerHelper sh = new SpoolerHelper();
                    SpoolerHelper.GenericResult result = sh.AddVPrinter(printerName, printerName);
                    if (result.Success == false)
                    {
                        LogError(result.Method, result.Message, result.Exception);
                        throw new InstallException(string.Format("Source: {0}\nMessage: {1}", result.Method, result.Message), result.Exception);
                    }
                }
                catch (Exception ex)
                {
                    LogError("AddVPrinter", ex.Message, ex);
                }
            LogHelper.Log("Install Finished.");
        }

        private static void LogError(string exceptionSource, string message, Exception innerException)
        {
            string eventMessage = string.Format("Source: {0}\nMessage: {1}\nInnerException: {2}", exceptionSource, message, innerException);
            LogHelper.Log(eventMessage);
        }
    }
}
