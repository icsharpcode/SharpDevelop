// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
