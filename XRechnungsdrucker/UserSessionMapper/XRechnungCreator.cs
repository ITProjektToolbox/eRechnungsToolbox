
using Ghostscript.NET;
using Ghostscript.NET.Processor;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace XRechnungs_Drucker
{
    class XRechnungCreator
    {

        public static void CreateXML(string filename, string psfileName)
        {
            string pdfFileName = PsToPDF(psfileName);
            string extractedText = PdfToText(pdfFileName);
            var fields = FieldExtractor.ExtractFieldsFromText(extractedText);            
            var lineFields = FieldExtractor.ExtractInvoiceLineFields(pdfFileName);
            var vatBreakDown = FieldExtractor.ComputeTotalsAndCreateVATBreakdown(lineFields, fields);
            var invoice = UBLMapping.CreateInvoice(fields, lineFields, vatBreakDown);

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("ubl", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");
            ns.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
            ns.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
            ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            ns.Add("qdt", "urn:oasis:names:specification:ubl:schema:xsd:QualifiedDataTypes-2");
            ns.Add("udt", "urn:oasis:names:specification:ubl:schema:xsd:UnqualifiedDataTypes-2");
            ns.Add("ccts", "urn:un:unece:uncefact:documentation:2");
            ns.Add("", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2");

            XmlSerializer serializer = new XmlSerializer(typeof(InvoiceType));
            TextWriter writer = new StreamWriter(filename);


            serializer.Serialize(writer, invoice, ns);
            writer.Close();
        }

        private static string PsToPDF(string psfilename)
        {
            string inputFile = psfilename;
            GhostscriptPipedOutput gsPipedOutput = new GhostscriptPipedOutput();

            string pdfFilePath = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".pdf";


            // pipe handle format: %handle%hexvalue
            string outputPipeHandle = "%handle%" + int.Parse(gsPipedOutput.ClientHandle).ToString("X2");

            using (GhostscriptProcessor processor = new GhostscriptProcessor())
            {
                List<string> switches = new List<string>();
                switches.Add("-empty");
                switches.Add("-dQUIET");
                switches.Add("-dSAFER");
                switches.Add("-dBATCH");
                switches.Add("-dNOPAUSE");
                switches.Add("-dNOPROMPT");
                switches.Add("-sDEVICE=pdfwrite");
                switches.Add("-o" + outputPipeHandle);
                switches.Add("-q");
                switches.Add("-f");
                switches.Add(inputFile);

                try
                {
                    processor.StartProcessing(switches.ToArray(), null);

                    byte[] rawDocumentData = gsPipedOutput.Data;

                    File.WriteAllBytes(pdfFilePath, rawDocumentData);
                    return pdfFilePath;

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message + "hallo");
                }
                finally
                {
                    gsPipedOutput.Dispose();
                    gsPipedOutput = null;

                }
            }
        }


        private static string PdfToText(string pdfFileName)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(pdfFileName));

            //Get the number of pages in pdf.
            int pages = pdfDoc.GetNumberOfPages();

            //Iterate the pdf through pages.
            string pdfContent = "";
            for (int i = 1; i <= pages; i++)
            {
                //Extract the page content using PdfTextExtractor.
                String pageContent = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(i));
                pdfContent += " " + pageContent;
            }
            return pdfContent;
        }
    
        private static string PdfToCsv(string pdfFileName)
        {
            //CSVExtractor extractor = new CSVExtractor();
            //extractor.RegistrationName = "demo";
            //extractor.RegistrationKey = "demo";

            //TableDetector tdetector = new TableDetector();
            //tdetector.RegistrationKey = "demo";
            //tdetector.RegistrationName = "demo";

            //// Load the document
            //extractor.LoadDocumentFromFile(pdfFileName);
            //tdetector.LoadDocumentFromFile(pdfFileName);

            //int pageCount = tdetector.GetPageCount();

            //for (int i = 0; i < pageCount; i++)
            //{
            //    int j = 1;

            //    do
            //    {
            //        extractor.SetExtractionArea(tdetector.GetPageRect_Left(i),
            //        tdetector.GetPageRect_Top(i),
            //        tdetector.GetPageRect_Width(i),
            //        tdetector.GetPageRect_Height(i)
            //    );

            //        // and finally save the table into CSV file
            //        extractor.SavePageCSVToFile(i, "C:\\Test\\page-" + i + "-table-" + j + ".csv");
            //        j++;
            //    } while (tdetector.FindNextTable()); // search next table
            //}
            return null;
        }
    
    }
}
