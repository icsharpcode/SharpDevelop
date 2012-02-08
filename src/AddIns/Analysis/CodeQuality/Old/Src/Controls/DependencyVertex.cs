// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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

            if (dependencyVertex == null)
                return false;
            
            return Node.Equals(dependencyVertex.Node);
        }

        public override int GetHashCode()
        {
            return this.Node.GetHashCode();
        }
    }
}
