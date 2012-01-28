// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.CodeQualityAnalysis
{
    public class DependencyGraphCommand : AbstractMenuCommand
    {
    	public override void Run()
    	{
            var window = new MainWindow();
            window.Show();
    	}
    }
}
