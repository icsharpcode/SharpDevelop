// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	public class MissingProject : AbstractProject
	{
		public MissingProject(string fileName, string title)
		{
			Name     = title;
			FileName = fileName;
			TypeGuid = "{00000000-0000-0000-0000-000000000000}";
		}
		
		public override bool ReadOnly {
			get {
				// don't get the readonly flag from the project file - the project file does not exist.
				return true;
			}
		}
		
		protected override ProjectBehavior GetOrCreateBehavior()
		{
			// don't add behaviors from AddIn-Tree to MissingProject
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
