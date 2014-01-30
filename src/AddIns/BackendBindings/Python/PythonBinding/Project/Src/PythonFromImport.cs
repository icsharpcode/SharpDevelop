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
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonFromImport : DefaultUsing
	{
		FromImportStatement fromImport;
		
		public PythonFromImport(IProjectContent projectContent, FromImportStatement fromImport)
			: base(projectContent)
		{
			this.fromImport = fromImport;
		}
		
		public bool IsImportedName(string name)
		{
			if (String.IsNullOrEmpty(name)) {
				return false;
			}
			
			for (int i = 0; i < fromImport.Names.Count; ++i) {
				string importedName = GetImportedAsNameIfExists(i);
				if (importedName == name) {
					return true;
				}
			}
			return false;
		}
		
		string GetImportedAsNameIfExists(int index)
		{
			if (fromImport.AsNames != null) {
				string importedAsName = fromImport.AsNames[index];
				if (importedAsName != null) {
					return importedAsName;
				}
			}
			return fromImport.Names[index];
		}
		
		public string Module {
			get { return fromImport.Root.MakeString(); }
		}
		
		public string GetOriginalNameForAlias(string alias)
		{
			if (fromImport.AsNames == null) {
				return null;
			}
			
			int index = fromImport.AsNames.IndexOf(alias);
			if (index >= 0) {
				return fromImport.Names[index];
			}
			return null;
		}
		
		public bool ImportsEverything {
			get {
				return fromImport.Names[0] == "*";
			}
		}
	}
}
