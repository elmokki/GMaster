﻿namespace GMaster.Camera.LumixResponces
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "camrply")]
    public class MenuSetRuquestResult : BaseRequestResult
    {
        [XmlElement(ElementName = "menuset")]
        public RawMenuSet MenuSet { get; set; }
    }
}
