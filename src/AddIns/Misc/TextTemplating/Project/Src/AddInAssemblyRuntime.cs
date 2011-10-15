// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
