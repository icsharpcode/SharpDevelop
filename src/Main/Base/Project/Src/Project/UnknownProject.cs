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
	public class UnknownProject : AbstractProject
	{
		string warningText = "${res:ICSharpCode.SharpDevelop.Commands.ProjectBrowser.NoBackendForProjectType}";
		bool warningDisplayedToUser;
		
		public string WarningText {
			get { return warningText; }
			set { warningText = value; }
		}
		
		public bool WarningDisplayedToUser {
			get { return warningDisplayedToUser; }
			set { warningDisplayedToUser = value; }
		}
		
		public void ShowWarningMessageBox()
		{
			warningDisplayedToUser = true;
			MessageService.ShowError("Error loading " + this.FileName + ":\n" + warningText);
		}
		
		public UnknownProject(string fileName, string title, string warningText, bool displayWarningToUser)
			: this(fileName, title)
		{
			this.warningText = warningText;
			if (displayWarningToUser) {
				ShowWarningMessageBox();
			}
		}
		
		public UnknownProject(string fileName, string title)
		{
			Name     = title;
			FileName = fileName;
			TypeGuid = "{00000000-0000-0000-0000-000000000000}";
		}
	}
}
