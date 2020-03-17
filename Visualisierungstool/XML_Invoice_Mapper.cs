using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace XRechnung2PDF
{
    class XML_Invoice_Mapper
    {

        public static invoice MapToInvoice(string xml)
        {           
            var p = new Saxon.Api.Processor();
            var c = p.NewXsltCompiler();
            string rootNodeName = XDocument.Load(xml).Root.Name.LocalName;
            Saxon.Api.XsltExecutable ex;
            if (rootNodeName == "Invoice")
            {               
                ex = c.Compile(GenerateStreamFromString(Properties.Resources.ubl_invoice_xr));
            }
            else if(rootNodeName == "CrossIndustryInvoice")
            {
                ex = c.Compile(GenerateStreamFromString(Properties.Resources.cii_xr));
            }
            else
            {               
                return null;
            }
            var transformer = ex.Load();
            transformer.SetInputStream(File.OpenRead(xml), new Uri(@"C:\")); //TODO Temp URI
            var destination = new Saxon.Api.DomDestination();
            transformer.Run(destination);
            var ms = new MemoryStream();
            destination.XmlDocument.Save(ms);
            ms.Flush();
            ms.Position = 0;
            var s = new XmlSerializer(typeof(invoice));
            return (invoice) s.Deserialize(ms);    
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

    }

}
