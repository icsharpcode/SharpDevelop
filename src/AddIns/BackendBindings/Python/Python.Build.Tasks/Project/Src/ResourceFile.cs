// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Python.Build.Tasks
{
	/// <summary>
	/// Stores the name and filename of a resource that will be embedded by the PythonCompiler.
	/// </summary>
	public class ResourceFile
	{
		string name;
		string fileName;
		bool isPublic;
		
		public ResourceFile(string name, string fileName) : this(name, fileName, true)
		{
		}
		
		public ResourceFile(string name, string fileName, bool isPublic)
		{
			this.name = name;
			this.fileName = fileName;
			this.isPublic = isPublic;
		}
		
		public string Name {
			get { return name; }
			set { name = value; }
		}
		
		public string FileName {
			get { return fileName; }
			set { fileName = value; }
		}
		
		public bool IsPublic {
			get { return isPublic; }
			set { isPublic = value; }
		}
	}
}
