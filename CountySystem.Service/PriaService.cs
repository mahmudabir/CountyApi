using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CountySystem.Service;

public class PriaService
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
        XmlSchemaSet schema = new XmlSchemaSet();
        schema.Add("", @"XmlSchemaDefinitions\PRIA_REQUEST_v2_4_2.xsd");
        schema.Add("", @"XmlSchemaDefinitions\PRIA_DOCUMENT_v2_4_1.xsd");
        schema.Add("", @"XmlSchemaDefinitions\MISMO_SIGNATURE_Type.xsd");
        schema.Add("http://www.w3.org/2000/09/xmldsig#",
            @"XmlSchemaDefinitions\SMART_DOCUMENT_xmldsig_core_schema_V_1_02.xsd");

        XmlReaderSettings settings = new XmlReaderSettings();
        settings.ValidationType = ValidationType.Schema;
        settings.Schemas = schema;

        bool isValid = true; // This will track if validation passed or failed

        // Capture schema validation errors
        settings.ValidationEventHandler += (sender, e) =>
        {
            isValid = false; // Mark as invalid on the first error
            // Console.WriteLine($"Validation Error: {e.Message}\n");
            throw new Exception(e.Message);
        };

        try
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(xml), settings))
            {
                while (reader.Read())
                {
                } // Read through the XML
            }
        }
        catch (XmlException ex)
        {
            Console.WriteLine($"XML Exception: {ex.Message}");
            return false; // Invalid XML syntax
        }

        return isValid; // Returns true if valid, false if validation failed
    }

    public string GenerateXmlString(PRIA_REQUEST_GROUP_Type priaDocument, bool isEscaped = false)
    {
        // Step 2: Serialize to XML
        string xmlString = SerializeToXml(priaDocument);

        // Step 3: Validate XML
        if (!ValidateXml(xmlString))
        {
            throw new Exception("Generated XML is invalid.");
        }

        return isEscaped ? SecurityElement.Escape(xmlString) : xmlString;
    }

    public string ConvertFileToBase64(string filePath)
    {
        byte[] fileBytes = File.ReadAllBytes(filePath);
        return Convert.ToBase64String(fileBytes);
    }

    public PRIA_REQUEST_GROUP_Type GeneratePriaDocument()
    {
        string pdfFilePath = @"C:\noc.pdf";
        var base64String = ConvertFileToBase64(pdfFilePath);

        var priaDocument = new PRIA_DOCUMENT_Type
        {
            _ID = "adasdasd",
            _Code = "NOC",
            DocumentNonRecordableIndicator = "Y", // Y/N
            PRIAVersionIdentifier = "2.4.2",
            _UniqueIdentifier = "asdas13426",
            RecordableDocumentSequenceIdentifier = "123asd456789",
            RecordableDocumentType = PRIA_RecordableDocumentTypeEnumerated.DeedOfTrust,
            RecordableDocumentTypeOtherDescription = "Deed of Trust",

            GRANTEE =
            [
                new()
                {
                    _ID = "jyfgduyjt",
                    _StreetAddress = "1",
                    _StreetAddress2 = "1",
                    _City = "1",
                    _State = "1",
                    _PostalCode = "1",
                    _County = "1",
                    _Country = "1",
                    _UnparsedName = "John Doe",
                    _CapacityDescription = "1",
                    MaritalStatusType = PRIA_MaritalStatusTypeEnumerated.Married,
                    NonPersonEntityIndicator = "N", // Y/N
                    _SequenceIdentifier = "1",
                }
            ],
            GRANTOR =
            [
                new()
                {
                    _ID = "kghjvfjkly",
                    _StreetAddress = "2",
                    _StreetAddress2 = "2",
                    _City = "2",
                    _State = "2",
                    _PostalCode = "2",
                    _County = "2",
                    _Country = "2",
                    _UnparsedName = "Jane Doe",
                    _CapacityDescription = "2",
                    MaritalStatusType = PRIA_MaritalStatusTypeEnumerated.Married,
                    NonPersonEntityIndicator = "N",
                    _SequenceIdentifier = "2",
                }
            ],
            PROPERTY =
            [
                new()
                {
                    _ID = "asdasdsad",
                    MISMOVersionId = "1.0",
                    _StreetAddress = "123 Main St",
                    _StreetAddress2 = "",
                    _City = "Somewhere",
                    _State = "FL",
                    _PostalCode = "12345",
                    _County = "",
                    _Country = "",
                    BuildingStatusType = PRIA_BuildingStatusTypeEnumerated.Complete,
                    LandUseType = PRIA_LandUseTypeEnumerated.Residential,
                    LandUseTypeOtherDescription = "",
                    LandUseComment = "",
                    _CurrentOccupancyType = PRIA_PropertyCurrentOccupancyTypeEnumerated.Vacant,
                    _CurrentOccupantUnparsedName = "",
                    _GatedCommunityIndicator = "Y",
                    _RightsType = PRIA_PropertyRightsTypeEnumerated.FeeSimple,
                    _TaxDelinquentIndicator = "Y",
                    _TitleCategoryType = PRIA_PropertyTitleCategoryTypeEnumerated.Other,
                    _TitleCategoryTypeOtherDescription = "",

                    _IDENTIFICATION = new PRIA_PROPERTY_IDENTIFICATION_Type()
                    {
                        _ID = "asdasdasdasdasd"
                    },

                    PARSED_STREET_ADDRESS = null,

                    _LEGAL_DESCRIPTION =
                    [
                        new()
                        {
                            _ID = "yrtdjrydjtyu",
                            _TextDescription = "asdas dsd as",
                            _Type = PRIA_PropertyLegalDescriptionTypeEnumerated.ShortLegal,
                            _TypeOtherDescription = "adkj hasj hajs hlasj das",

                            PARCEL_IDENTIFICATION = new()
                            {
                                _ID = "asdasdasd",
                                _Type = PRIA_ParcelIdentificationTypeEnumerated.Other,
                            }
                        }
                    ],
                }
            ],
            PARTIES = new()
            {
                _RETURN_TO_PARTY =
                [
                    new()
                    {
                        _ID = "qweqweqwew",
                        PREFERRED_RESPONSE =
                        [
                            new()
                            {
                                _ID = "eijsijisrojbgoigsjbojs",
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
                ],
                _PREPARED_BY_PARTY =
                [
                    new PRIA_PARTIES_PREPARED_BY_PARTY_Type()
                    {
                        _ID = "sdasdasdasd",
                        _UnparsedName = "asd dasd asd asd a",
                        
                        PREFERRED_RESPONSE = [
                            new PRIA_PREFERRED_RESPONSE_Type()
                            {
                                _ID = "BFHILOSU"
                            }
                        ],
                        NON_PERSON_ENTITY_DETAIL = new()
                        {
                            _ID = "EWEWWERTGYIRKS"
                        },
                        CONTACT_DETAIL = new()
                        {
                            _ID = "sdaASDsdasdasd"
                        }
                    }
                ]
            },
            EXECUTION = new()
            {
                _ID = "asdaddwasdadasdefregb",
                _Date = "2024-03-03",
                _City = "42543254 ",
                _County = "42543254 ",
                _State = "42543254 ",
            },
            EMBEDDED_FILE =
            [
                new()
                {
                    ID = "asasdasdas",
                    EmbeddedFileType = "PDF",
                    EmbeddedFileVersion = "1.0",
                    EmbeddedFileName = "noc.pdf",
                    FileEncodingType = "base64",
                    FileDescription = "Notice of Commencement",
                    MIMEType = "application/pdf",
                    // DOCUMENT = base64String
                }
            ]
        };

        PRIA_REQUEST_GROUP_Type priaRequestGroup = new PRIA_REQUEST_GROUP_Type()
        {
            PRIAVersionIdentifier = "2.4.2",
            REQUEST =
            [
                new REQUEST_Type()
                {
                    PRIA_REQUEST =
                    [
                        new PRIA_REQUEST_Type()
                        {
                            PACKAGE =
                            [
                                new PRIA_PACKAGE_Type()
                                {
                                    PAYMENT =
                                    [
                                        new PRIA_PAYMENT_Type()
                                        {
                                            _ReferenceIdentifier = "333333333",
                                            _AccountIdentifier = "y8362ls6a2",
                                            _Amount = "250"
                                        },
                                        new PRIA_PAYMENT_Type()
                                        {
                                            _ReferenceIdentifier = "222222222",
                                            _AccountIdentifier = "y836asdasdasd-asd2ls6a2",
                                            _Amount = "2950"
                                        }
                                    ],
                                    PRIA_DOCUMENT = [priaDocument]
                                }
                            ],
                        }
                    ]
                }
            ]
        };

        return priaRequestGroup;
    }
}