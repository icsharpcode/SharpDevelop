using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Field : IDependency
    {
        /// <summary>
        /// Name of field
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type of field
        /// </summary>
        public Type Type { get; set; }

        public BidirectionalGraph<object, IEdge<object>> BuildDependencyGraph()
        {
            return null;
        }
    }
}
