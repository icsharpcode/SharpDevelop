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
using System.Linq;

namespace ICSharpCode.TextTemplating
{
	public class AddInAssemblyRuntime
	{
		string fileName;
		IAddIn addIn;
		IAddInRuntime runtime;
		
		public AddInAssemblyRuntime(IAddIn addIn)
		{
			this.addIn = addIn;
			GetFileName();
			GetRuntime();
		}
		
		void GetFileName()
		{
			string fileNameWithoutExtension = GetFileNameWithoutExtension();
			fileName = fileNameWithoutExtension + ".dll";
		}
		
		string GetFileNameWithoutExtension()
		{
			string id = addIn.PrimaryIdentity;
			int index = id.IndexOf(".", StringComparison.OrdinalIgnoreCase);
			return id.Substring(index + 1);
		}
		
		public string FileName {
			get { return fileName; }
		}
		
		void GetRuntime()
		{
			runtime = addIn
				.GetRuntimes()
				.SingleOrDefault(r => Matches(r.Assembly));
		}
		
		public bool Matches(string runtimeFileName)
		{
			return CompareIgnoringCase(runtimeFileName, this.fileName);
		}
		
		bool CompareIgnoringCase(string a, string b)
		{
			return String.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
		
		public IAddInRuntime Runtime {
			get { return runtime; }
		}
	}
}
