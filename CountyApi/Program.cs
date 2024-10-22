using CountySystem.MainModuleService;
using System.Reflection.PortableExecutable;
using System.Security;
using System.Text;
using System.Xml;

namespace CountyApi;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        string agentKey = "#1528";
        string agentPassword = "1";
        string filePath = "c:\\sample.pdf";

        PdfToXmlConverter converter = new PdfToXmlConverter();
        var xmlString = converter.GenerateXmlFromPdf(filePath, @"C:\Users\acer\Downloads\PRIA_REQUEST_v2_4_2\PRIA_DOCUMENT_v2_4_1.XSD");

        string response = await SubmitPackageAsync(agentKey, agentPassword, xmlString);
        Console.WriteLine("Response from SubmitPackage API:");
        Console.WriteLine(response);
    }

    // Method to call the SubmitPackage SOAP API
    public static async Task<string> SubmitPackageAsync(string agentKey, string agentPassword, string priaPackageXml)
    {
        // Sample PRIA package XML content as string (escaped properly)
        //string priaPackageXml = @"
        //<PACKAGE xmlns=""http://www.pria.org/eRecording"">
        //    <PRIA_DOCUMENT _UniqueIdentifier=""111111111"" PRIAVersionIdentifier=""2.4.2"">
        //        <GRANTOR _FirstName=""John"" _LastName=""Doe""/>
        //        <GRANTEE _FirstName=""Jane"" _LastName=""Doe""/>
        //        <PROPERTY _StreetAddress=""123 Main St"" _City=""Somewhere"" _State=""FL"" _PostalCode=""12345""/>
        //        <EXECUTION _ExecutionDate=""2024-10-01""/>
        //        <MORTGAGE_CONSIDERATION _Amount=""250000.00""/>
        //        <SIGNATORY _Name=""John Doe""/>
        //        <NOTARY _Name=""Notary Public"" _CommissionNumber=""12345""/>
        //        <RECORDING_ENDORSEMENT _EndorsementDate=""2024-10-22""/>
        //    </PRIA_DOCUMENT>
        //</PACKAGE>";

        // Encode the XML string (escape special characters)
        string priaPackage = SecurityElement.Escape(priaPackageXml);

        // SOAP envelope for the SubmitPackage request
        var soapEnvelope = $@"
        <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:lan=""Landmark.DistributedServices.ERecord"">
           <soapenv:Header/>
           <soapenv:Body>
              <lan:SubmitPackage>
                 <lan:agentKey>{agentKey}</lan:agentKey>
                 <lan:agentPassword>{agentPassword}</lan:agentPassword>
                 <lan:priaPackage>{priaPackage}</lan:priaPackage>
              </lan:SubmitPackage>
           </soapenv:Body>
        </soapenv:Envelope>";

        var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

        // Set SOAPAction header as required by the service
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("SOAPAction", "\"Landmark.DistributedServices.ERecord/IERecordWCFService/SubmitPackage\"");

        //// Send POST request to the SOAP endpoint
        //HttpResponseMessage response = await client.PostAsync("https://erecordqa.mypalmbeachclerk.com/MainModuleService.svc/basic", content);

        //// Ensure the request was successful and return the response content
        //return await response.Content.ReadAsStringAsync();


        ERecordWCFServiceClient countyClient = new ERecordWCFServiceClient();

        var result = await countyClient.SubmitPackageAsync(new SubmitPackageRequest()
        {
            agentKey = agentKey,
            agentPassword = agentPassword,
            priaPackage = priaPackage
        });

        return result.SubmitPackageResult;


        //ERecordWCFServiceClient countyClient = new ERecordWCFServiceClient();

        //var result2 = await countyClient.SubmitPackageAsync(new SubmitPackageRequest()
        //{
        //    agentKey = agentKey,
        //    agentPassword = agentPassword,
        //    priaPackage = base64File
        //});

        //Console.WriteLine(result);
    }
}