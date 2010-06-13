// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class RunTestWithDebuggerCommand : AbstractRunTestCommand
	{
		public RunTestWithDebuggerCommand()
		{
		}
		
		public RunTestWithDebuggerCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		protected override ITestRunner CreateTestRunner(IProject project)
		{
			return Context.RegisteredTestFrameworks.CreateTestDebugger(project);
		}
	}
}
