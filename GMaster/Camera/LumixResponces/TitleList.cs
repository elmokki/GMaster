namespace GMaster.Camera.LumixResponces
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "titlelist")]
    public class TitleList
    {
        [XmlAttribute(AttributeName = "date")]
        public string Date { get; set; }

        [XmlElement(ElementName = "language")]
        public HashCollection<Language> Languages { get; set; }

        [XmlAttribute(AttributeName = "model")]
        public string Model { get; set; }

        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
}