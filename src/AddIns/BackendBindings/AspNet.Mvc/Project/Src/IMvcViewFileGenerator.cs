// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcViewFileGenerator
	{
		IMvcProject Project { get; set; }
		string ModelClassName { get; set; }
		string ModelClassAssemblyLocation { get; set; }
		bool IsContentPage { get; set; }
		string MasterPageFile { get; set; }
		string PrimaryContentPlaceHolderId { get; set; }
		MvcViewTextTemplate Template { get; set; }
		CompilerErrorCollection Errors { get; }
		bool HasErrors { get; }
		
		void GenerateFile(MvcViewFileName fileName);
	}
}
