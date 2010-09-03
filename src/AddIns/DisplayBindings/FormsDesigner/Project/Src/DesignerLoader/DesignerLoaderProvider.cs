// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.NRefactory;

namespace ICSharpCode.FormsDesigner
{
	public interface IDesignerLoaderProvider
	{
		DesignerLoader CreateLoader(IDesignerGenerator generator);
	}
	
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
	}
}
