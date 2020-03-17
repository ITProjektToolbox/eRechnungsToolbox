using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace XRechnungs_Drucker
{
    class UBLMapping
    {
        internal static InvoiceType CreateInvoice(Dictionary<string, string> fields, List<Dictionary<string, string>> lineFields, List<Dictionary<string, string>> vatBreakdown)
        {
            var invoice = new InvoiceType();
            string s;

            if (fields.TryGetValue("BT-1", out s))
            {
                var temp = new IDType();
                temp.Value = s;
                invoice.ID = temp;
            }

            if (fields.TryGetValue("BT-2", out s))
            {
                if (DateTime.TryParse(s, out DateTime d))
                {
                    var temp = new IssueDateType();
                    temp.Value = d;
                    invoice.IssueDate = temp;
                }
                else
                {
                    //ERRORHANDLING
                }
            }

            if (fields.TryGetValue("BT-3", out s))
            {
                var temp = new InvoiceTypeCodeType();
                temp.Value = s;
                invoice.InvoiceTypeCode = temp;
            }

            //if (fields.TryGetValue("BT-4", out s))
            //{
            //    var temp = new InvoiceTypeCodeType();
            //    temp.Value = s;
            //    invoice.InvoiceTypeCode = temp;
            //}

            if (fields.TryGetValue("BT-5", out s))
            {
                var temp = new DocumentCurrencyCodeType();
                temp.Value = s;
                invoice.DocumentCurrencyCode = temp;
            }

            if (fields.TryGetValue("BT-6", out s))
            {
                var temp = new TaxCurrencyCodeType();
                temp.Value = s;
                invoice.TaxCurrencyCode = temp;
            }

            if (fields.TryGetValue("BT-7", out s))
            {
                if (DateTime.TryParse(s, out DateTime d))
                {
                    var temp = new TaxPointDateType();
                    temp.Value = d;
                    invoice.TaxPointDate = temp;
                }
                else
                {
                    //ERRORHANDLING
                }
            }

            if (fields.TryGetValue("BT-8", out s))
            {
                var temp = new PeriodType[] { new PeriodType() };
                temp[0].DescriptionCode = new DescriptionCodeType[] { new DescriptionCodeType() };
                temp[0].DescriptionCode[0].Value = s;
                invoice.InvoicePeriod = temp;
            }

            if (fields.TryGetValue("BT-9", out s))
            {
                if (DateTime.TryParse(s, out DateTime d))
                {
                    var temp = new DueDateType();
                    temp.Value = d;
                    invoice.DueDate = temp;
                }
                else
                {
                    //ERRORHANDLING
                }
            }

            if (fields.TryGetValue("BT-10", out s))
            {
                var temp = new BuyerReferenceType();
                temp.Value = s;
                invoice.BuyerReference = temp;
            }

            if (fields.TryGetValue("BT-11", out s))
            {
                var temp = new ProjectReferenceType[] { new ProjectReferenceType() };
                temp[0].ID = new IDType();
                temp[0].ID.Value = s;
                invoice.ProjectReference = temp;
            }

            if (fields.TryGetValue("BT-12", out s))
            {
                var temp = new DocumentReferenceType[] { new DocumentReferenceType() };// unsicher
                temp[0].ID = new IDType();
                temp[0].ID.Value = s;
                invoice.ContractDocumentReference = temp;
            }
            if (fields.TryGetValue("BT-13", out s))
            {
                var temp = new OrderReferenceType();
                temp.ID = new IDType();
                temp.ID.Value = s;
                invoice.OrderReference = temp;
            }
            if (fields.TryGetValue("BT-14", out s))
            {
                var temp = new OrderReferenceType();
                temp.SalesOrderID = new SalesOrderIDType();
                temp.SalesOrderID.Value = s;
                invoice.OrderReference.SalesOrderID = temp.SalesOrderID;
            }
            if (fields.TryGetValue("BT-15", out s))
            {
                var temp = new DocumentReferenceType[] { new DocumentReferenceType() };// unsicher
                temp[0].ID = new IDType();
                temp[0].ID.Value = s;
                invoice.ReceiptDocumentReference = temp;
            }
            if (fields.TryGetValue("BT-16", out s))
            {
                var temp = new DocumentReferenceType[] { new DocumentReferenceType() };
                temp[0].ID = new IDType();
                temp[0].ID.Value = s;
                invoice.DespatchDocumentReference = temp;
            }
            if (fields.TryGetValue("BT-17", out s))
            {
                var temp = new DocumentReferenceType[] { new DocumentReferenceType() };
                temp[0].ID = new IDType();
                temp[0].ID.Value = s;
                invoice.OriginatorDocumentReference = temp;
            }
            if (fields.TryGetValue("BT-18", out s))
            {
                var temp = new DocumentReferenceType[] { new DocumentReferenceType() };
                temp[0].ID = new IDType();
                temp[0].ID.Value = s;
                invoice.AdditionalDocumentReference = temp;
            }
            if (fields.TryGetValue("BT-18-1", out s))
            {
                var temp = new DocumentReferenceType[] { new DocumentReferenceType() };
                temp[0].ID.schemeID = s;
                invoice.AdditionalDocumentReference = temp;
            }
            if (fields.TryGetValue("BT-19", out s))
            {
                var temp = new AccountingCostType();
                temp.Value = s;
                invoice.AccountingCost = temp;
            }
            if (fields.TryGetValue("BT-20", out s))
            {
                var temp = new PaymentTermsType[] { new PaymentTermsType() };
                temp[0].Note = new NoteType[] { new NoteType() };
                temp[0].Note[0].Value = s;
                invoice.PaymentTerms = temp;
            }
            if (fields.TryGetValue("BT-23", out s))//BG_2
            {
                var temp = new ProfileIDType();
                temp.Value = s;
                invoice.ProfileID = temp;
            }
            if (fields.TryGetValue("BT-24", out s))//BG_2
            {
                var temp = new CustomizationIDType();
                temp.Value = s;
                invoice.CustomizationID = temp;
            }






            invoice.Note = BG_1(ref fields);
            invoice.BillingReference = BG_3(ref fields);

            invoice.AccountingSupplierParty = BG_4to6(ref fields);

            invoice.AccountingCustomerParty = BG_7to9(ref fields);

            invoice.PayeeParty = BG_10(ref fields);

            invoice.TaxRepresentativeParty = BG_11to12(ref fields);

            invoice.Delivery = BG_13and15(ref fields);

            invoice.InvoicePeriod = BG_14(ref fields);

            invoice.PaymentMeans = new PaymentMeansType[] { BG_16to19(ref fields) };


            var allowances = BG_20(ref fields);
            var charges = BG_21(ref fields);

            if (allowances != null && charges != null)
                invoice.AllowanceCharge = new AllowanceChargeType[] { allowances, charges };

            else if (allowances != null)
                invoice.AllowanceCharge = new AllowanceChargeType[] { allowances };

            else if (charges != null)
                invoice.AllowanceCharge = new AllowanceChargeType[] { charges };



            invoice.LegalMonetaryTotal = BG_22(ref fields);

            invoice.TaxTotal = new TaxTotalType[] { BG_22_Part2(ref fields) };

            if(invoice.TaxTotal != null)
            {   
                if(vatBreakdown.Count == 1)
                {
                    invoice.TaxTotal[0].TaxSubtotal = new TaxSubtotalType[] { BG_23(vatBreakdown[0]) };
                }
                if (vatBreakdown.Count == 2)
                {
                    invoice.TaxTotal[0].TaxSubtotal = new TaxSubtotalType[] { BG_23(vatBreakdown[0]), BG_23(vatBreakdown[1]) };
                }

            }
                






            List<InvoiceLineType> lines = new List<InvoiceLineType>();

            foreach(var line in lineFields)
            {
                lines.Add(BG_25(line));
            }

            invoice.InvoiceLine = lines.ToArray();



            


            return invoice;
        }

        private static NoteType[] BG_1(ref Dictionary<string, string> fields)
        {//unsicher
            string s;
            bool empty = true;
            var inNote = new NoteType[] { new NoteType() };


            if (fields.TryGetValue("BT-21", out s) && s != null)
            {
                inNote[0].Value = "#" + s + "#";
                empty = false;
            }
            if (fields.TryGetValue("BT-22", out s) && s != null)
            {
                inNote[0].Value = inNote[0].Value + s;
                empty = false;
            }

            if (empty)
                return null;

            return inNote;
        }

        private static BillingReferenceType[] BG_3(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var billReference = new BillingReferenceType[] { new BillingReferenceType() };
            billReference[0].InvoiceDocumentReference = new DocumentReferenceType();

            if (fields.TryGetValue("BT-25", out s) && s != null)
            {
                billReference[0].InvoiceDocumentReference.ID = new IDType();
                billReference[0].InvoiceDocumentReference.ID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-26", out s) && s != null)
            {
                if (DateTime.TryParse(s, out DateTime d))
                {
                    billReference[0].InvoiceDocumentReference.IssueDate = new IssueDateType();
                    billReference[0].InvoiceDocumentReference.IssueDate.Value = d;
                    empty = false;
                }
                else
                {
                    //ERRORHANDLING
                }

            }
            if (empty)
            {
                return null;
            }
            return billReference;
        }
        private static SupplierPartyType BG_4to6(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var suParty = new SupplierPartyType();
            suParty.Party = new PartyType();

            if (fields.TryGetValue("BT-27", out s) && s != null)
            {
                suParty.Party.PartyLegalEntity = suParty.Party.PartyLegalEntity ?? new PartyLegalEntityType[] { new PartyLegalEntityType() };
                suParty.Party.PartyLegalEntity[0].RegistrationName = suParty.Party.PartyLegalEntity[0].RegistrationName ?? new RegistrationNameType();
                suParty.Party.PartyLegalEntity[0].RegistrationName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-28", out s) && s != null)
            {
                suParty.Party.PartyName = suParty.Party.PartyName ?? new PartyNameType[] { new PartyNameType() };
                suParty.Party.PartyName[0].Name = suParty.Party.PartyName[0].Name ?? new NameType1();
                suParty.Party.PartyName[0].Name.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-29", out s) && s != null)
            {
                suParty.Party.PartyIdentification = suParty.Party.PartyIdentification ?? new PartyIdentificationType[] { new PartyIdentificationType() };
                suParty.Party.PartyIdentification[0].ID = suParty.Party.PartyIdentification[0].ID ?? new IDType();
                suParty.Party.PartyIdentification[0].ID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-30", out s) && s != null)
            {
                suParty.Party.PartyLegalEntity = suParty.Party.PartyLegalEntity ?? new PartyLegalEntityType[] { new PartyLegalEntityType() };
                suParty.Party.PartyLegalEntity[0].CompanyID = suParty.Party.PartyLegalEntity[0].CompanyID ?? new CompanyIDType();
                suParty.Party.PartyLegalEntity[0].CompanyID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-31", out s) && s != null)
            {
                suParty.Party.PartyTaxScheme = suParty.Party.PartyTaxScheme ?? new PartyTaxSchemeType[] { new PartyTaxSchemeType() };
                suParty.Party.PartyTaxScheme[0].CompanyID = suParty.Party.PartyTaxScheme[0].CompanyID ?? new CompanyIDType();
                suParty.Party.PartyTaxScheme[0].CompanyID.Value = s;

                suParty.Party.PartyTaxScheme[0].TaxScheme = suParty.Party.PartyTaxScheme[0].TaxScheme ?? new TaxSchemeType();
                suParty.Party.PartyTaxScheme[0].TaxScheme.ID = suParty.Party.PartyTaxScheme[0].TaxScheme.ID ?? new IDType();
                suParty.Party.PartyTaxScheme[0].TaxScheme.ID.Value = "VAT";
                empty = false;
            }


            if (fields.TryGetValue("BT-32", out s) && s != null)
            {
                suParty.Party.PartyTaxScheme = suParty.Party.PartyTaxScheme ?? new PartyTaxSchemeType[] { new PartyTaxSchemeType() };
                suParty.Party.PartyTaxScheme[0].CompanyID = suParty.Party.PartyLegalEntity[0].CompanyID ?? new CompanyIDType();
                suParty.Party.PartyTaxScheme[0].CompanyID.Value = s;
                empty = false;

                //suParty.Party.PartyTaxScheme[0].TaxScheme = suParty.Party.PartyTaxScheme[0].TaxScheme ?? new TaxSchemeType();
                //suParty.Party.PartyTaxScheme[0].TaxScheme.ID = suParty.Party.PartyTaxScheme[0].TaxScheme.ID ?? new IDType();
                //suParty.Party.PartyTaxScheme[0].TaxScheme.ID.Value = "???";
            }

            if (fields.TryGetValue("BT-33", out s) && s != null)
            {
                suParty.Party.PartyLegalEntity = suParty.Party.PartyLegalEntity ?? new PartyLegalEntityType[] { new PartyLegalEntityType() };
                suParty.Party.PartyLegalEntity[0].CompanyLegalForm = suParty.Party.PartyLegalEntity[0].CompanyLegalForm ?? new CompanyLegalFormType();
                suParty.Party.PartyLegalEntity[0].CompanyLegalForm.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-34", out s) && s != null)
            {
                suParty.Party.EndpointID = suParty.Party.EndpointID ?? new EndpointIDType();
                suParty.Party.EndpointID.Value = s;
                empty = false;
            }

            //aus BT-19 (bisher Behelfslösung)
            if(fields.TryGetValue("BT-80", out s) && s != null)
            {

                suParty.Party.PartyIdentification = suParty.Party.PartyIdentification ?? new PartyIdentificationType[] { new PartyIdentificationType() };
                suParty.Party.PartyIdentification[0].ID = suParty.Party.PartyIdentification[0].ID ?? new IDType();
                suParty.Party.PartyIdentification[0].ID.Value = s;
                suParty.Party.PartyIdentification[0].ID.schemeID = "SEPA";
                empty = false;
            }

            suParty.Party.PostalAddress = BG_5(ref fields);
            suParty.Party.Contact = BG_6(ref fields);



            if (empty && suParty.Party.PostalAddress == null && suParty.Party.Contact == null)
                return null;


            return suParty;
        }

        private static AddressType BG_5(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var postalAddress = new AddressType();
            postalAddress.Country = new CountryType();
            if (fields.TryGetValue("BT-35", out s) && s != null)
            {
                postalAddress.StreetName = postalAddress.StreetName ?? new StreetNameType();
                postalAddress.StreetName.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-36", out s) && s != null)
            {
                postalAddress.AdditionalStreetName = postalAddress.AdditionalStreetName ?? new AdditionalStreetNameType();
                postalAddress.AdditionalStreetName.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-37", out s) && s != null)
            {
                postalAddress.CityName = postalAddress.CityName ?? new CityNameType();
                postalAddress.CityName.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-38", out s) && s != null)
            {
                postalAddress.PostalZone = postalAddress.PostalZone ?? new PostalZoneType();
                postalAddress.PostalZone.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-39", out s) && s != null)
            {
                postalAddress.CountrySubentity = postalAddress.CountrySubentity ?? new CountrySubentityType();
                postalAddress.CountrySubentity.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-40", out s) && s != null)
            {
                postalAddress.Country.IdentificationCode = postalAddress.Country.IdentificationCode ?? new IdentificationCodeType();
                postalAddress.Country.IdentificationCode.Value = s;
                empty = false;
            }
            if (empty) return null;
            return postalAddress;
        }
        private static ContactType BG_6(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var sellerContact = new ContactType();

            if (fields.TryGetValue("BT-41", out s) && s != null)
            {
                sellerContact.Name = sellerContact.Name ?? new NameType1();
                sellerContact.Name.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-42", out s) && s != null)
            {
                sellerContact.Telephone = sellerContact.Telephone ?? new TelephoneType();
                sellerContact.Telephone.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-43", out s) && s != null)
            {
                sellerContact.ElectronicMail = sellerContact.ElectronicMail ?? new ElectronicMailType();
                sellerContact.ElectronicMail.Value = s;
                empty = false;
            }
            if (empty) return null;
            return sellerContact;
        }
        private static CustomerPartyType BG_7to9(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var cuParty = new CustomerPartyType();
            cuParty.Party = new PartyType();

            if (fields.TryGetValue("BT-44", out s) && s != null)
            {
                cuParty.Party.PartyLegalEntity = cuParty.Party.PartyLegalEntity ?? new PartyLegalEntityType[] { new PartyLegalEntityType() };
                cuParty.Party.PartyLegalEntity[0].RegistrationName = cuParty.Party.PartyLegalEntity[0].RegistrationName ?? new RegistrationNameType();
                cuParty.Party.PartyLegalEntity[0].RegistrationName.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-45", out s) && s != null)
            {
                cuParty.Party.PartyName = cuParty.Party.PartyName ?? new PartyNameType[] { new PartyNameType() };
                cuParty.Party.PartyName[0].Name = cuParty.Party.PartyName[0].Name ?? new NameType1();
                cuParty.Party.PartyName[0].Name.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-46", out s) && s != null)
            {
                cuParty.Party.PartyIdentification = cuParty.Party.PartyIdentification ?? new PartyIdentificationType[] { new PartyIdentificationType() };
                cuParty.Party.PartyIdentification[0].ID = cuParty.Party.PartyIdentification[0].ID ?? new IDType();
                cuParty.Party.PartyIdentification[0].ID.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-46-1", out s) && s != null)
            {
                cuParty.Party.PartyIdentification = cuParty.Party.PartyIdentification ?? new PartyIdentificationType[] { new PartyIdentificationType() };
                cuParty.Party.PartyIdentification[0].ID = cuParty.Party.PartyIdentification[0].ID ?? new IDType();
                cuParty.Party.PartyIdentification[0].ID.schemeID = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-47", out s) && s != null)
            {
                cuParty.Party.PartyLegalEntity = cuParty.Party.PartyLegalEntity ?? new PartyLegalEntityType[] { new PartyLegalEntityType() };
                cuParty.Party.PartyLegalEntity[0].CompanyID = cuParty.Party.PartyLegalEntity[0].CompanyID ?? new CompanyIDType();
                cuParty.Party.PartyLegalEntity[0].CompanyID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-48", out s) && s != null)
            {
                cuParty.Party.PartyTaxScheme = cuParty.Party.PartyTaxScheme ?? new PartyTaxSchemeType[] { new PartyTaxSchemeType() };
                cuParty.Party.PartyTaxScheme[0].CompanyID = cuParty.Party.PartyTaxScheme[0].CompanyID ?? new CompanyIDType();
                cuParty.Party.PartyTaxScheme[0].CompanyID.Value = s;

                cuParty.Party.PartyTaxScheme[0].TaxScheme = cuParty.Party.PartyTaxScheme[0].TaxScheme ?? new TaxSchemeType();
                cuParty.Party.PartyTaxScheme[0].TaxScheme.ID = cuParty.Party.PartyTaxScheme[0].TaxScheme.ID ?? new IDType();
                cuParty.Party.PartyTaxScheme[0].TaxScheme.ID.Value = "VAT";
                empty = false;
            }

            if (fields.TryGetValue("BT-49", out s) && s != null)
            {
                cuParty.Party.EndpointID = cuParty.Party.EndpointID ?? new EndpointIDType();
                cuParty.Party.EndpointID.Value = s;
                empty = false;
            }
            if (fields.TryGetValue("BT-49-1", out s) && s != null)
            {
                cuParty.Party.EndpointID = cuParty.Party.EndpointID ?? new EndpointIDType();
                cuParty.Party.EndpointID.schemeID = s;
                empty = false;
            }

            cuParty.Party.PostalAddress = BG_8(ref fields);
            cuParty.Party.Contact = BG_9(ref fields);

            if (cuParty.Party.PostalAddress == null && cuParty.Party.Contact == null && empty)
                return null;

            return cuParty;

        }
        //private static SupplierPartyType BG_4(ref Dictionary<string, string> fields)
        //{
        //    string s;
        //    bool empty = true;
        //    var suParty = new SupplierPartyType();
        //    suParty.Party = new PartyType();

        //    if (fields.TryGetValue("BT-27", out s) && s != null)
        //    {
        //        suParty.Party.PartyLegalEntity = suParty.Party.PartyLegalEntity ?? new PartyLegalEntityType[] { new PartyLegalEntityType() };
        //        suParty.Party.PartyLegalEntity[0].RegistrationName = suParty.Party.PartyLegalEntity[0].RegistrationName ?? new RegistrationNameType();
        //        suParty.Party.PartyLegalEntity[0].RegistrationName.Value = s;
        //        empty = false;
        //    }

        //    if (fields.TryGetValue("BT-28", out s) && s != null)
        //    {
        //        suParty.Party.PartyName = suParty.Party.PartyName ?? new PartyNameType[] { new PartyNameType() };
        //        suParty.Party.PartyName[0].Name = suParty.Party.PartyName[0].Name ?? new NameType1();
        //        suParty.Party.PartyName[0].Name.Value = s;
        //        empty = false;
        //    }

        //    if (fields.TryGetValue("BT-29", out s) && s != null)
        //    {
        //        suParty.Party.PartyIdentification = suParty.Party.PartyIdentification ?? new PartyIdentificationType[] { new PartyIdentificationType() };
        //        suParty.Party.PartyIdentification[0].ID = suParty.Party.PartyIdentification[0].ID ?? new IDType();
        //        suParty.Party.PartyIdentification[0].ID.Value = s;
        //        empty = false;
        //    }

        //    if (fields.TryGetValue("BT-30", out s) && s != null)
        //    {
        //        suParty.Party.PartyLegalEntity = suParty.Party.PartyLegalEntity ?? new PartyLegalEntityType[] { new PartyLegalEntityType() };
        //        suParty.Party.PartyLegalEntity[0].CompanyID = suParty.Party.PartyLegalEntity[0].CompanyID ?? new CompanyIDType();
        //        suParty.Party.PartyLegalEntity[0].CompanyID.Value = s;
        //        empty = false;
        //    }

        //    if (fields.TryGetValue("BT-31", out s) && s != null)
        //    {
        //        suParty.Party.PartyTaxScheme = suParty.Party.PartyTaxScheme ?? new PartyTaxSchemeType[] { new PartyTaxSchemeType() };
        //        suParty.Party.PartyTaxScheme[0].CompanyID = suParty.Party.PartyTaxScheme[0].CompanyID ?? new CompanyIDType();
        //        suParty.Party.PartyTaxScheme[0].CompanyID.Value = s;

        //        suParty.Party.PartyTaxScheme[0].TaxScheme = suParty.Party.PartyTaxScheme[0].TaxScheme ?? new TaxSchemeType();
        //        suParty.Party.PartyTaxScheme[0].TaxScheme.ID = suParty.Party.PartyTaxScheme[0].TaxScheme.ID ?? new IDType();
        //        suParty.Party.PartyTaxScheme[0].TaxScheme.ID.Value = "VAT";
        //        empty = false;
        //    }


        //    if (fields.TryGetValue("BT-32", out s) && s != null)
        //    {
        //        suParty.Party.PartyTaxScheme = suParty.Party.PartyTaxScheme ?? new PartyTaxSchemeType[] { new PartyTaxSchemeType() };
        //        suParty.Party.PartyTaxScheme[0].CompanyID = suParty.Party.PartyLegalEntity[0].CompanyID ?? new CompanyIDType();
        //        suParty.Party.PartyTaxScheme[0].CompanyID.Value = s;
        //        empty = false;

        //    }

        //    if (fields.TryGetValue("BT-33", out s) && s != null)
        //    {
        //        suParty.Party.PartyLegalEntity = suParty.Party.PartyLegalEntity ?? new PartyLegalEntityType[] { new PartyLegalEntityType() };
        //        suParty.Party.PartyLegalEntity[0].CompanyLegalForm = suParty.Party.PartyLegalEntity[0].CompanyLegalForm ?? new CompanyLegalFormType();
        //        suParty.Party.PartyLegalEntity[0].CompanyLegalForm.Value = s;
        //        empty = false;
        //    }

        //    if (fields.TryGetValue("BT-34", out s) && s != null)
        //    {
        //        suParty.Party.EndpointID = suParty.Party.EndpointID ?? new EndpointIDType();
        //        suParty.Party.EndpointID.Value = s;
        //        empty = false;
        //    }

        //    if (empty)
        //        return null;

        //    return suParty;
        //}

        private static AddressType BG_8(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var cuParty = new CustomerPartyType();
            cuParty.Party = new PartyType();
            cuParty.Party.PostalAddress = new AddressType();

            if (fields.TryGetValue("BT-50", out s) && s != null)
            {
                cuParty.Party.PostalAddress.StreetName = cuParty.Party.PostalAddress.StreetName ?? new StreetNameType();
                cuParty.Party.PostalAddress.StreetName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-51", out s) && s != null)
            {
                cuParty.Party.PostalAddress.AdditionalStreetName = cuParty.Party.PostalAddress.AdditionalStreetName ?? new AdditionalStreetNameType();
                cuParty.Party.PostalAddress.AdditionalStreetName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-163", out s) && s != null)
            {
                cuParty.Party.PostalAddress.AddressLine = cuParty.Party.PostalAddress.AddressLine ?? new AddressLineType[] { new AddressLineType() };
                cuParty.Party.PostalAddress.AddressLine[0].Line = cuParty.Party.PostalAddress.AddressLine[0].Line ?? new LineType();
                cuParty.Party.PostalAddress.AddressLine[0].Line.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-52", out s) && s != null)
            {
                cuParty.Party.PostalAddress.CityName = cuParty.Party.PostalAddress.CityName ?? new CityNameType();
                cuParty.Party.PostalAddress.CityName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-53", out s) && s != null)
            {
                cuParty.Party.PostalAddress.PostalZone = cuParty.Party.PostalAddress.PostalZone ?? new PostalZoneType();
                cuParty.Party.PostalAddress.PostalZone.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-54", out s) && s != null)
            {
                cuParty.Party.PostalAddress.CountrySubentity = cuParty.Party.PostalAddress.CountrySubentity ?? new CountrySubentityType();
                cuParty.Party.PostalAddress.CountrySubentity.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-55", out s) && s != null)
            {
                cuParty.Party.PostalAddress.Country = cuParty.Party.PostalAddress.Country ?? new CountryType();
                cuParty.Party.PostalAddress.Country.IdentificationCode = cuParty.Party.PostalAddress.Country.IdentificationCode ?? new IdentificationCodeType();
                cuParty.Party.PostalAddress.Country.IdentificationCode.Value = s;
                empty = false;
            }

            if (empty)
                return null;

            return cuParty.Party.PostalAddress;
        }

        private static ContactType BG_9(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var cuParty = new CustomerPartyType();
            cuParty.Party = new PartyType();
            cuParty.Party.Contact = new ContactType();

            if (fields.TryGetValue("BT-56", out s) && s != null)
            {
                cuParty.Party.Contact.Name = cuParty.Party.Contact.Name ?? new NameType1();
                cuParty.Party.Contact.Name.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-57", out s) && s != null)
            {
                cuParty.Party.Contact.Telephone = cuParty.Party.Contact.Telephone ?? new TelephoneType();
                cuParty.Party.Contact.Telephone.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-58", out s) && s != null)
            {
                cuParty.Party.Contact.ElectronicMail = cuParty.Party.Contact.ElectronicMail ?? new ElectronicMailType();
                cuParty.Party.Contact.ElectronicMail.Value = s;
                empty = false;
            }

            if (empty)
                return null;

            return cuParty.Party.Contact;
        }

        private static PartyType BG_10(ref Dictionary<string, string> fields)
        {
            string s;          
            bool empty = true;
            var paParty = new PartyType();

            if (fields.TryGetValue("BT-59", out s) && s != null)
            {
                paParty.PartyName = paParty.PartyName ?? new PartyNameType[] { new PartyNameType() };
                paParty.PartyName[0].Name = paParty.PartyName[0].Name ?? new NameType1();
                paParty.PartyName[0].Name.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-60", out s) && s != null)
            {
                paParty.PartyIdentification = paParty.PartyIdentification ?? new PartyIdentificationType[] { new PartyIdentificationType() };
                paParty.PartyIdentification[0].ID = paParty.PartyIdentification[0].ID ?? new IDType();
                paParty.PartyIdentification[0].ID.Value = s;
                paParty.PartyIdentification[0].ID.schemeID = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-61", out s) && s != null)
            {
                paParty.PartyLegalEntity = paParty.PartyLegalEntity ?? new PartyLegalEntityType[] { new PartyLegalEntityType() };
                paParty.PartyLegalEntity[0].CompanyID = paParty.PartyLegalEntity[0].CompanyID ?? new CompanyIDType();
                paParty.PartyLegalEntity[0].CompanyID.Value = s;
                paParty.PartyLegalEntity[0].CompanyID.schemeID = s;
                empty = false;
            }

            if (empty)
                return null;

            return paParty;
        }

        private static PartyType BG_11to12(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var strParty = new PartyType();

            if (fields.TryGetValue("BT-62", out s) && s != null)
            {
                strParty.PartyName = strParty.PartyName ?? new PartyNameType[] { new PartyNameType() };
                strParty.PartyName[0].Name = strParty.PartyName[0].Name ?? new NameType1();
                strParty.PartyName[0].Name.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-63", out s) && s != null)
            {
                strParty.PartyTaxScheme = strParty.PartyTaxScheme ?? new PartyTaxSchemeType[] { new PartyTaxSchemeType() };
                strParty.PartyTaxScheme[0].CompanyID = strParty.PartyTaxScheme[0].CompanyID ?? new CompanyIDType();
                strParty.PartyTaxScheme[0].CompanyID.Value = s;

                strParty.PartyTaxScheme[0].TaxScheme = strParty.PartyTaxScheme[0].TaxScheme ?? new TaxSchemeType();
                strParty.PartyTaxScheme[0].TaxScheme.ID = strParty.PartyTaxScheme[0].TaxScheme.ID ?? new IDType();
                strParty.PartyTaxScheme[0].TaxScheme.ID.Value = s;
                empty = false;
            }

            strParty.PostalAddress = BG_12(ref fields);


            if (strParty.PostalAddress == null && empty)
                return null;


            return strParty;
        }

        private static AddressType BG_12(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var strParty = new PartyType();
            strParty.PostalAddress = new AddressType();

            if (fields.TryGetValue("BT-64", out s) && s != null)
            {
                strParty.PostalAddress.StreetName = strParty.PostalAddress.StreetName ?? new StreetNameType();
                strParty.PostalAddress.StreetName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-65", out s) && s != null)
            {
                strParty.PostalAddress.AdditionalStreetName = strParty.PostalAddress.AdditionalStreetName ?? new AdditionalStreetNameType();
                strParty.PostalAddress.AdditionalStreetName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-164", out s) && s != null)
            {
                strParty.PostalAddress.AddressLine = strParty.PostalAddress.AddressLine ?? new AddressLineType[] { new AddressLineType() };
                strParty.PostalAddress.AddressLine[0].Line = strParty.PostalAddress.AddressLine[0].Line ?? new LineType();
                strParty.PostalAddress.AddressLine[0].Line.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-66", out s) && s != null)
            {
                strParty.PostalAddress.CityName = strParty.PostalAddress.CityName ?? new CityNameType();
                strParty.PostalAddress.CityName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-67", out s) && s != null)
            {
                strParty.PostalAddress.PostalZone = strParty.PostalAddress.PostalZone ?? new PostalZoneType();
                strParty.PostalAddress.PostalZone.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-68", out s) && s != null)
            {
                strParty.PostalAddress.CountrySubentity = strParty.PostalAddress.CountrySubentity ?? new CountrySubentityType();
                strParty.PostalAddress.CountrySubentity.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-69", out s) && s != null)
            {
                strParty.PostalAddress.Country = strParty.PostalAddress.Country ?? new CountryType();
                strParty.PostalAddress.Country.IdentificationCode = strParty.PostalAddress.Country.IdentificationCode ?? new IdentificationCodeType();
                strParty.PostalAddress.Country.IdentificationCode.Value = s;
                empty = false;
            }

            if (empty)
                return null;

            return strParty.PostalAddress;
        }

        private static DeliveryType[] BG_13and15(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var delivery = new DeliveryType[] { new DeliveryType() };
            delivery[0].DeliveryLocation = new LocationType1();

            if (fields.TryGetValue("BT-70", out s) && s != null)
            {
                delivery[0].DeliveryParty = delivery[0].DeliveryParty ?? new PartyType();
                delivery[0].DeliveryParty.PartyName = delivery[0].DeliveryParty.PartyName ?? new PartyNameType[] { new PartyNameType() };
                delivery[0].DeliveryParty.PartyName[0].Name = delivery[0].DeliveryParty.PartyName[0].Name ?? new NameType1();
                delivery[0].DeliveryParty.PartyName[0].Name.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-71", out s) && s != null)
            {              
                delivery[0].DeliveryLocation.ID = delivery[0].DeliveryLocation.ID ?? new IDType();
                delivery[0].DeliveryLocation.ID.Value = s;
                delivery[0].DeliveryLocation.ID.schemeID = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-72", out s) && s != null)
            {
                if (DateTime.TryParse(s, out DateTime d))
                {
                    var temp = new ActualDeliveryDateType();
                    temp.Value = d;
                    delivery[0].ActualDeliveryDate = temp;
                    empty = false;
                }
                else
                {
                    //ERRORHANDLING
                }
            }


            delivery[0].DeliveryLocation.Address = BG_15(ref fields);

            if (delivery[0].DeliveryLocation.Address == null && empty)
                return null;

            return delivery;
        }

        private static PeriodType[] BG_14(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var invPeriod = new PeriodType[] { new PeriodType() };

            if (fields.TryGetValue("BT-73", out s) && s != null)
            {
                if (DateTime.TryParse(s, out DateTime d))
                {
                    var temp = new StartDateType();
                    temp.Value = d;
                    invPeriod[0].StartDate = temp;
                    empty = false;
                }
                else
                {
                    //ERRORHANDLING
                }
            }

            if (fields.TryGetValue("BT-74", out s) && s != null)
            {
                if (DateTime.TryParse(s, out DateTime d))
                {
                    var temp = new EndDateType();
                    temp.Value = d;
                    invPeriod[0].EndDate = temp;
                    empty = false;
                }
                else
                {
                    //ERRORHANDLING
                }
            }

            if (empty)
                return null;

            return invPeriod;
        }

        private static AddressType BG_15(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var delivery = new DeliveryType();
            delivery.DeliveryLocation = new LocationType1();
            delivery.DeliveryLocation.Address = new AddressType();

            if (fields.TryGetValue("BT-75", out s) && s != null)
            {
                delivery.DeliveryLocation.Address.StreetName = delivery.DeliveryLocation.Address.StreetName ?? new StreetNameType();
                delivery.DeliveryLocation.Address.StreetName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-76", out s) && s != null)
            {
                delivery.DeliveryLocation.Address.AdditionalStreetName = delivery.DeliveryLocation.Address.AdditionalStreetName ?? new AdditionalStreetNameType();
                delivery.DeliveryLocation.Address.AdditionalStreetName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-165", out s) && s != null)
            {
                delivery.DeliveryLocation.Address.AddressLine = delivery.DeliveryLocation.Address.AddressLine ?? new AddressLineType[] { new AddressLineType() };
                delivery.DeliveryLocation.Address.AddressLine[0].Line = delivery.DeliveryLocation.Address.AddressLine[0].Line ?? new LineType();
                delivery.DeliveryLocation.Address.AddressLine[0].Line.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-77", out s) && s != null)
            {
                delivery.DeliveryLocation.Address.CityName = delivery.DeliveryLocation.Address.CityName ?? new CityNameType();
                delivery.DeliveryLocation.Address.CityName.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-78", out s) && s != null)
            {
                delivery.DeliveryLocation.Address.PostalZone = delivery.DeliveryLocation.Address.PostalZone ?? new PostalZoneType();
                delivery.DeliveryLocation.Address.PostalZone.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-79", out s) && s != null)
            {
                delivery.DeliveryLocation.Address.CountrySubentity = delivery.DeliveryLocation.Address.CountrySubentity ?? new CountrySubentityType();
                delivery.DeliveryLocation.Address.CountrySubentity.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-80", out s) && s != null)
            {
                delivery.DeliveryLocation.Address.Country = delivery.DeliveryLocation.Address.Country ?? new CountryType();
                delivery.DeliveryLocation.Address.Country.IdentificationCode = delivery.DeliveryLocation.Address.Country.IdentificationCode ?? new IdentificationCodeType();
                delivery.DeliveryLocation.Address.Country.IdentificationCode.Value = s;
                empty = false;
            }

            if (empty)
                return null;

            return delivery.DeliveryLocation.Address;
        }

        private static PaymentMeansType BG_16to19(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var paymentMeans = new PaymentMeansType();

            if (fields.TryGetValue("BT-81", out s) && s != null)
            {
                paymentMeans.PaymentMeansCode = paymentMeans.PaymentMeansCode ?? new PaymentMeansCodeType();
                paymentMeans.PaymentMeansCode.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-82", out s) && s != null)
            {
                paymentMeans.PaymentMeansCode = paymentMeans.PaymentMeansCode ?? new PaymentMeansCodeType();
                paymentMeans.PaymentMeansCode.name = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-83", out s) && s != null)
            {
                paymentMeans.PaymentID = paymentMeans.PaymentID ?? new PaymentIDType[] { new PaymentIDType() };
                paymentMeans.PaymentID[0].Value = s;
                empty = false;
            }

            //BG-18
            if (fields.TryGetValue("BT-87", out s) && s != null)
            {
                paymentMeans.CardAccount.PrimaryAccountNumberID = paymentMeans.CardAccount.PrimaryAccountNumberID ?? new PrimaryAccountNumberIDType();
                paymentMeans.CardAccount.PrimaryAccountNumberID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-88", out s) && s != null)
            {
                paymentMeans.CardAccount.HolderName = paymentMeans.CardAccount.HolderName ?? new HolderNameType();
                paymentMeans.CardAccount.HolderName.Value = s;
                empty = false;
            }

            // Einfügen der Untergruppen
            paymentMeans.PayeeFinancialAccount = BG_17(ref fields);
            paymentMeans.PaymentMandate = BG_19(ref fields);

            if (empty && paymentMeans.PayeeFinancialAccount == null && paymentMeans.PaymentMandate == null)
                return null;



            return paymentMeans;
        }

        private static FinancialAccountType BG_17(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var paymentMeans = new PaymentMeansType();
            paymentMeans.PayeeFinancialAccount = new FinancialAccountType();

            if (fields.TryGetValue("BT-84", out s) && s != null)
            {
                paymentMeans.PayeeFinancialAccount.ID = paymentMeans.PayeeFinancialAccount.ID ?? new IDType();
                paymentMeans.PayeeFinancialAccount.ID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-85", out s) && s != null)
            {
                paymentMeans.PayeeFinancialAccount.Name = paymentMeans.PayeeFinancialAccount.Name ?? new NameType1();
                paymentMeans.PayeeFinancialAccount.Name.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-86", out s) && s != null)
            {
                paymentMeans.PayeeFinancialAccount.FinancialInstitutionBranch = paymentMeans.PayeeFinancialAccount.FinancialInstitutionBranch ?? new BranchType();
                paymentMeans.PayeeFinancialAccount.FinancialInstitutionBranch.ID = paymentMeans.PayeeFinancialAccount.FinancialInstitutionBranch.ID ?? new IDType();
                paymentMeans.PayeeFinancialAccount.FinancialInstitutionBranch.ID.Value = s;
                empty = false;
            }

            if (empty)
                return null;

            return paymentMeans.PayeeFinancialAccount;
        }

        private static PaymentMeansType BG_18(ref Dictionary<string, string> fields)
        {

            string s;
            bool empty = true;
            var paymentMeans = new PaymentMeansType();
            paymentMeans.CardAccount = new CardAccountType();



            if (empty)
                return null;

            return paymentMeans;

        }

        private static PaymentMandateType BG_19(ref Dictionary<string, string> fields)
        {

            string s;
            bool empty = true;
            var paymentMandate = new PaymentMandateType();

            if (fields.TryGetValue("BT-89", out s) && s != null)
            {

                paymentMandate.ID = paymentMandate.ID ?? new IDType();
                paymentMandate.ID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-91", out s) && s != null)
            {

                paymentMandate.PayerFinancialAccount = paymentMandate.PayerFinancialAccount ?? new FinancialAccountType();
                paymentMandate.PayerFinancialAccount.ID = paymentMandate.PayerFinancialAccount.ID ?? new IDType();
                paymentMandate.PayerFinancialAccount.ID.Value = s;
                empty = false;
            }


            if (empty)
                return null;

            return paymentMandate;
        }

        private static AllowanceChargeType BG_20(ref Dictionary<string, string> fields)
        {

            string s;
            bool empty = true;
            var allowanceCharge = new AllowanceChargeType();
            allowanceCharge.ChargeIndicator = new ChargeIndicatorType();
            allowanceCharge.ChargeIndicator.Value = false;

            if (fields.TryGetValue("BT-92", out s) && s != null)
            {

                allowanceCharge.Amount = allowanceCharge.Amount ?? new AmountType2();
                allowanceCharge.Amount.Value = Decimal.Parse(s);
                allowanceCharge.Amount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-93", out s) && s != null)
            {

                allowanceCharge.BaseAmount = allowanceCharge.BaseAmount ?? new BaseAmountType();
                allowanceCharge.BaseAmount.Value = Decimal.Parse(s);
                allowanceCharge.BaseAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-94", out s) && s != null)
            {

                allowanceCharge.MultiplierFactorNumeric = allowanceCharge.MultiplierFactorNumeric ?? new MultiplierFactorNumericType();
                allowanceCharge.MultiplierFactorNumeric.Value = Decimal.Parse(s);
                empty = false;

            }

            if (fields.TryGetValue("BT-95", out s) && s != null)
            {

                allowanceCharge.TaxCategory = allowanceCharge.TaxCategory ?? new TaxCategoryType[] { new TaxCategoryType() };
                allowanceCharge.TaxCategory[0].ID = allowanceCharge.TaxCategory[0].ID ?? new IDType();
                allowanceCharge.TaxCategory[0].ID.Value = s;

                allowanceCharge.TaxCategory[0].TaxScheme = allowanceCharge.TaxCategory[0].TaxScheme ?? new TaxSchemeType();
                allowanceCharge.TaxCategory[0].TaxScheme.ID = allowanceCharge.TaxCategory[0].TaxScheme.ID ?? new IDType();
                allowanceCharge.TaxCategory[0].TaxScheme.ID.Value = "VAT";
                empty = false;

            }

            if (fields.TryGetValue("BT-96", out s) && s != null)
            {

                allowanceCharge.TaxCategory = allowanceCharge.TaxCategory ?? new TaxCategoryType[] { new TaxCategoryType() };
                allowanceCharge.TaxCategory[0].Percent = allowanceCharge.TaxCategory[0].Percent ?? new PercentType1();
                allowanceCharge.TaxCategory[0].Percent.Value = Decimal.Parse(s);
                empty = false;

            }

            if (fields.TryGetValue("BT-97", out s) && s != null)
            {

                allowanceCharge.AllowanceChargeReason = allowanceCharge.AllowanceChargeReason ?? new AllowanceChargeReasonType[] { new AllowanceChargeReasonType() };
                allowanceCharge.AllowanceChargeReason[0].Value = s;
                empty = false;

            }

            if (fields.TryGetValue("BT-98", out s) && s != null)
            {

                allowanceCharge.AllowanceChargeReasonCode = allowanceCharge.AllowanceChargeReasonCode ?? new AllowanceChargeReasonCodeType();
                allowanceCharge.AllowanceChargeReasonCode.Value = s;
                empty = false;

            }

            if (empty)
                return null;

            return allowanceCharge;
        }

        private static AllowanceChargeType BG_21(ref Dictionary<string, string> fields)
        {

            string s;
            bool empty = true;
            var allowanceCharge = new AllowanceChargeType();
            allowanceCharge.ChargeIndicator = new ChargeIndicatorType();
            allowanceCharge.ChargeIndicator.Value = true;

            if (fields.TryGetValue("BT-99", out s) && s != null)
            {

                allowanceCharge.Amount = allowanceCharge.Amount ?? new AmountType2();
                allowanceCharge.Amount.Value = Decimal.Parse(s);
                allowanceCharge.Amount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-100", out s) && s != null)
            {

                allowanceCharge.BaseAmount = allowanceCharge.BaseAmount ?? new BaseAmountType();
                allowanceCharge.BaseAmount.Value = Decimal.Parse(s);
                allowanceCharge.BaseAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-101", out s) && s != null)
            {

                allowanceCharge.MultiplierFactorNumeric = allowanceCharge.MultiplierFactorNumeric ?? new MultiplierFactorNumericType();
                allowanceCharge.MultiplierFactorNumeric.Value = Decimal.Parse(s);
                empty = false;

            }

            if (fields.TryGetValue("BT-102", out s) && s != null)
            {

                allowanceCharge.TaxCategory = allowanceCharge.TaxCategory ?? new TaxCategoryType[] { new TaxCategoryType() };
                allowanceCharge.TaxCategory[0].ID = allowanceCharge.TaxCategory[0].ID ?? new IDType();
                allowanceCharge.TaxCategory[0].ID.Value = s;
            }

            if (fields.TryGetValue("BT-103", out s) && s != null)
            {
                allowanceCharge.TaxCategory = allowanceCharge.TaxCategory ?? new TaxCategoryType[] { new TaxCategoryType() };
                allowanceCharge.TaxCategory[0].Percent = allowanceCharge.TaxCategory[0].Percent ?? new PercentType1();
                allowanceCharge.TaxCategory[0].Percent.Value = Decimal.Parse(s);
                empty = false;
            }

            if (fields.TryGetValue("BT-104", out s) && s != null)
            {

                allowanceCharge.AllowanceChargeReason = allowanceCharge.AllowanceChargeReason ?? new AllowanceChargeReasonType[] { new AllowanceChargeReasonType() };
                allowanceCharge.AllowanceChargeReason[0].Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-105", out s) && s != null)
            {

                allowanceCharge.AllowanceChargeReasonCode = allowanceCharge.AllowanceChargeReasonCode ?? new AllowanceChargeReasonCodeType();
                allowanceCharge.AllowanceChargeReasonCode.Value = s;
                empty = false;
            }

            if (empty)
                return null;

            return allowanceCharge;

        }

        private static MonetaryTotalType BG_22(ref Dictionary<string, string> fields)
        {

            string s;
            bool empty = true;
            var monetaryTotals = new MonetaryTotalType();

            if (fields.TryGetValue("BT-106", out s) && s != null)
            {

                monetaryTotals.LineExtensionAmount = monetaryTotals.LineExtensionAmount ?? new LineExtensionAmountType();
                monetaryTotals.LineExtensionAmount.Value = Decimal.Parse(s);
                monetaryTotals.LineExtensionAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-107", out s) && s != null)
            {

                monetaryTotals.AllowanceTotalAmount = monetaryTotals.AllowanceTotalAmount ?? new AllowanceTotalAmountType();
                monetaryTotals.AllowanceTotalAmount.Value = Decimal.Parse(s);
                monetaryTotals.AllowanceTotalAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-108", out s) && s != null)
            {

                monetaryTotals.ChargeTotalAmount = monetaryTotals.ChargeTotalAmount ?? new ChargeTotalAmountType();
                monetaryTotals.ChargeTotalAmount.Value = Decimal.Parse(s);
                monetaryTotals.ChargeTotalAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-109", out s) && s != null)
            {

                monetaryTotals.TaxExclusiveAmount = monetaryTotals.TaxExclusiveAmount ?? new TaxExclusiveAmountType();
                monetaryTotals.TaxExclusiveAmount.Value = Decimal.Parse(s);
                monetaryTotals.TaxExclusiveAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-112", out s) && s != null)
            {

                monetaryTotals.TaxInclusiveAmount = monetaryTotals.TaxInclusiveAmount ?? new TaxInclusiveAmountType();
                monetaryTotals.TaxInclusiveAmount.Value = Decimal.Parse(s);
                monetaryTotals.TaxInclusiveAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-113", out s) && s != null)
            {

                monetaryTotals.PrepaidAmount = monetaryTotals.PrepaidAmount ?? new PrepaidAmountType();
                monetaryTotals.PrepaidAmount.Value = Decimal.Parse(s);
                monetaryTotals.PrepaidAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-114", out s) && s != null)
            {

                monetaryTotals.PayableRoundingAmount = monetaryTotals.PayableRoundingAmount ?? new PayableRoundingAmountType();
                monetaryTotals.PayableRoundingAmount.Value = Decimal.Parse(s);
                monetaryTotals.PayableRoundingAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-115", out s) && s != null)
            {

                monetaryTotals.PayableAmount = monetaryTotals.PayableAmount ?? new PayableAmountType();
                monetaryTotals.PayableAmount.Value = Decimal.Parse(s);
                monetaryTotals.PayableAmount.currencyID = "EUR";
                empty = false;
            }

            if (empty)
                return null;

            return monetaryTotals;

        }

        private static TaxTotalType BG_22_Part2(ref Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var taxTotal = new TaxTotalType();

            if (fields.TryGetValue("BT-110", out s) && s != null)
            {

                taxTotal.TaxAmount = taxTotal.TaxAmount ?? new TaxAmountType();
                taxTotal.TaxAmount.Value = Decimal.Parse(s);
                taxTotal.TaxAmount.currencyID = "EUR";
                empty = false;
            }

            //if (fields.TryGetValue("BT-111", out s) && s != null)
            //{
            //    invoice.TaxTotal = invoice.TaxTotal ?? new TaxTotalType[](new taxTotal());
            //    taxTotal.TaxAmount = taxTotal.TaxAmount ?? new TaxAmountType[] { TaxExclusiveAmountType() };
            //    taxTotal.TaxAmount.Value = s;
            //    empty = false;
            //}

            if (empty)
                return null;

            return taxTotal;
        }

        private static TaxSubtotalType BG_23(Dictionary<string, string> fields)
        {

            string s;
            bool empty = true;
            var taxSubTotal = new TaxSubtotalType();

            if (fields.TryGetValue("BT-116", out s) && s != null)
            {

                taxSubTotal.TaxableAmount = taxSubTotal.TaxableAmount ?? new TaxableAmountType();
                taxSubTotal.TaxableAmount.Value = Decimal.Parse(s);
                taxSubTotal.TaxableAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-117", out s) && s != null)
            {

                taxSubTotal.TaxAmount = taxSubTotal.TaxAmount ?? new TaxAmountType();
                taxSubTotal.TaxAmount.Value = Decimal.Parse(s);
                taxSubTotal.TaxAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-118", out s) && s != null)
            {

                taxSubTotal.TaxCategory = taxSubTotal.TaxCategory ?? new TaxCategoryType();
                taxSubTotal.TaxCategory.ID = taxSubTotal.TaxCategory.ID ?? new IDType();
                taxSubTotal.TaxCategory.ID.Value = s;

                taxSubTotal.TaxCategory.TaxScheme = taxSubTotal.TaxCategory.TaxScheme ?? new TaxSchemeType();
                taxSubTotal.TaxCategory.TaxScheme.ID = taxSubTotal.TaxCategory.TaxScheme.ID ?? new IDType();
                taxSubTotal.TaxCategory.TaxScheme.ID.Value = "VAT";
                empty = false;
            }

            if (fields.TryGetValue("BT-119", out s) && s != null)
            {

                taxSubTotal.TaxCategory = taxSubTotal.TaxCategory ?? new TaxCategoryType();
                taxSubTotal.TaxCategory.Percent = taxSubTotal.TaxCategory.Percent ?? new PercentType1();
                taxSubTotal.TaxCategory.Percent.Value = Decimal.Parse(s);
                empty = false;
            }

            if (fields.TryGetValue("BT-120", out s) && s != null)
            {

                taxSubTotal.TaxCategory = taxSubTotal.TaxCategory ?? new TaxCategoryType();
                taxSubTotal.TaxCategory.TaxExemptionReason = taxSubTotal.TaxCategory.TaxExemptionReason ?? new TaxExemptionReasonType[] { new TaxExemptionReasonType() };
                taxSubTotal.TaxCategory.TaxExemptionReason[0].Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-121", out s) && s != null)
            {

                taxSubTotal.TaxCategory = taxSubTotal.TaxCategory ?? new TaxCategoryType();
                taxSubTotal.TaxCategory.TaxExemptionReason = taxSubTotal.TaxCategory.TaxExemptionReason ?? new TaxExemptionReasonType[] { new TaxExemptionReasonType() };
                taxSubTotal.TaxCategory.TaxExemptionReason[0].Value = s;
                empty = false;
            }

            if (empty)
                return null;

            return taxSubTotal;

        }

        //private static AllowanceChargeType BG_24(ref Dictionary<string, string> fields)
        //{

        //    string s;
        //    bool empty = true;
        //    var additionalDocumentReference = new taxTotal();
        //    additionalDocumentReference.Attachment = AttachmentType();

        //    if (fields.TryGetValue("BT-122", out s) && s != null)
        //    {

        //        additionalDocumentReference.ID = additionalDocumentReference.ID ?? new IDType[] { IDType() };
        //        additionalDocumentReference.ID.Value = s;
        //        empty = false;
        //    }

        //    if (fields.TryGetValue("BT-123", out s) && s != null)
        //    {

        //        additionalDocumentReference.DocumentDescription = additionalDocumentReference.DocumentDescription ?? new DocumentDescriptionType[] { DocumentDescriptionType() };
        //        additionalDocumentReference.DocumentDescription.Value = s;
        //        empty = false;
        //    }

        //    if (fields.TryGetValue("BT-124", out s) && s != null)
        //    {

        //        additionalDocumentReference.Attachment.ExternalReference = additionalDocumentReference.Attachment.ExternalReference ?? new ExternalReferenceType[] { ExternalReferenceType() };
        //        additionalDocumentReference.Attachment.ExternalReference[0].URI = additionalDocumentReference.Attachment.ExternalReference[0].URI ?? new URIType();
        //        additionalDocumentReference.Attachment.ExternalReference[0].URI.Value = s;
        //        empty = false;
        //    }

        //    if (empty)
        //        return null;

        //    return additionalDocumentReference;

        //}



        private static InvoiceLineType BG_25(Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var inLine = new InvoiceLineType();


            if (fields.TryGetValue("BT-126", out s) && s != null)
            {
                inLine.ID = inLine.ID ?? new IDType();
                inLine.ID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-127", out s) && s != null)
            {
                inLine.Note = inLine.Note ?? new NoteType[] { new NoteType() };
                inLine.Note[0].Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-128", out s) && s != null)
            {
                // schemeID ; with DocumentTypeCode = 130
                inLine.DocumentReference = inLine.DocumentReference ?? new DocumentReferenceType[] { new DocumentReferenceType() };
                inLine.DocumentReference[0].ID = inLine.DocumentReference[0].ID ?? new IDType();
                inLine.DocumentReference[0].ID.Value = s;
                empty = false;

                inLine.DocumentReference[0].DocumentTypeCode.Value = "130";
            }

            if (fields.TryGetValue("BT-129", out s) && s != null)
            {
                inLine.InvoicedQuantity = inLine.InvoicedQuantity ?? new InvoicedQuantityType();
                inLine.InvoicedQuantity.Value = Decimal.Parse(s);
                empty = false;
            }

            if (fields.TryGetValue("BT-130", out s) && s != null)
            {
                // unitcode
                inLine.InvoicedQuantity = inLine.InvoicedQuantity ?? new InvoicedQuantityType();
                inLine.InvoicedQuantity.unitCode = s;
                empty = false;

            }

            if (fields.TryGetValue("BT-131", out s) && s != null)
            {
                //f
                inLine.LineExtensionAmount = inLine.LineExtensionAmount ?? new LineExtensionAmountType();
                inLine.LineExtensionAmount.Value = Decimal.Parse(s.Remove(s.Length-1));
                inLine.LineExtensionAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-132", out s) && s != null)
            {
                //f
                inLine.OrderLineReference = inLine.OrderLineReference ?? new OrderLineReferenceType[] { new OrderLineReferenceType() };
                inLine.OrderLineReference[0].LineID = inLine.OrderLineReference[0].LineID ?? new LineIDType();
                inLine.OrderLineReference[0].LineID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-133", out s) && s != null)
            {
                //f
                inLine.AccountingCost = inLine.AccountingCost ?? new AccountingCostType();
                inLine.AccountingCost.Value = s;
                empty = false;
            }

            inLine.InvoicePeriod = BG_26(fields);

            var allowances = BG_27(fields);
            var charges = BG_28(fields);

            if (allowances != null && charges != null)
                inLine.AllowanceCharge = new AllowanceChargeType[] { allowances, charges };

            else if (allowances != null)
                inLine.AllowanceCharge = new AllowanceChargeType[] { allowances };

            else if (charges != null)
                inLine.AllowanceCharge = new AllowanceChargeType[] { charges };

            inLine.Price = BG_29(fields);
            inLine.Item = BG_30to32(fields);
            
            
            if (inLine.InvoicePeriod == null && inLine.AllowanceCharge == null && inLine.Price == null && inLine.Item == null && empty)
                return null;

            return inLine;
        }

        private static PeriodType[] BG_26(Dictionary<string, string> fields)
        {
            string s;
            bool empty = true;
            var inLine = new InvoiceLineType();


            if (fields.TryGetValue("BT-134", out s) && s != null)
            {
                if (DateTime.TryParse(s, out DateTime d))
                {
                    inLine.InvoicePeriod = inLine.InvoicePeriod ?? new PeriodType[] { new PeriodType() };
                    var temp = new StartDateType();
                    temp.Value = d;
                    inLine.InvoicePeriod[0].StartDate = temp;
                    empty = false;
                }
                else
                {
                    //ERRORHANDLING
                }


            }

            if (fields.TryGetValue("BT-135", out s) && s != null)
            {

                if (DateTime.TryParse(s, out DateTime d))
                {
                    inLine.InvoicePeriod = inLine.InvoicePeriod ?? new PeriodType[] { new PeriodType() };
                    var temp = new EndDateType();
                    temp.Value = d;
                    inLine.InvoicePeriod[0].EndDate = temp;
                    empty = false;
                }
                else
                {
                    //ERRORHANDLING
                }

            }

            if (empty)
                return null;

            return inLine.InvoicePeriod;

        }

        private static AllowanceChargeType BG_27(Dictionary<string, string> fields)
        {
            string s;
            var inLine = new InvoiceLineType();
            bool empty = true;
            inLine.AllowanceCharge = new AllowanceChargeType[] { new AllowanceChargeType() };
            inLine.AllowanceCharge[0].ChargeIndicator = new ChargeIndicatorType();
            inLine.AllowanceCharge[0].ChargeIndicator.Value = false;


            if (fields.TryGetValue("BT-136", out s) && s != null)
            {
                inLine.AllowanceCharge = inLine.AllowanceCharge ?? new AllowanceChargeType[] { new AllowanceChargeType() };
                inLine.AllowanceCharge[0].Amount = inLine.AllowanceCharge[0].Amount ?? new AmountType2();
                inLine.AllowanceCharge[0].Amount.Value = Decimal.Parse(s);
                inLine.AllowanceCharge[0].Amount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-137", out s) && s != null)
            {
                inLine.AllowanceCharge = inLine.AllowanceCharge ?? new AllowanceChargeType[] { new AllowanceChargeType() };
                inLine.AllowanceCharge[0].BaseAmount = inLine.AllowanceCharge[0].BaseAmount ?? new BaseAmountType();
                inLine.AllowanceCharge[0].BaseAmount.Value = Decimal.Parse(s);
                inLine.AllowanceCharge[0].BaseAmount.currencyID = "EUR";
                empty = false;

            }

            if (fields.TryGetValue("BT-138", out s) && s != null)
            {
                inLine.AllowanceCharge = inLine.AllowanceCharge ?? new AllowanceChargeType[] { new AllowanceChargeType() };
                inLine.AllowanceCharge[0].MultiplierFactorNumeric = inLine.AllowanceCharge[0].MultiplierFactorNumeric ?? new MultiplierFactorNumericType();
                inLine.AllowanceCharge[0].MultiplierFactorNumeric.Value = decimal.Parse(s);
                empty = false;
            }

            if (fields.TryGetValue("BT-139", out s) && s != null)
            {
                inLine.AllowanceCharge = inLine.AllowanceCharge ?? new AllowanceChargeType[] { new AllowanceChargeType() };
                inLine.AllowanceCharge[0].AllowanceChargeReason = inLine.AllowanceCharge[0].AllowanceChargeReason ?? new AllowanceChargeReasonType[] { new AllowanceChargeReasonType() };
                inLine.AllowanceCharge[0].AllowanceChargeReason[0].Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-140", out s) && s != null)
            {
                inLine.AllowanceCharge = inLine.AllowanceCharge ?? new AllowanceChargeType[] { new AllowanceChargeType() };
                inLine.AllowanceCharge[0].AllowanceChargeReasonCode = inLine.AllowanceCharge[0].AllowanceChargeReasonCode ?? new AllowanceChargeReasonCodeType();
                inLine.AllowanceCharge[0].AllowanceChargeReasonCode.Value = s;
                empty = false;

            }

            if (empty)
                return null;
            return inLine.AllowanceCharge[0];

        }

        private static AllowanceChargeType BG_28(Dictionary<string, string> fields)
        {
            String s;
            var inLine = new InvoiceLineType();
            bool empty = true;
            inLine.AllowanceCharge = new AllowanceChargeType[] { new AllowanceChargeType() };
            inLine.AllowanceCharge[0].ChargeIndicator = new ChargeIndicatorType();
            inLine.AllowanceCharge[0].ChargeIndicator.Value = true;

            if (fields.TryGetValue("BT-141", out s) && s != null)
            {
                inLine.AllowanceCharge[0].Amount = inLine.AllowanceCharge[0].Amount ?? new AmountType2();
                inLine.AllowanceCharge[0].Amount.Value = decimal.Parse(s);
                inLine.AllowanceCharge[0].Amount.currencyID = "EUR";
                empty = false;

            }

            if (fields.TryGetValue("BT-142", out s) && s != null)
            {
                inLine.AllowanceCharge[0].BaseAmount = inLine.AllowanceCharge[0].BaseAmount ?? new BaseAmountType();
                inLine.AllowanceCharge[0].BaseAmount.Value = decimal.Parse(s);
                inLine.AllowanceCharge[0].BaseAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-143", out s) && s != null)
            {
                inLine.AllowanceCharge[0].MultiplierFactorNumeric = inLine.AllowanceCharge[0].MultiplierFactorNumeric ?? new MultiplierFactorNumericType();
                inLine.AllowanceCharge[0].MultiplierFactorNumeric.Value = decimal.Parse(s);
                empty = false;
            }

            if (fields.TryGetValue("BT-144", out s) && s != null)
            {
                inLine.AllowanceCharge[0].AllowanceChargeReason = inLine.AllowanceCharge[0].AllowanceChargeReason ?? new AllowanceChargeReasonType[] { new AllowanceChargeReasonType() };
                inLine.AllowanceCharge[0].AllowanceChargeReason[0].Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-145", out s) && s != null)
            {
                inLine.AllowanceCharge[0].AllowanceChargeReasonCode = inLine.AllowanceCharge[0].AllowanceChargeReasonCode ?? new AllowanceChargeReasonCodeType();
                inLine.AllowanceCharge[0].AllowanceChargeReasonCode.Value = s;
                empty = false;
            }

            if (empty)
                return null;
            return inLine.AllowanceCharge[0];

        }

        private static PriceType BG_29(Dictionary<string, string> fields)
        {
            string s;
            var inLine = new InvoiceLineType();
            inLine.Price = new PriceType();
            bool empty = true;

            if (fields.TryGetValue("BT-146", out s))
            {
                inLine.Price.PriceAmount = inLine.Price.PriceAmount ?? new PriceAmountType();
                inLine.Price.PriceAmount.Value = decimal.Parse(s.Remove(s.Length - 1));
                inLine.Price.PriceAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-147", out s) && s != null)
            {
                inLine.Price.AllowanceCharge = inLine.Price.AllowanceCharge ?? new AllowanceChargeType[] { new AllowanceChargeType() };
                inLine.Price.AllowanceCharge[0].ChargeIndicator.Value = false;
                inLine.Price.AllowanceCharge[0].Amount = inLine.Price.AllowanceCharge[0].Amount ?? new AmountType2();
                inLine.Price.AllowanceCharge[0].Amount.Value = Decimal.Parse(s);
                inLine.Price.AllowanceCharge[0].Amount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-148", out s) && s != null)
            {
                inLine.Price.AllowanceCharge = inLine.Price.AllowanceCharge ?? new AllowanceChargeType[] { new AllowanceChargeType() };
                inLine.Price.AllowanceCharge[0].ChargeIndicator.Value = false;
                inLine.Price.AllowanceCharge[0].BaseAmount = inLine.Price.AllowanceCharge[0].BaseAmount ?? new BaseAmountType();
                inLine.Price.AllowanceCharge[0].BaseAmount.Value = Decimal.Parse(s);
                inLine.Price.AllowanceCharge[0].BaseAmount.currencyID = "EUR";
                empty = false;
            }

            if (fields.TryGetValue("BT-149", out s) && s != null)
            {
                inLine.Price.BaseQuantity = inLine.Price.BaseQuantity ?? new BaseQuantityType();
                inLine.Price.BaseQuantity.Value = decimal.Parse(s);
                empty = false;
            }

            if (fields.TryGetValue("BT-150", out s) && s != null)
            {
                //@unitcode
                inLine.Price.AllowanceCharge = inLine.Price.AllowanceCharge ?? new AllowanceChargeType[] { new AllowanceChargeType() };
                inLine.Price.AllowanceCharge[0].BaseAmount = inLine.Price.AllowanceCharge[0].BaseAmount ?? new BaseAmountType();
                inLine.Price.AllowanceCharge[0].BaseAmount.Value = Decimal.Parse(s);
                inLine.Price.AllowanceCharge[0].BaseAmount.currencyID = "EUR";
                empty = false;
            }

            if (empty)
                return null;
            return inLine.Price;
        }

        private static ItemType BG_30to32(Dictionary<string, string> fields)
        {
            String s;
            var inLine = new InvoiceLineType();
            inLine.Item = new ItemType();
            bool empty = true;

            if (fields.TryGetValue("BT-151", out s) && s != null)
            {
                inLine.Item.ClassifiedTaxCategory = inLine.Item.ClassifiedTaxCategory ?? new TaxCategoryType[] { new TaxCategoryType() };
                inLine.Item.ClassifiedTaxCategory[0].ID = inLine.Item.ClassifiedTaxCategory[0].ID ?? new IDType();
                inLine.Item.ClassifiedTaxCategory[0].ID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-152", out s) && s != null)
            {
                inLine.Item.ClassifiedTaxCategory = inLine.Item.ClassifiedTaxCategory ?? new TaxCategoryType[] { new TaxCategoryType() };
                inLine.Item.ClassifiedTaxCategory[0].Percent = inLine.Item.ClassifiedTaxCategory[0].Percent ?? new PercentType1();
                inLine.Item.ClassifiedTaxCategory[0].Percent.Value = Decimal.Parse(s.Remove(s.Length - 1));

                inLine.Item.ClassifiedTaxCategory[0].TaxScheme = inLine.Item.ClassifiedTaxCategory[0].TaxScheme ?? new TaxSchemeType();
                inLine.Item.ClassifiedTaxCategory[0].TaxScheme.ID = inLine.Item.ClassifiedTaxCategory[0].TaxScheme.ID ?? new IDType();
                inLine.Item.ClassifiedTaxCategory[0].TaxScheme.ID.Value = "VAT";
                empty = false;
            }


            if (fields.TryGetValue("BT-153", out s) && s != null)
            {
                inLine.Item.Name = inLine.Item.Name ?? new NameType1();
                inLine.Item.Name.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-154", out s) && s != null)
            {
                inLine.Item.Description = inLine.Item.Description ?? new DescriptionType[] { new DescriptionType() };
                inLine.Item.Description[0].Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-155", out s) && s != null)
            {
                inLine.Item.SellersItemIdentification = inLine.Item.SellersItemIdentification ?? new ItemIdentificationType();
                inLine.Item.SellersItemIdentification.ID = inLine.Item.SellersItemIdentification.ID ?? new IDType();
                inLine.Item.SellersItemIdentification.ID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-156", out s) && s != null)
            {
                inLine.Item.BuyersItemIdentification = inLine.Item.BuyersItemIdentification ?? new ItemIdentificationType();
                inLine.Item.BuyersItemIdentification.ID = inLine.Item.BuyersItemIdentification.ID ?? new IDType();
                inLine.Item.BuyersItemIdentification.ID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-157", out s) && s != null)
            {
                // schemeID
                inLine.Item.StandardItemIdentification = inLine.Item.StandardItemIdentification ?? new ItemIdentificationType();
                inLine.Item.StandardItemIdentification.ID = inLine.Item.StandardItemIdentification.ID ?? new IDType();
                inLine.Item.StandardItemIdentification.ID.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-158", out s) && s != null)
            {
                //listID ; listVersionID
                inLine.Item.CommodityClassification = inLine.Item.CommodityClassification ?? new CommodityClassificationType[] { new CommodityClassificationType() };
                inLine.Item.CommodityClassification[0].ItemClassificationCode = inLine.Item.CommodityClassification[0].ItemClassificationCode ?? new ItemClassificationCodeType();
                inLine.Item.CommodityClassification[0].ItemClassificationCode.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-159", out s) && s != null)
            {
                //f
                inLine.Item.OriginCountry = inLine.Item.OriginCountry ?? new CountryType();
                inLine.Item.OriginCountry.IdentificationCode = inLine.Item.OriginCountry.IdentificationCode ?? new IdentificationCodeType();
                inLine.Item.OriginCountry.IdentificationCode.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-160", out s) && s != null)
            {
                inLine.Item.AdditionalItemProperty = inLine.Item.AdditionalItemProperty ?? new ItemPropertyType[] { new ItemPropertyType() };
                inLine.Item.AdditionalItemProperty[0].Name = inLine.Item.AdditionalItemProperty[0].Name ?? new NameType1();
                inLine.Item.AdditionalItemProperty[0].Name.Value = s;
                empty = false;
            }

            if (fields.TryGetValue("BT-161", out s) && s != null)
            {
                inLine.Item.AdditionalItemProperty = inLine.Item.AdditionalItemProperty ?? new ItemPropertyType[] { new ItemPropertyType() };
                inLine.Item.AdditionalItemProperty[0].Value = inLine.Item.AdditionalItemProperty[0].Value ?? new ValueType();
                inLine.Item.AdditionalItemProperty[0].Value.Value = s;
                empty = false;
            }

            if (empty)
                return null;
            return inLine.Item;

        }


        private static bool CheckIfEmpty<T>(T obj)
        {
            foreach (FieldInfo FI in obj.GetType().GetFields())
            {
                if (FI.GetValue(obj) != null)
                    return false;
            }
            return true;
        }
    }
}
