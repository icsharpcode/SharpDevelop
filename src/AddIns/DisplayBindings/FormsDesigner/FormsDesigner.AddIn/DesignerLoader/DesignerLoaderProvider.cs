// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.NRefactory;

namespace ICSharpCode.FormsDesigner
{
	public interface IDesignerLoaderProvider
	{
		IDesignerLoader CreateLoader(IDesignerGenerator generator);
		FormsDesignerViewContent ViewContent { get; set; }
	}
	
	public class NRefactoryDesignerLoaderProvider : IDesignerLoaderProvider
	{
		readonly SupportedLanguage language;
		FormsDesignerViewContent viewContent;
		
		public FormsDesignerViewContent ViewContent { get; set; }
		
		public NRefactoryDesignerLoaderProvider(SupportedLanguage language)
		{
			this.language = language;
		}
		
		public IDesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new NRefactoryDesignerLoader(language, generator, ViewContent);
		}
	}
}