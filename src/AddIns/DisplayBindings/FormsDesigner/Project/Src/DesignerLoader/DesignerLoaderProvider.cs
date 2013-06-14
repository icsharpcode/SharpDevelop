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
		/// The first file in the resulting list is the main code file.
		/// </summary>
		IReadOnlyList<OpenedFile> GetSourceFiles(FormsDesignerViewContent viewContent);
	}
	/*
	public class NRefactoryDesignerLoaderProvider : IDesignerLoaderProvider
	{
		readonly SupportedLanguage language;
		
		public NRefactoryDesignerLoaderProvider(SupportedLanguage language)
		{
			this.language = language;
		}
		
		public DesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new NRefactoryDesignerLoader(language, generator);
		}
	}*/
}
