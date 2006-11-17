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
			IdGuid = "{" + Guid.NewGuid().ToString() + "}";
			TypeGuid = "{00000000-0000-0000-0000-000000000000}";
		}
	}
}
