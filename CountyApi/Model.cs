using System.Xml.Serialization;

namespace CountyApi
{

    [XmlRoot("PRIA_DOCUMENT")]
    public class PRIA_DOCUMENT
    {
        [XmlElement("GRANTOR")]
        public List<GRANTOR> Grantors { get; set; }

        [XmlElement("GRANTEE")]
        public List<GRANTEE> Grantees { get; set; }

        [XmlElement("PROPERTY")]
        public List<PROPERTY> Properties { get; set; }

        [XmlElement("EXECUTION")]
        public EXECUTION Execution { get; set; }

        // Add more fields as needed from the DTD
    }

    public class GRANTOR
    {
        [XmlElement("Name")]
        public string Name { get; set; }
    }

    public class GRANTEE
    {
        [XmlElement("Name")]
        public string Name { get; set; }
    }

    public class PROPERTY
    {
        [XmlElement("Address")]
        public string Address { get; set; }

        // More fields like LegalDescription, TaxID, etc.
    }

    public class EXECUTION
    {
        [XmlElement("ExecutionDate")]
        public DateTime ExecutionDate { get; set; }

        // More fields like Signatures, Notary information, etc.
    }

}
