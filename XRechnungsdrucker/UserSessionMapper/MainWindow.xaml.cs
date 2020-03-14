using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XRechnungs_Drucker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateAndListenNamedPipe(string outputFilename)
        {
            string psfileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".ps";
            using (NamedPipeServerStream pipeServer =
            new NamedPipeServerStream("XRechnungsDruckerPipe", PipeDirection.In))
            {
                pipeServer.WaitForConnection();

                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(psfileName))
                    {
                        using (StreamReader sr = new StreamReader(pipeServer))
                        {
                            file.Write(sr.ReadToEnd());
                        }
                    }
                    ShowSaveDialog(psfileName);
                }

                catch (IOException e)
                {
                    Console.WriteLine("ERROR: {0}", e.Message);
                }
            }
        }

        private void ShowSaveDialog(string psfilename)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.AddExtension = true;
            saveFileDialog.Filter = "UBL-XRechnung (*.xml)|*.xml";
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.Title = "XRechnungsDrucker";
            if (saveFileDialog.ShowDialog() == true)
                XRechnungCreator.CreateXML(saveFileDialog.FileName, psfilename);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            while (true)
            {
                CreateAndListenNamedPipe("test.xml");
            }
        }
    }
}
