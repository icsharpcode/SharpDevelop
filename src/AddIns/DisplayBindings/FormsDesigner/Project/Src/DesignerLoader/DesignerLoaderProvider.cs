// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.NRefactory;
using ICSharpCode.TextEditor;

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
	
	public class XmlDesignerLoaderProvider : IDesignerLoaderProvider
	{
		public XmlDesignerLoaderProvider()
		{
		}
		
		public DesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new XmlDesignerLoader(generator);
		}
	}
}
