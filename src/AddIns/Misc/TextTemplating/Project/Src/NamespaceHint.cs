// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class NamespaceHint
	{
		string hint = String.Empty;
		FileProjectItem templateFile;
		
		public NamespaceHint(FileProjectItem templateFile)
		{
			this.templateFile = templateFile;
			GetNamespaceHint();
		}
		
		void GetNamespaceHint()
		{
			hint = GetCustomToolNamespace();
			if (String.IsNullOrEmpty(hint)) {
				hint = GetProjectRootNamespace();
			}
		}
		
		string GetProjectRootNamespace()
		{
			return templateFile.Project.RootNamespace;
		}

		string GetCustomToolNamespace()
		{
			return templateFile.GetEvaluatedMetadata("CustomToolNamespace");
		}
		
		public override string ToString()
		{
			return hint;
		}
	}
}
