using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CountyApi;
class PdfToXmlConverter
{
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
        XmlSchemaSet schema = new XmlSchemaSet();//C:\Users\abir\Desktop\Projects\dotnet\CountyApi\CountyApi\PRIA_DOCUMENT_v2_4_1.XSD
        schema.Add("", @"..\..\..\PRIA_DOCUMENT_v2_4_1.XSD");
        schema.Add("", @"..\..\..\MISMO_SIGNATURE_Type.xsd");
        schema.Add("http://www.w3.org/2000/09/xmldsig#", @"..\..\..\SMART_DOCUMENT_xmldsig_core_schema_V_1_02.xsd");

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

    public string GenerateXmlString()
    {
        // Step 1: Populate PRIA_DOCUMENT (mapping the extracted data to the class)
        var priaDocument = GeneratePriaDocument();

        // Step 2: Serialize to XML
        string xmlString = SerializeToXml(priaDocument);

        // Step 3: Validate XML
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

    public static string ConvertFileToBase64(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        return Convert.ToBase64String(fileBytes);
    }
    
    private PRIA_DOCUMENT_Type GeneratePriaDocument()
    {
        string pdfFilePath = @"C:\Users\abir\Downloads\noc.tiff";
        var base64String = ConvertFileToBase64(pdfFilePath);

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
            },
            EMBEDDED_FILE = [
                new()
                {
                    ID = "asasdasdas",
                    EmbeddedFileType = "TIFF",
                    EmbeddedFileVersion="1.0",
                    EmbeddedFileName="DocumentImage.tiff",
                    FileEncodingType="Base64",
                    FileDescription="Property Deed Document",
                    MIMEType="image/tiff",
                    //PageCount ="1",
                    //_SequenceIdentifier="1",
                }
            ]
        };
    }
}
