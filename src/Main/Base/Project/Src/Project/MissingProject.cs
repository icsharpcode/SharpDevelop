// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
	}
}
