// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public class RunTestInPadCommand : AbstractRunTestCommand
	{
		public RunTestInPadCommand()
		{
		}
		
		public RunTestInPadCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		protected override ITestRunner CreateTestRunner(IProject project)
		{
			return Context.RegisteredTestFrameworks.CreateTestRunner(project);
		}
	}
}
