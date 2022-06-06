using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Assets.Scripts.Utils.Results
{
    [Serializable]
    [XmlType("RESULTS")]
    [XmlInclude(typeof(RunResultsContainer))]
    public class ResultsManager
    {
        [XmlElement("RUN")]
        public List<RunResultsContainer> Containers = new List<RunResultsContainer>();

        public void Add(RunResultsContainer container)
        {
            Containers.Add(container);
        }

        public void Save()
        {
            var serializer = new XmlSerializer(typeof(ResultsManager));
            using (var streamWriter = new StreamWriter($"EvacuationSimulator_Result.xml"))
            {
                serializer.Serialize(streamWriter, this);
                streamWriter.Flush();
            }
        }
    }
}
