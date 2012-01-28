// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.CodeQualityAnalysis.Controls;
using QuickGraph;

namespace ICSharpCode.CodeQualityAnalysis
{
    public interface IDependency
    {
        DependencyGraph BuildDependencyGraph();
    }
}
