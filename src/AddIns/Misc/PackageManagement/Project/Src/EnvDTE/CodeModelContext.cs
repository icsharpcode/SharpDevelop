// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class CodeModelContext
	{
		public IProject CurrentProject { get; set; }
		public IDocumentLoader DocumentLoader { get; set; }
		public CodeGenerator CodeGenerator { get; set; }
		
		/// <summary>
		/// Specifies the file name if this code model context refers to
		/// </summary>
		public string FilteredFileName { get; set; }
		
		public CodeModelContext WithFilteredFileName(string fileName)
		{
			CodeModelContext newContext = (CodeModelContext)MemberwiseClone();
			newContext.FilteredFileName = fileName;
			return newContext;
		}
	}
}
