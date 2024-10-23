using CountySystem.MainModuleService;
using System.Security;

namespace CountyApi;

class Program
{
    private static readonly HttpClient client = new HttpClient();

    static async Task Main(string[] args)
    {
        string agentKey = "1";
        string agentPassword = "1";

        PdfToXmlConverter converter = new PdfToXmlConverter();
        var xmlString = converter.GenerateXmlFromPdf();

        string response = await SubmitPackageAsync(agentKey, agentPassword, xmlString);
        Console.WriteLine("Response from SubmitPackage API:");
        Console.WriteLine(response);
    }

    // Method to call the SubmitPackage SOAP API
    public static async Task<string> SubmitPackageAsync(string agentKey, string agentPassword, string priaPackageXml)
    {
        // Encode the XML string (escape special characters)
        string priaPackage = SecurityElement.Escape(priaPackageXml);

        ERecordWCFServiceClient countyClient = new ERecordWCFServiceClient();

        var result = await countyClient.SubmitPackageAsync(new SubmitPackageRequest()
        {
            agentKey = agentKey,
            agentPassword = agentPassword,
            priaPackage = priaPackage
        });

        return result.SubmitPackageResult;

        //// SOAP envelope for the SubmitPackage request
        //var soapEnvelope = $@"
        //<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:lan=""Landmark.DistributedServices.ERecord"">
        //   <soapenv:Header/>
        //   <soapenv:Body>
        //      <lan:SubmitPackage>
        //         <lan:agentKey>{agentKey}</lan:agentKey>
        //         <lan:agentPassword>{agentPassword}</lan:agentPassword>
        //         <lan:priaPackage>{priaPackage}</lan:priaPackage>
        //      </lan:SubmitPackage>
        //   </soapenv:Body>
        //</soapenv:Envelope>";

        //var content = new StringContent(soapEnvelope, Encoding.UTF8, "text/xml");

        //// Set SOAPAction header as required by the service
        //client.DefaultRequestHeaders.Clear();
        //client.DefaultRequestHeaders.Add("SOAPAction", "\"Landmark.DistributedServices.ERecord/IERecordWCFService/SubmitPackage\"");

        //// Send POST request to the SOAP endpoint
        //HttpResponseMessage response = await client.PostAsync("https://erecordqa.mypalmbeachclerk.com/MainModuleService.svc/basic", content);

        //// Ensure the request was successful and return the response content
        //return await response.Content.ReadAsStringAsync();
    }
}