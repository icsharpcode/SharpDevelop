// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public interface IMvcTextTemplateHost : IDisposable
	{
		string Namespace { get; set; }
		
		// Add View properties.
		string ViewName { get; set; }
		bool IsPartialView { get; set; }
		string ViewDataTypeName { get; set; }
		string ViewDataTypeAssemblyLocation { get; set; }
		bool IsContentPage { get; set; }
		string MasterPageFile { get; set; }
		string PrimaryContentPlaceHolderID { get; set; }
		
		// Add Controller properties.
		string ControllerName { get; set; }
		string ControllerRootName { get; set; }
		bool AddActionMethods { get; set; }
		
		bool ProcessTemplate(string inputFile, string outputFile);
		
		CompilerErrorCollection Errors { get; }
	}
}
