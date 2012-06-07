// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		protected override ProjectBehavior GetOrCreateBehavior()
		{
			// don't add behaviors from AddIn-Tree to UnknownProject
			lock (SyncRoot) {
				if (projectBehavior == null)
					projectBehavior = new DefaultProjectBehavior(this);
				return projectBehavior;
			}
		}
		
		public override bool HasProjectType(Guid projectTypeGuid)
		{
			return false;
		}
	}
}
