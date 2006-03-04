// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	public class MissingProject : AbstractProject
	{
		string typeGuid = "{00000000-0000-0000-0000-000000000000}";
		
		public override string TypeGuid {
			get {
				return typeGuid;
			}
			set {
				typeGuid = value;
			}
		}
		
		public MissingProject(string fileName, string title)
		{
			Name     = title;
			FileName = fileName;
			IdGuid = "{" + Guid.NewGuid().ToString() + "}";
		}
	}
}
