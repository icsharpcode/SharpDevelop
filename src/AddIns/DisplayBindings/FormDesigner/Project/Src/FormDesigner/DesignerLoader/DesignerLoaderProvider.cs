/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 16.08.2005
 * Time: 21:40
 */

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.TextEditor;
using ICSharpCode.NRefactory.Parser;

namespace ICSharpCode.FormDesigner
{
	public interface IDesignerLoaderProvider
	{
		DesignerLoader CreateLoader();
	}
	
	public class NRefactoryDesignerLoaderProvider : IDesignerLoaderProvider
	{
		SupportedLanguages language;
		TextEditorControl textEditorControl;
		
		public NRefactoryDesignerLoaderProvider(SupportedLanguages language, TextEditorControl textEditorControl)
		{
			this.language = language;
			this.textEditorControl = textEditorControl;
		}
		
		public DesignerLoader CreateLoader()
		{
			return new NRefactoryDesignerLoader(language, textEditorControl);
		}
	}
	
	public class XmlDesignerLoaderProvider : IDesignerLoaderProvider
	{
		TextEditorControl textEditorControl;
		
		public XmlDesignerLoaderProvider(TextEditorControl textEditorControl)
		{
			this.textEditorControl = textEditorControl;
		}
		
		public DesignerLoader CreateLoader()
		{
			return new XmlDesignerLoader(textEditorControl);
		}
	}
}
