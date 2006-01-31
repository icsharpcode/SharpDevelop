// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.MbUnitPad;
using System;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Menu command selected after right clicking a test fixture in the text editor 
	/// to run tests with code coverage.
	/// </summary>
	public class RunTestFixtureWithCodeCoverageCommand : AbstractRunTestsWithCodeCoverageCommand
	{
		IProject project;
		
		public override void Run()
		{
			IMember m = MbUnitTestableCondition.GetMember(Owner);
			IClass c = (m != null) ? m.DeclaringType : MbUnitTestableCondition.GetClass(Owner);
			project = c.ProjectContent.Project;
			if (project != null) {
				base.Run();
			} else {
				MessageService.ShowMessage("Unable to determine project.");
			}
		}
		
		protected override IProject GetProject()
		{
			return project;
		}
		
		protected override string GetTestAssemblyFileName()
		{
			return project.OutputAssemblyFullPath;
		}
	}
}

