// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.FormsDesigner
{
	public interface IDesignerLoaderProvider
	{
		DesignerLoader CreateLoader(FormsDesignerViewContent viewContent);
		
		/// <summary>
		/// Gets the source files involved when designing.
		/// </summary>
		/// <param name="viewContent"></param>
		/// <param name="designerCodeFile">The file that contains the InitializeComponents() implementation</param>
		IReadOnlyList<OpenedFile> GetSourceFiles(FormsDesignerViewContent viewContent, out OpenedFile designerCodeFile);
	}
}
