// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	public class RunTestWithProfilerCommand : AbstractRunTestCommand
	{
		protected override ITestRunner CreateTestRunner(IProject project)
		{
			return new ProfilerTestRunner();
		}
	}
}
