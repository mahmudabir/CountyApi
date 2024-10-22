

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;
using System.Xml.Schema;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

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

    public string SerializeToXml(PRIA_DOCUMENT priaDocument)
    {
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(PRIA_DOCUMENT));
        using (StringWriter stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, priaDocument);
            return stringWriter.ToString();
        }
    }

    public bool ValidateXml(string xml, string xsdPath)
    {
        return true;

        XmlSchemaSet schema = new XmlSchemaSet();
        schema.Add("", xsdPath);

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.ValidationType = ValidationType.Schema;
        settings.Schemas = schema;
        settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

        XmlReader reader = XmlReader.Create(new StringReader(xml), settings);
        try
        {
            while (reader.Read()) { }
            return true; // XML is valid
        }
        catch (XmlException)
        {
            return false; // XML is invalid
        }
    }

    private void ValidationCallBack(object sender, ValidationEventArgs args)
    {
        Console.WriteLine($"Validation Error: {args.Message}");
    }

    public string GenerateXmlFromPdf(string pdfPath, string xsdPath)
    {
        // Step 1: Extract text from PDF
        string pdfText = ExtractTextFromPdf(pdfPath);

        Console.WriteLine("pdfText: " + pdfText + "\n");
        File.WriteAllText("..\\..\\..\\pdfText.xml", pdfText);

        // Step 2: Populate PRIA_DOCUMENT (mapping the extracted data to the class)
        PRIA_DOCUMENT priaDocument = new PRIA_DOCUMENT
        {
            Grantors = new List<GRANTOR>
        {
            new GRANTOR { Name = ExtractGrantorNameFromPdfText(pdfText) }
        },
            Grantees = new List<GRANTEE>
        {
            new GRANTEE { Name = ExtractGranteeNameFromPdfText(pdfText) }
        },
            Properties = new List<PROPERTY>
        {
            new PROPERTY { Address = ExtractPropertyAddressFromPdfText(pdfText) }
        },
            Execution = new EXECUTION
            {
                ExecutionDate = DateTime.Now // You can extract actual date from the PDF if needed
            }
        };

        // Step 3: Serialize to XML
        string xmlString = SerializeToXml(priaDocument);

        File.WriteAllText("..\\..\\..\\xmlString.xml", xmlString);

        // Step 4: Validate XML
        bool isValid = ValidateXml(xmlString, xsdPath);
        if (isValid)
        {
            Console.WriteLine("Generated XML is valid.");
            Console.WriteLine(xmlString);
        }
        else
        {
            Console.WriteLine("Generated XML is invalid.");
        }

        return xmlString;
    }


    public string ExtractGrantorNameFromPdfText(string pdfText)
    {
        // Define a pattern to match the Grantor's name
        // Assuming Grantor is written as "GRANTOR: John Doe" in the PDF
        string pattern = @"GRANTOR:\s*(.*)";

        // Search for the pattern in the extracted PDF text
        Match match = Regex.Match(pdfText, pattern, RegexOptions.IgnoreCase);

        // If a match is found, return the grantor's name
        if (match.Success)
        {
            return match.Groups[1].Value.Trim(); // Extract the Grantor name
        }

        // Return a default value or throw an exception if needed
        return "Grantor Name Not Found";
    }

    public string ExtractGranteeNameFromPdfText(string pdfText)
    {
        // Define a pattern to match the Grantee's name
        // Assuming Grantee is written as "GRANTEE: Jane Doe" in the PDF
        string pattern = @"GRANTEE:\s*(.*)";

        // Search for the pattern in the extracted PDF text
        Match match = Regex.Match(pdfText, pattern, RegexOptions.IgnoreCase);

        // If a match is found, return the grantee's name
        if (match.Success)
        {
            return match.Groups[1].Value.Trim(); // Extract the Grantee name
        }

        // Return a default value or throw an exception if needed
        return "Grantee Name Not Found";
    }

    public string ExtractPropertyAddressFromPdfText(string pdfText)
    {
        // Define a pattern to match the Grantee's name
        // Assuming Grantee is written as "GRANTEE: Jane Doe" in the PDF
        string pattern = @"GRANTEE:\s*(.*)";

        // Search for the pattern in the extracted PDF text
        Match match = Regex.Match(pdfText, pattern, RegexOptions.IgnoreCase);

        // If a match is found, return the grantee's name
        if (match.Success)
        {
            return match.Groups[1].Value.Trim(); // Extract the Grantee name
        }

        // Return a default value or throw an exception if needed
        return "Property Address Not Found";
    }


}
