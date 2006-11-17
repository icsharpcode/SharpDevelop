// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class UnknownProject : AbstractProject
	{
		string warningText = "${res:ICSharpCode.SharpDevelop.Commands.ProjectBrowser.NoBackendForProjectType}";
		
		public string WarningText {
			get { return warningText; }
			set { warningText = value; }
		}
		
		public UnknownProject(string fileName, string title, string warningText)
			: this(fileName, title)
		{
			this.warningText = warningText;
		}
		
		public UnknownProject(string fileName, string title)
		{
			Name     = title;
			FileName = fileName;
			IdGuid = "{" + Guid.NewGuid().ToString() + "}";
			TypeGuid = "{00000000-0000-0000-0000-000000000000}";
		}
	}
}
