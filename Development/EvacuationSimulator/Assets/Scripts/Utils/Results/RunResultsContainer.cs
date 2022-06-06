using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Assets.Scripts.Utils.Results
{
    [Serializable]
    [XmlType("RUN")]
    [XmlInclude(typeof(BotResult))]
    public class RunResultsContainer
    {
        [XmlElement("BOT")]
        public BotResult[] Results;

        [XmlAttribute("Number")]
        public uint IndexOfRun { get; set; } = 0;

        private int arrayIndex = 0;

        public RunResultsContainer()
        { }

        public RunResultsContainer(int numberOfResults)
        {
            Results = new BotResult[numberOfResults];
        }

        public RunResultsContainer(int numberOfResults, uint runIndex)
        {
            Results = new BotResult[numberOfResults];
            IndexOfRun = runIndex;
        }

        /// <summary>
        /// Add result to container (not thread safe).
        /// </summary>
        /// <param name="result"></param>
        public void Add(BotResult result)
        {
            Results[arrayIndex++] = result;
        }

        /// <summary>
        /// Add result to container (thread safe but won't go well with the other add).
        /// </summary>
        /// <param name="result"></param>
        /// <param name="index"></param>
        public void Add(BotResult result, int index)
        {
            Results[index] = result;
        }

        public bool IsCompleted()
        {
            foreach(var result in Results)
            {
                if(result == null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
