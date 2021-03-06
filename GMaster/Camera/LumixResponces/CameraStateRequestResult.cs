﻿namespace GMaster.Camera.LumixResponces
{
    using System.Xml.Serialization;

    [XmlRoot(ElementName = "camrply")]
    public class CameraStateRequestResult : BaseRequestResult
    {
        [XmlElement(ElementName = "state")]
        public CameraState State { get; set; }
    }
}