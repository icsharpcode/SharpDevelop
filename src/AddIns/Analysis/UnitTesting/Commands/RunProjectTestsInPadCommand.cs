// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.UnitTesting
{
	public class RunProjectTestsInPadCommand : RunTestInPadCommand, ITestTreeView
	{
		public RunProjectTestsInPadCommand()
		{
		}
		
		public RunProjectTestsInPadCommand(IRunTestCommandContext context)
			: base(context)
		{
		}
		
		public override void Run()
		{
			Owner = this;
			base.Run();
		}
		
		public TestMember SelectedMember {
			get { return null; }
		}
		
		public TestClass SelectedClass {
			get { return null; }
		}
		
		public TestProject SelectedProject {
			get { return TestService.Solution.GetTestProject(ProjectService.CurrentProject); }
		}
		
		public string SelectedNamespace {
			get { return null; }
		}
	}
}
