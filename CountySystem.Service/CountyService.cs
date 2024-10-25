using System.Security;
using CountySystem.Service.ERecordWCFService;

namespace CountySystem.Service;

public class CountyService
{
    private readonly PriaService _priaService;

    public CountyService(PriaService priaService)
    {
        _priaService = priaService;
    }
    
    // Get All Document Types
    public async Task<GetDocumentTypesResponse> GetAllDocumentTypes()
    {
        ERecordWCFServiceClient countyClient = new ERecordWCFServiceClient();
        GetDocumentTypesResponse? documentTypeResponse = await countyClient.GetDocumentTypesAsync(new GetDocumentTypesRequest());
        // var documentTypeResponseJson = JsonSerializer.Serialize(documentTypeResponse.GetDocumentTypesResult);
        // Console.WriteLine(documentTypeResponseJson);
        await countyClient.CloseAsync();
        return documentTypeResponse;
    }

    // Method to call the SubmitPackage SOAP API
    public async Task<string> SubmitPackageAsync(string agentKey, string agentPassword)
    {
        var priaPackageXml = _priaService.GenerateXmlString();
        
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
        
        // Console.WriteLine("Response from SubmitPackage API:");
        // Console.WriteLine(result.SubmitPackageResult);

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