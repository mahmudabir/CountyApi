using System.Security;
using CountySystem.MainModuleService;

namespace CountyApi;

class Program
{
    static async Task Main(string[] args)
    {
        string agentKey = "1";
        string agentPassword = "1";
        
        // GetDocumentTypesResponse documentTypesResponse = await GetAllDocumentTypes();
        string submitPackageResponse = await SubmitPackageAsync(agentKey, agentPassword);
        
    }

    // Get All Document Types
    public static async Task<GetDocumentTypesResponse> GetAllDocumentTypes()
    {
        ERecordWCFServiceClient countyClient = new ERecordWCFServiceClient();
        GetDocumentTypesResponse? documentTypeResponse = await countyClient.GetDocumentTypesAsync(new GetDocumentTypesRequest());
        await countyClient.CloseAsync();
        return documentTypeResponse;
    }

    // Method to call the SubmitPackage SOAP API
    public static async Task<string> SubmitPackageAsync(string agentKey, string agentPassword)
    {
        PdfToXmlConverter converter = new PdfToXmlConverter();
        var priaPackageXml = converter.GenerateXmlString();
        
        // Encode the XML string (escape special characters)
        string priaPackage = SecurityElement.Escape(priaPackageXml);

        ERecordWCFServiceClient countyClient = new ERecordWCFServiceClient();
        var result = await countyClient.SubmitPackageAsync(new SubmitPackageRequest()
        {
            agentKey = agentKey,
            agentPassword = agentPassword,
            priaPackage = priaPackage
        });
        await countyClient.CloseAsync();
        
        Console.WriteLine("Response from SubmitPackage API:");
        Console.WriteLine(result.SubmitPackageResult);

        return result.SubmitPackageResult;
        
        /*
        // HttpClient client = new HttpClient();
        
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
        //return await response.Content.ReadAsStringAsync();*/
    }
    
}