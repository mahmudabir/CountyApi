using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CountyApi;
class PdfToXmlConverter
{
    public string ExtractTextFromPdf(string pdfFilePath)
    {
        StringBuilder text = new StringBuilder();
        using (PdfReader reader = new PdfReader(pdfFilePath))
        {
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
            }
        }
        return text.ToString();
    }

    public string SerializeToXml<T>(T document)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
        using (StringWriter stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, document);
            return stringWriter.ToString();
        }
    }

    public bool ValidateXml(string xml)
    {
        XmlSchemaSet schema = new XmlSchemaSet();
        schema.Add("", @"C:\Users\acer\Downloads\PRIA_REQUEST_v2_4_2\PRIA_DOCUMENT_v2_4_1.XSD");
        schema.Add("", @"C:\Users\acer\Downloads\PRIA_REQUEST_v2_4_2\MISMO_SIGNATURE_Type.xsd");
        schema.Add("http://www.w3.org/2000/09/xmldsig#", @"C:\Users\acer\Downloads\PRIA_REQUEST_v2_4_2\SMART_DOCUMENT_xmldsig_core_schema_V_1_02.xsd");

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.ValidationType = ValidationType.Schema;
        settings.Schemas = schema;

        bool isValid = true;  // This will track if validation passed or failed

        // Capture schema validation errors
        settings.ValidationEventHandler += (sender, e) =>
        {
            isValid = false;  // Mark as invalid on the first error
            Console.WriteLine($"Validation Error: {e.Message}\n");
        };

        try
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(xml), settings))
            {
                while (reader.Read()) { }  // Read through the XML
            }
        }
        catch (XmlException ex)
        {
            Console.WriteLine($"XML Exception: {ex.Message}");
            return false;  // Invalid XML syntax
        }

        return isValid;  // Returns true if valid, false if validation failed
    }


    public string GenerateXmlFromPdf()
    {
        // Step 1: Extract text from PDF
        string pdfPath = "c:\\sample.pdf";
        string pdfText = ExtractTextFromPdf(pdfPath);

        File.WriteAllText("..\\..\\..\\pdfText.xml", pdfText);

        // Step 2: Populate PRIA_DOCUMENT (mapping the extracted data to the class)
        var priaDocument = GeneratePriaDocument();

        // Step 3: Serialize to XML
        string xmlString = SerializeToXml(priaDocument);

        // Step 4: Validate XML
        bool isValid = ValidateXml(xmlString);
        if (isValid)
        {
            Console.WriteLine("Generated XML is valid.");
            Console.WriteLine(xmlString);
            File.WriteAllText("..\\..\\..\\xmlString.xml", xmlString);
        }
        else
        {
            Console.WriteLine("Generated XML is invalid.");
            File.Delete("..\\..\\..\\xmlString.xml");
            //throw new Exception("Generated XML is invalid.");

        }

        return xmlString;
    }

    private PRIA_DOCUMENT_Type GeneratePriaDocument()
    {
        return new PRIA_DOCUMENT_Type
        {
            GRANTEE =
            [
                new()
                {
                    _FirstName = "John",
                    _LastName = "Doe"
                }
            ],
            GRANTOR =
            [
                new()
                {
                    _FirstName = "Jane",
                    _LastName = "Doe"
                }
            ],
            PROPERTY =
            [
                new()
                {
                    _StreetAddress = "123 Main St",
                    _City = "Somewhere",
                    _State = "FL",
                    _PostalCode = "12345"
                }
            ],
            PARTIES = new()
            {
                //_ID = "zxcvbnm",
                _RETURN_TO_PARTY = [
                    new()
                    {
                        _ID="qweqweqwew",
                        PREFERRED_RESPONSE = [
                            new()
                            {
                                _ID="eijsijisrojbgoigsjbojs",
                                
                            }
                        ],
                        NON_PERSON_ENTITY_DETAIL = new() 
                        {
                            _ID = "asdaddwefregb"
                        },
                        CONTACT_DETAIL = new() 
                        {
                            _ID = "asdaddwefregbasdasdas",
                            _Name = "asdasdreg"
                        }

                    }
                ]
            },
            EXECUTION = new()
            {
                _ID = "asdaddwasdadasdefregb"
            }
        };
    }
}
