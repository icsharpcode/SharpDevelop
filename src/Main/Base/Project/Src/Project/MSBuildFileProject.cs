// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A project that is not a real project, but a MSBuild file included as project.
	/// </summary>
	public class MSBuildFileProject : AbstractProject
	{
		public MSBuildFileProject(string fileName, string title)
		{
			Name     = title;
			FileName = fileName;
			TypeGuid = "{00000000-0000-0000-0000-000000000000}";
		}
		
		public override void StartBuild(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			MSBuildEngine.StartBuild(this, options, feedbackSink, MSBuildEngine.AdditionalTargetFiles);
		}
	}
}
