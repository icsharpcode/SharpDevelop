// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Compiler.Ast;

namespace ICSharpCode.PythonBinding
{
	public class PythonImport : DefaultUsing
	{
		ImportStatement importStatement;
		
		public PythonImport(IProjectContent projectContent, ImportStatement importStatement)
			: base(projectContent)
		{
			this.importStatement = importStatement;
			AddUsings();
		}
		
		void AddUsings()
		{
			for (int i = 0; i < importStatement.Names.Count; ++i) {
				string name = GetImportedAsNameIfExists(i);
				Usings.Add(name);
			}
		}
		
		string GetImportedAsNameIfExists(int index)
		{
			string name = importStatement.AsNames[index];
			if (name != null) {
				return name;
			}
			return importStatement.Names[index].MakeString();
		}
		
		public string Module {
			get { return importStatement.Names[0].MakeString(); }
		}
		
		public string GetOriginalNameForAlias(string alias)
		{
			int index = importStatement.AsNames.IndexOf(alias);
			if (index >= 0) {
				return importStatement.Names[index].MakeString();
			}
			return null;
		}
	}
}
