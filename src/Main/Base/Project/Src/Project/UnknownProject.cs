using System;
using System.IO;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
	public class UnknownProject : AbstractProject
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
		
		public UnknownProject(string fileName)
		{
			Name     = Path.GetFileNameWithoutExtension(fileName);
			FileName = fileName;
		}
	}
}
