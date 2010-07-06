using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis.Controls
{
    public class DependencyEdge : Edge<DependencyVertex>
    {
        public DependencyEdge(DependencyVertex source, DependencyVertex target) : base(source, target)
        {
        }
    }
}
