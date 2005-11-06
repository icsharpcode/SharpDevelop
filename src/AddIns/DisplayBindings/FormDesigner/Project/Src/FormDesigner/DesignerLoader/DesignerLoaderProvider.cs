// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.TextEditor;
using ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.FormDesigner
{
	public interface IDesignerLoaderProvider
	{
		DesignerLoader CreateLoader(IDesignerGenerator generator);
	}
	
	public class NRefactoryDesignerLoaderProvider : IDesignerLoaderProvider
	{
		SupportedLanguage language;
		TextEditorControl textEditorControl;
		
		public NRefactoryDesignerLoaderProvider(SupportedLanguage language, TextEditorControl textEditorControl)
		{
			this.language = language;
			this.textEditorControl = textEditorControl;
		}
		
		public DesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new NRefactoryDesignerLoader(language, textEditorControl, generator);
		}
	}
	
	public class XmlDesignerLoaderProvider : IDesignerLoaderProvider
	{
		TextEditorControl textEditorControl;
		
		public XmlDesignerLoaderProvider(TextEditorControl textEditorControl)
		{
			this.textEditorControl = textEditorControl;
		}
		
		public DesignerLoader CreateLoader(IDesignerGenerator generator)
		{
			return new XmlDesignerLoader(textEditorControl, generator);
		}
	}
}
