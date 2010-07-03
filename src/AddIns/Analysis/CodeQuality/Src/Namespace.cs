using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Namespace : IDependency
    {
        /// <summary>
        /// Types within namespace
        /// </summary>
        public ISet<Type> Types { get; set; }

        /// <summary>
        /// Name of namespace
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Module where is namespace located
        /// </summary>
        public Module Module { get; set; }

        public Namespace()
        {
            Types = new HashSet<Type>();
        }
        
        public BidirectionalGraph<object, IEdge<object>> BuildDependencyGraph()
        {
            var g = new BidirectionalGraph<object, IEdge<object>>();

            foreach (var type in Types)
            {
                g.AddVertex(type.Name);
            }

            foreach (var type in Types)
            {
                var types = type.GetUses();

                foreach (var dependType in types)
                {
                    if (dependType != type && dependType.Namespace == type.Namespace)
                        g.AddEdge(new Edge<object>(type.Name, dependType.Name));
                }
            }

            return g;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
