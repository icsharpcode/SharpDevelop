// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
