using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Module : IDependency
    {
        /// <summary>
        /// Namespaces within module
        /// </summary>
        public ISet<Namespace> Namespaces { get; set; }

        /// <summary>
        /// Name of module
        /// </summary>
        public string Name { get; set; }

        public Module()
        {
            Namespaces = new HashSet<Namespace>();
        }

        public BidirectionalGraph<object, IEdge<object>> BuildDependencyGraph()
        {
            return null;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
