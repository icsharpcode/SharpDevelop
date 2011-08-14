// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcTextTemplateHost : IDisposable
	{
		string ViewName { get; set; }
		string ControllerName { get; set; }
		string ControllerRootName { get; set; }
		string Namespace { get; set; }
		
		bool AddActionMethods { get; set; }
		
		bool ProcessTemplate(string inputFile, string outputFile);
		
		CompilerErrorCollection Errors { get; }
	}
}
