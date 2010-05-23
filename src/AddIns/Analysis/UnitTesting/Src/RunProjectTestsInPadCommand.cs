// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
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
		
		public IMember SelectedMethod {
			get { return null; }
		}
		
		public IClass SelectedClass {
			get { return null; }
		}
		
		public IProject SelectedProject {
			get { return ProjectService.CurrentProject; }
		}
		
		public string SelectedNamespace {
			get { return null; }
		}
	}
}
