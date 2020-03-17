using Bytescout.PDFExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XRechnungs_Drucker
{
    class FieldExtractor
    {
        internal static Dictionary<string, string> ExtractFieldsFromText(string extractedText)
        {
            var fields = new Dictionary<string, string>();
            var cA = extractedText.ToCharArray();
            for (int i = 0; i < extractedText.Length; i++)
            {
                if (cA[i].Equals('<'))
                {
                    ExtractField(ref fields, ref cA, ref i);
                }
            }

            return fields;
        }

        private static void ExtractField(ref Dictionary<string, string> fields, ref char[] cA, ref int i)
        {
            i++;
            int startKeyIndex = i;
            IterateToNextChar(ref cA, ref i, ' ');
            string key = new String(cA, startKeyIndex, i - startKeyIndex).Trim();

            if (fields.TryGetValue(key, out string s))
                key = IterateName(key);

            i++;
            int startValueIndex = i;
            IterateToNextChar(ref cA, ref i, '>');
            string value = new String(cA, startValueIndex, i - startValueIndex).Trim();

            if (value != "")
                fields.Add(key, value);
            i++;
        }

        private static string IterateName(string key)
        {
            if (key.Contains('+'))
            {
                return key.Split('x')[0] + "+" + Int32.Parse(key.Split('+')[1]) + 1;
            }
            return key + "+1";
        }

        private static void IterateToNextChar(ref char[] cA, ref int i, char c)
        {
            while (i < cA.Length && !cA[i].Equals(c))
            {
                i++;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        internal static List<Dictionary<string,string>> ExtractInvoiceLineFields(string pdfPath)
        {
            List<Dictionary<string, string>> invoiceLineFields = null;

            // Initialise table detector
            using (TableDetector tableDetector = new TableDetector("demo", "demo"))
            {
                using (CSVExtractor CSVExtractor = new CSVExtractor("demo", "demo"))
                {
                    // Set table detection mode to "bordered tables" - best for tables with closed solid borders.
                    tableDetector.ColumnDetectionMode = ColumnDetectionMode.BorderedTables;

                    // We should define what kind of tables we should detect.
                    // So we set min required number of columns to 2 ...
                    tableDetector.DetectionMinNumberOfColumns = 2;
                    // ... and we set min required number of rows to 2
                    tableDetector.DetectionMinNumberOfRows = 1;

                    // Load PDF document
                    tableDetector.LoadDocumentFromFile(pdfPath);
                    CSVExtractor.LoadDocumentFromFile(pdfPath);

                    // Get page count
                    int pageCount = tableDetector.GetPageCount();

                   if(tableDetector.FindTable(pageCount-1))
                    {
                        // Set extraction area for CSV extractor to rectangle received from the table detector
                        CSVExtractor.SetExtractionArea(tableDetector.FoundTableLocation);

                        // Generate CSV data
                        var allCsvData = CSVExtractor.GetCSV();

                        // Generate Datatable
                        invoiceLineFields = GetFieldsFromCSV(allCsvData);
                    }
                }
            }

            return invoiceLineFields;
        }

        private static List<Dictionary<string, string>> GetFieldsFromCSV(string allCsvData)
        {

            var rows = allCsvData.Split('\n');
            int start = 0;
            int totals = 0;
            string[] columns = null;
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

            for (int iRow = 1; iRow < rows.Length; iRow++)
            {
                // Get all column data
                string[] row = rows[iRow].Split(';');
                if(row.First().Replace("\"", "").Equals("BT-126"))
                {
                    for (int i = 0; i < row.Count(); i++)
                        row[i] = row[i].Replace("\"", "");

                    columns = row;
                    start = iRow + 1;
                    break;
                }
            }


            for (int iRow = start; iRow < rows.Length; iRow++)
            {
                Dictionary<string, string> rowDict = new Dictionary<string, string>();
                string[] row = rows[iRow].Split(';');
                bool ended = false;

                for (int i = 0; i < row.Count() - 1; i++)
                {
                    if (row[i].Replace("\"", "").Equals("<totals>"))
                    {
                        totals = iRow + 1;
                        ended = true;
                        break;
                    }
                    if(!columns[i].Equals(""))
                        rowDict.Add(columns[i], row[i].Replace("\"", ""));
                }

                if (ended)
                    break;

                //add standard fields
                rowDict.Add("BT-130", "XPP");
                rowDict.Add("BT-151", "S");


                if(rowDict.TryGetValue("BT-126", out string s))
                {
                    if(!s.Equals(""))
                        ret.Add(rowDict);
                }
            }



                return ret;
        }

        internal static  List<Dictionary<string, string>> ComputeTotalsAndCreateVATBreakdown(List<Dictionary<string, string>> inLines, Dictionary<string, string> fields)
        {
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();
            Dictionary<string, string> vat19dict = new Dictionary<string, string>();
            Dictionary<string, string> vat7dict = new Dictionary<string, string>();
            decimal sum19 = 0;
            decimal sum7 = 0;
            decimal remaining = 0;

            foreach(var l in inLines)
            {
                if(l.TryGetValue("BT-152", out string s))
                {
                    if (s.Equals("19%"))
                    {
                        if (l.TryGetValue("BT-131", out string t))
                        {
                            sum19 += Decimal.Parse(t.Remove(t.Length-1));
                        }
                    }

                    else if (s.Equals("7%"))
                    {
                        if (l.TryGetValue("BT-131", out string t))
                        {
                            sum7 += Decimal.Parse(t.Remove(t.Length - 1));
                        }
                    }

                    else
                    {
                        if (l.TryGetValue("BT-131", out string t))
                        {
                            remaining += Decimal.Parse(t.Remove(t.Length - 1));
                        }
                    }
                }
            }

            vat19dict.Add("BT-116", Math.Round(sum19, 2).ToString());
            vat19dict.Add("BT-118", "S");
            vat19dict.Add("BT-119", "19");
            decimal vat19 = Math.Round(sum19 * (decimal)0.19, 2, MidpointRounding.ToPositiveInfinity);
            vat19dict.Add("BT-117", vat19.ToString());
            if(sum19 != 0)
                ret.Add(vat19dict);

            vat7dict.Add("BT-116", Math.Round(sum7, 2).ToString());
            vat7dict.Add("BT-118", "S");
            vat7dict.Add("BT-119", "7");
            decimal vat7 = Math.Round(sum7 * (decimal)0.07, 2, MidpointRounding.ToPositiveInfinity);
            vat7dict.Add("BT-117", vat7.ToString());
            if (sum7 != 0)
                ret.Add(vat7dict);

            decimal totalVat = vat7 + vat19;
            decimal totalNetto = Math.Round(sum19 + sum7 + remaining, 2);
            fields.Add("BT-106", totalNetto.ToString());
            fields.Add("BT-109", totalNetto.ToString());
            fields.Add("BT-110", totalVat.ToString());
            fields.Add("BT-112", (totalNetto + totalVat).ToString());
            fields.Add("BT-115", (totalNetto + totalVat).ToString());

            return ret;
        }
    }
}
