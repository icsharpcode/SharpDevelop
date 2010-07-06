using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
    public class DependencyVertex
    {
        public INode Node { get; private set; }

        public DependencyVertex(INode node)
        {
            Node = node;
        }

        public override string ToString()
        {
            return Node.ToString();
        }

        public override bool Equals(object obj)
        {
            var dependencyVertex = obj as DependencyVertex;

            if (obj == null)
                return false;
            else
            {
                return this.Node.Equals(dependencyVertex.Node);
            }
        }

        public override int GetHashCode()
        {
            return this.Node.GetHashCode();
        }
    }
}
