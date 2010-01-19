// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonImport : DefaultUsing
	{
		FromImportStatement fromImport;
		
		public PythonImport(IProjectContent projectContent, FromImportStatement fromImport)
			: base(projectContent)
		{
			this.fromImport = fromImport;
		}
		
		public bool HasIdentifier(string name)
		{
			if (String.IsNullOrEmpty(name)) {
				return false;
			}
			
			for (int i = 0; i < fromImport.Names.Count; ++i) {
				string identifier = GetImportedAsNameIfExists(i);
				if (identifier == name) {
					return true;
				}
			}
			return false;
		}
		
		string GetImportedAsNameIfExists(int index)
		{
			string identifier = fromImport.AsNames[index];
			if (identifier != null) {
				return identifier;
			}
			return fromImport.Names[index];
		}
		
		public string Module {
			get { return fromImport.Root.MakeString(); }
		}
		
		public string GetIdentifierForAlias(string alias)
		{
			int index = fromImport.AsNames.IndexOf(alias);
			if (index >= 0) {
				return fromImport.Names[index];
			}
			return null;
		}
	}
}
