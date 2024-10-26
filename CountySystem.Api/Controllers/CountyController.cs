using CountySystem.Service;
using Microsoft.AspNetCore.Mvc;

namespace CountySystem.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CountyController(PriaService priaService, CountyService countyService) : Controller
{
    [HttpGet(template: "document-types")]
    public async Task<IActionResult> GetAllDocumentTypes()
    {
        var documentTypes = await countyService.GetAllDocumentTypes();
        return Ok(documentTypes);
    }

    [HttpPost("submit-pria-package")]
    public async Task<IActionResult> SubmitPriaPackage()
    {
        var xmlString = priaService.GenerateXmlString(priaService.GeneratePriaDocument(), true);

        var response = await countyService.SubmitPackageAsync("1", "1");

        return Ok(new
        {
            CountyResponse = response,
            PriaPackage = xmlString
        });
    }

    [HttpPost("generate-pria-package")]
    public async Task<IActionResult> GeneraetPriaStandardXml([FromQuery] bool isEscaped = false)
    {
        var xmlString = priaService.GenerateXmlString(priaService.GeneratePriaDocument(), isEscaped);
        return Ok(xmlString);
    }

    [HttpPost("validate-xml-file")]
    public async Task<IActionResult> ValidatePriaStandardXmlFile(IFormFile file)
    {
        string xmlString;
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            xmlString = await reader.ReadToEndAsync();
        }

        string message = "Document is valid";
        bool isValid = false;
        try
        {
            isValid = priaService.ValidateXml(xmlString);
        }
        catch (Exception e)
        {
            message = e.Message;
        }

        return Ok(new
        {
            IsValid = isValid,
            Message = message
        });
    }


    [HttpPost("validate-xml-string")]
    public async Task<IActionResult> ValidatePriaStandardXmlString(string xmlString)
    {
        string message = "Document is valid";
        bool isValid = false;
        try
        {
            isValid = priaService.ValidateXml(xmlString);
        }
        catch (Exception e)
        {
            message = e.Message;
        }

        return Ok(new
        {
            IsValid = isValid,
            Message = message
        });
    }
}