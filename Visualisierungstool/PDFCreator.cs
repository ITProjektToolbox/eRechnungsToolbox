using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;

namespace XRechnung2PDF
{
    class PDFHelper
    {

    }
    class PDFCreator                            
    {
        private static Border border = Border.NO_BORDER;
        private static float margin = 36;
        private static float footerHeight = 60;
        private static Cell CreateCell(text t)
        {
            var c = new Cell();
            c.SetBorder(border);
            c.SetMinHeight(0);

            if (t == null || t.Value == null)
                return null;

            return c.Add(new Paragraph(t.ToString()));
        }

        private static Cell CreateCell(string s)
        {
            var c = new Cell();
            c.SetBorder(border);
            c.SetMinHeight(0);

            return c.Add(new Paragraph(s));
        }

        private static Cell CreateCell(text t, string s)
        {
            var c = new Cell();
            c.SetBorder(border);
            c.SetMinHeight(0);

            if (t == null || t.Value == null)
                return null;

            return c.Add(new Paragraph(s + t.ToString()));
        }

        private static Cell CreateCell(identifier i, string s)
        {
            var c = new Cell();
            c.SetBorder(border);
            c.SetMinHeight(0);

            if (i == null || i.Value == null)
                return null;

            return c.Add(new Paragraph(s + i.Value));
        }

        private static Cell CreateSpacing(int space)
        {
            var c = new Cell();
            c.SetBorder(border);
            c.SetMinHeight(space);
            return c;
        }

        private static Table Totals(invoice invoice)
        {
            Table totals = null;
            try
            {
                totals = new Table(2);
                totals.SetFontSize(10);
                totals.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                totals.SetWidth(200);
                totals.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT);

                totals.AddCell(new Cell(1, 2).SetMinHeight(20).SetBorder(border));

                totals.AddTextCell(CreateCell("Summe Netto").SetBold().SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));
                totals.AddTextCell(CreateCell(invoice.DOCUMENT_TOTALS.Invoice_total_amount_without_VAT.Value + " €").SetBold().SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

                totals.AddTextCell(CreateCell("Umsatzsteuer " + invoice.INVOICE_LINE.First().LINE_VAT_INFORMATION.Invoiced_item_VAT_rate.Value + "%").SetBold().SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));
                totals.AddTextCell(CreateCell(invoice.DOCUMENT_TOTALS.Invoice_total_VAT_amount.Value + " €").SetBold().SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));

                totals.AddTextCell(CreateCell("Rechnungsbetrag").SetBold().SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));
                totals.AddTextCell(CreateCell(invoice.DOCUMENT_TOTALS.Invoice_total_amount_with_VAT.Value + " €").SetBold().SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT));
            }

            catch(Exception e) { return null; }
                
            return totals;
        }

        private static Cell CreateCellM(IEnumerable<text> ts, string p)
        {
            var c = new Cell();
            c.SetBorder(border);
            c.SetMinHeight(0);

            if (ts == null)
                return null;
            string s = "";
            foreach(text t in ts)
            {
                if (t == null || t.Value == null)
                    continue;

                s += t.ToString();
                s += p;
            }
            return c.Add(new Paragraph(s.Remove(s.Length - p.Length, p.Length)));
        }

        private static Table BuyerAdress(invoice invoice)
        {
            var adresseAndReturn = new Table(1);
            adresseAndReturn.UseAllAvailableWidth();
            adresseAndReturn.SetBorder(border);
            var adresse = new Table(1);
            adresse.SetFontSize(8);
            adresse.UseAllAvailableWidth();
            adresse.SetBorder(border);

            adresse.AddTextCell(CreateCell(invoice.BUYER.Buyer_trading_name));
            adresse.AddTextCell(CreateCell(invoice.BUYER.Buyer_name));
            adresse.AddTextCell(CreateCell(invoice.BUYER.BUYER_POSTAL_ADDRESS.Buyer_address_line_1));
            adresse.AddTextCell(CreateCell(invoice.BUYER.BUYER_POSTAL_ADDRESS.Buyer_address_line_2));
            adresse.AddTextCell(CreateCell(invoice.BUYER.BUYER_POSTAL_ADDRESS.Buyer_address_line_3));
            adresse.AddTextCell(CreateCellM(new text[] {invoice.BUYER.BUYER_POSTAL_ADDRESS.Buyer_post_code,
                                                    invoice.BUYER.BUYER_POSTAL_ADDRESS.Buyer_city}, " "));


            adresseAndReturn.AddCell(ReturnAdress(invoice));

            var c = new Cell();
            c.SetBorder(border);
            c.Add(adresse);
            adresseAndReturn.AddCell(c);

            return adresseAndReturn;
        }

        private static Cell ReturnAdress(invoice invoice)
        {
            var c = CreateCellM(new text[] {invoice.SELLER.Seller_trading_name,
                                            invoice.SELLER.Seller_name,
                                            invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_address_line_1,
                                            invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_address_line_2,
                                            invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_address_line_3,
                                            invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_post_code,
                                            invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_city}, " - ");

            c.SetUnderline();
            c.SetFontSize(6);
            c.SetBorder(border);
            return c;
        }

        internal static Table SellerAdress(invoice invoice)
        {
            var adresse = new Table(1);
            adresse.SetFontSize(8);
            adresse.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
            adresse.UseAllAvailableWidth();
            adresse.SetBorder(border);

            adresse.AddTextCell(CreateCell(invoice.SELLER.Seller_trading_name));
            adresse.AddTextCell(CreateCell(invoice.SELLER.Seller_name));
            adresse.AddTextCell(CreateCell(invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_address_line_1));
            adresse.AddTextCell(CreateCell(invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_address_line_2));
            adresse.AddTextCell(CreateCell(invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_address_line_3));
            adresse.AddTextCell(CreateCellM(new text[] {invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_post_code,
                                                    invoice.SELLER.SELLER_POSTAL_ADDRESS.Seller_city}, " "));

            return adresse;
        }

        internal static Table PayeeAdress(invoice invoice)
        {
            var adresse = new Table(1);
            adresse.SetFontSize(8);
            adresse.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
            adresse.UseAllAvailableWidth();
            adresse.SetBorder(border);

            adresse.AddTextCell(CreateCell(invoice.PAYEE.Payee_name));
            adresse.AddTextCell(CreateCell(invoice.PAYEE.Payee_identifier, ""));
            adresse.AddTextCell(CreateCell(invoice.PAYEE.Payee_legal_registration_identifier, ""));

            return adresse;
        }

        internal static Table ContactInfo(invoice invoice)
        {
            var contactInfo = new Table(1);
            contactInfo.SetFontSize(8);
            contactInfo.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
            contactInfo.UseAllAvailableWidth();
            contactInfo.SetBorder(border);

            contactInfo.AddTextCell(CreateCell(invoice.SELLER.SELLER_CONTACT.Seller_contact_point, "Kontakt: "));
            contactInfo.AddTextCell(CreateCell(invoice.SELLER.SELLER_CONTACT.Seller_contact_telephone_number, "Tel: "));
            contactInfo.AddTextCell(CreateCell(invoice.SELLER.SELLER_CONTACT.Seller_contact_email_address, "E-Mail: "));

            return contactInfo;
        }

        internal static Table PaymentInfo(invoice invoice)
        {
            var paymentInfo = new Table(1);
            paymentInfo.SetFontSize(8);
            paymentInfo.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
            paymentInfo.UseAllAvailableWidth();
            paymentInfo.SetBorder(border);

            paymentInfo.AddTextCell(CreateCell(invoice.PAYMENT_INSTRUCTIONS?.CREDIT_TRANSFER?.First()?.Payment_account_name, "Empfänger: "));
            paymentInfo.AddTextCell(CreateCell(invoice.PAYMENT_INSTRUCTIONS?.CREDIT_TRANSFER?.First()?.Payment_account_identifier, "IBAN: "));
            paymentInfo.AddTextCell(CreateCell(invoice.PAYMENT_INSTRUCTIONS?.CREDIT_TRANSFER?.First()?.Payment_service_provider_identifier, "BIC: "));
            paymentInfo.AddTextCell(CreateCell(invoice.SELLER_TAX_REPRESENTATIVE_PARTY?.Seller_tax_representative_VAT_identifier, "USt-ID: "));
            paymentInfo.AddTextCell(CreateCell(invoice.SELLER?.Seller_VAT_identifier, "USt-ID: "));

            return paymentInfo;
        }
        private static Table Header(invoice invoice)
        {
            var positions = new Table(1);
            positions.UseAllAvailableWidth();
            positions.SetBorder(border);

            positions.AddTextCell(CreateCell("Rechnung").SetFontSize(20).SetBold());
            positions.AddTextCell(CreateSpacing(5));
           
            positions.AddTextCell(CreateCell("Rechnung Nr. " + invoice.Invoice_number.Value ).SetFontSize(10)); //TODO Typisierung

            return positions;
        }

        private static Table Dates(invoice invoice)
        {
            var dates = new Table(1);
            dates.UseAllAvailableWidth();
            dates.SetFontSize(8);
            dates.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
            dates.SetBorder(border);

            dates.AddTextCell(CreateSpacing(5));
            dates.AddTextCell(CreateCell("Rechnungsdatum: " + invoice.Invoice_issue_date.Value)); //TODO Typisierung
            dates.AddTextCell(CreateCell(invoice.Buyer_reference, "Käufer-Referenz: " ));

            return dates;
        }

        private static Cell BeschreibungItem(ITEM_INFORMATIONType item)
        {

            var c = new Cell();
            c.SetBorder(border);
            var p1 = new Paragraph(item.Item_name.Value);
            var p2 = new Paragraph();

            if(item.Item_description != null)
            {
                p1.SetUnderline();
                p2.Add(item.Item_description.Value);
            }

            c.Add(p1);
            c.Add(p2);

            return c;
        }
        private static Table Positions(invoice invoice)
        {
            var positions = new Table(6);
            var posPadding = 20;
            positions.SetFontSize(10);
            positions.UseAllAvailableWidth();
            positions.SetBorder(border);
            positions.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);


            var hCell = new Cell();
            hCell.SetBorderRight(border);
            hCell.SetBold();
            hCell.SetFontSize(12);
            hCell.Add(new Paragraph("ID"));
            positions.AddHeaderCell(hCell);

            hCell = new Cell();
            hCell.SetBorderRight(border);
            hCell.SetBorderLeft(border);
            hCell.SetBold();
            hCell.SetFontSize(12);
            hCell.Add(new Paragraph("Beschreibung"));
            positions.AddHeaderCell(hCell);

            hCell = new Cell();
            hCell.SetBorderRight(border);
            hCell.SetBorderLeft(border);
            hCell.SetBold();
            hCell.SetFontSize(12);
            hCell.Add(new Paragraph("Menge"));
            positions.AddHeaderCell(hCell);

            hCell = new Cell();
            hCell.SetBorderRight(border);
            hCell.SetBorderLeft(border);
            hCell.SetBold();
            hCell.SetFontSize(12);
            hCell.Add(new Paragraph("Einheit"));
            positions.AddHeaderCell(hCell);

            hCell = new Cell();
            hCell.SetBorderRight(border);
            hCell.SetBorderLeft(border);
            hCell.SetBold();
            hCell.SetFontSize(12);
            hCell.Add(new Paragraph("Einzelpreis"));
            positions.AddHeaderCell(hCell);

            hCell = new Cell();
            hCell.SetBorderLeft(border);
            hCell.SetBold();
            hCell.SetFontSize(12);
            hCell.Add(new Paragraph("Gesamtpreis"));
            positions.AddHeaderCell(hCell);


            foreach (INVOICE_LINEType line in invoice.INVOICE_LINE)
            {
                posPadding--;

                var minheight = 0;
                var cell = new Cell();
                cell.SetMinHeight(minheight);
                cell.SetBorder(border);
                cell.Add(new Paragraph(line.Invoice_line_identifier.Value));
                cell.SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);
                cell.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
                cell.SetMarginTop(3);
                positions.AddCell(cell);

                positions.AddCell(BeschreibungItem(line.ITEM_INFORMATION).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT).SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE).SetMarginTop(3));

                cell = new Cell();
                cell.SetMinHeight(minheight);
                cell.SetBorder(border);
                cell.Add(new Paragraph(line.Invoiced_quantity.Value));
                cell.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                cell.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
                cell.SetMarginTop(3);
                positions.AddCell(cell);

                cell = new Cell();
                cell.SetMinHeight(minheight);
                cell.SetBorder(border);
                cell.Add(new Paragraph("Stück"));
                cell.SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);
                cell.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
                cell.SetMarginTop(3);
                positions.AddCell(cell);

                cell = new Cell();
                cell.SetMinHeight(minheight);
                cell.SetBorder(border);
                cell.Add(new Paragraph(line.PRICE_DETAILS.Item_net_price.Value + " €"));
                cell.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                cell.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
                cell.SetMarginTop(3);
                positions.AddCell(cell);

                cell = new Cell();
                cell.SetMinHeight(minheight);
                cell.SetBorder(border);
                cell.Add(new Paragraph(line.Invoice_line_net_amount.Value + " €"));
                cell.SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
                cell.SetVerticalAlignment(iText.Layout.Properties.VerticalAlignment.MIDDLE);
                cell.SetMarginTop(3);
                positions.AddCell(cell);
            }

            //for(int i=0; i<posPadding; i++)
            //{
            //    var minheight = 0;
            //    var cell = new Cell();
            //    cell.SetMinHeight(minheight);
            //    cell.SetBorder(border);

            //    cell.SetMarginTop(3);
            //    positions.AddCell(cell);
            //}

            return positions;
        }


        private static Table Footer(invoice invoice)
        {
            var footer = new Table(4);
            footer.UseAllAvailableWidth();
            footer.SetFontSize(5);
            footer.SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT);
            footer.SetBorder(border);

            var cell = new Cell(1, 4);
            cell.Add(new LineSeparator(new SolidLine(1)).SetMarginTop(10).SetMarginBottom(20));
            footer.AddCell(cell);


            footer.AddCell(SellerAdress(invoice).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));
            footer.AddCell(ContactInfo(invoice).SetTextAlignment(iText.Layout.Properties.TextAlignment.LEFT));

            return footer;
        }

        public static void CreatePDF(invoice invoice)
        {
            var dest = @"C:\Test\Rechnung.pdf";
            var writer = new PdfWriter(dest);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf);
            document.SetFontSize(10);
            document.SetBottomMargin(margin + footerHeight);
            pdf.AddEventHandler(PdfDocumentEvent.END_PAGE, new TextFooterEventHandler(document, invoice));
            

            var adressen = new Table(2);
            adressen.UseAllAvailableWidth();
            adressen.SetBorder(border);

            adressen.AddTable(BuyerAdress(invoice));

            adressen.AddTable(SellerAdress(invoice));

            var blank = new Cell();
            blank.SetBorder(border);
            adressen.AddCell(blank);

            adressen.AddTable(ContactInfo(invoice));

            adressen.AddTable(Header(invoice));

            adressen.AddTable(Dates(invoice));

            document.Add(adressen);

            document.Add(new LineSeparator(new SolidLine(1)).SetMarginTop(10).SetMarginBottom(20));

            document.Add(Positions(invoice).SetBorder(border));

            try { document.Add(Totals(invoice)); }
            catch(Exception e) { }

            document.Close();
            Process.Start(dest);
        }

    }




    static class TableAddedMethods
    {
        private static Border border = Border.NO_BORDER;

        public static Table AddTextCell(this Table t, Cell c)
        {
            if (c == null)
                return t;

            return t.AddCell(c);
        }

        public static Table AddTable(this Table t, Table table)
        {
            var c = new Cell();
            c.Add(table);
            c.SetBorder(border);

            return t.AddCell(c);
        }
    }

    class TextFooterEventHandler : IEventHandler
    {


        protected Document doc;
        protected invoice invoice;

        public TextFooterEventHandler(Document doc, invoice invoice)
        {
            this.doc = doc;
            this.invoice = invoice;
        }
        public void HandleEvent(Event @event)
        {
            var docEvent = (PdfDocumentEvent)@event;
            var canvas = new PdfCanvas(docEvent.GetPage());
            var pageSize = docEvent.GetPage().GetPageSize();
            canvas.BeginText();
            try
            {
                canvas.SetFontAndSize(PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA), 7);
            }
            catch (IOException e)
            {
                //e.printStackTrace();
            }


            var adresse = PDFCreator.SellerAdress(invoice);
            if (invoice.PAYEE != null)
                adresse = PDFCreator.PayeeAdress(invoice);
            var paymentInfo = PDFCreator.PaymentInfo(invoice);
            var contactInfo = PDFCreator.ContactInfo(invoice);

            PrintListToCanvas(canvas, adresse, doc.GetLeftMargin(), pageSize.GetBottom() + doc.GetBottomMargin() -30 );
            PrintListToCanvas(canvas, paymentInfo, 200, 0);
            PrintListToCanvas(canvas, contactInfo, 200, 0);


            canvas.EndText().Release();

            //   var c = new Canvas(canvas, docEvent.GetDocument(), pageSize);
        }

        private static void PrintListToCanvas(PdfCanvas canvas, Table list, double x, double y)
        {
            canvas = canvas.MoveText(x, y);
            int abstand = 10;
            var text = new List<string>();

            foreach (Cell c in list.GetChildren())
            {
                text.Add(((Text)((Paragraph)c.GetChildren().First()).GetChildren().First()).GetText());
            }


            foreach (string s in text)
            {
                canvas = canvas.ShowText(s);
                canvas = canvas.MoveText(0, -abstand);
            }

            canvas.MoveText(0, text.Count() * abstand);

        }

    }
}

