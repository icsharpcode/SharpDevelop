using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.CodeQualityAnalysis.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class Namespace : IDependency, INode
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

            Dependency = this;
        }

        public DependencyGraph BuildDependencyGraph()
        {
            var g = new DependencyGraph();

            foreach (var type in Types)
            {
                g.AddVertex(new DependencyVertex(type));
            }

            foreach (var type in Types)
            {
                var types = type.GetUses();

                foreach (var dependType in types)
                {
                    if (dependType != type && dependType.Namespace == type.Namespace)
                        g.AddEdge(new DependencyEdge(new DependencyVertex(type), new DependencyVertex(dependType)));
                }
            }

            return g;
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
