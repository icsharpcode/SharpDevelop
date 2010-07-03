using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Event : IDependency
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

        public BidirectionalGraph<object, IEdge<object>> BuildDependencyGraph()
        {
            return null;
        }
    }
}
