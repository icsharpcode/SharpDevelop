// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Custom build command that makes sure errors and warnings
	/// are not cleared from the Errors list before every build since
	/// we may be running multiple tests after each other.
	/// </summary>
	public class BuildProjectBeforeTestRun : BuildProjectBeforeExecute
	{
		IUnitTestSaveAllFilesCommand saveAllFilesCommand;
		
		public BuildProjectBeforeTestRun(IProject targetProject,
			IUnitTestSaveAllFilesCommand saveAllFilesCommand)
			: base(targetProject)
		{
			this.saveAllFilesCommand = saveAllFilesCommand;
		}
		
		public BuildProjectBeforeTestRun(IProject targetProject)
			: this(targetProject, new UnitTestSaveAllFilesCommand())
		{
		}
		
		/// <summary>
		/// Before a build do not clear the tasks, just save any
		/// dirty files.
		/// </summary>
		public override void BeforeBuild()
		{
			saveAllFilesCommand.SaveAllFiles();
		}
	}
}
