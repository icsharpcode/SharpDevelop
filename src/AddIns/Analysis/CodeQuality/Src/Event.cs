using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Event : INode
    {
        /// <summary>
        /// Name of event
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of event
        /// </summary>
        public Type EventType { get; set; }

        /// <summary>
        /// Type which owns this event
        /// </summary>
        public Type Owner { get; set; }

        public Event()
        {
            Dependency = null;
        }

        public override string ToString()
        {
            return Name;
        }

        public IDependency Dependency { get; set; }

        public string GetInfo()
        {
            return this.ToString();
        }
    }
}
