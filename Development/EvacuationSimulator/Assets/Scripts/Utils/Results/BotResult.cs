using System;
using System.Xml.Serialization;

namespace Assets.Scripts.Utils.Results
{
    [Serializable]
    public class BotResult
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Distance")]
        public string Distance { get; set; }

        [XmlAttribute("Time")]
        public string Time { get; set; }

        [XmlAttribute("Status")]
        public string Status { get; set; }
    }
}
