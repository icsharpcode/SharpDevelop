// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
