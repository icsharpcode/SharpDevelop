// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Description of XmlCodeCompletionBinding.
	/// </summary>
	public class XmlCodeCompletionBinding : ICodeCompletionBinding
	{
		static XmlCodeCompletionBinding instance;
		
		public static XmlCodeCompletionBinding Instance {
			get {
				if (instance == null)
					instance = new XmlCodeCompletionBinding();
				
				return instance;
			}
		}
		
		public XmlCodeCompletionBinding()
		{
		}
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			string text = string.Concat(editor.Document.GetText(0, editor.Caret.Offset), ch);
			string extension = Path.GetExtension(editor.FileName);
			string defaultNamespacePrefix = XmlSchemaManager.GetNamespacePrefix(extension);
			XmlSchemaCompletionData defaultSchemaCompletionData = XmlSchemaManager.GetSchemaCompletionData(extension);
			XmlCompletionDataProvider provider = new XmlCompletionDataProvider(XmlSchemaManager.SchemaCompletionDataItems,
			                                                                   defaultSchemaCompletionData,
			                                                                   defaultNamespacePrefix);
			
			editor.ShowCompletionWindow(provider.GenerateCompletionData(text, ch));
			
			if (ch == '<' || ch == ' ' || ch == '=')
				return CodeCompletionKeyPressResult.Completed;
			else
				return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{
			string text = editor.Document.Text;
			int offset = editor.Caret.Offset;
			
			string extension = Path.GetExtension(editor.FileName);
			string defaultNamespacePrefix = XmlSchemaManager.GetNamespacePrefix(extension);
			XmlSchemaCompletionData defaultSchemaCompletionData = XmlSchemaManager.GetSchemaCompletionData(extension);
			XmlCompletionDataProvider provider = new XmlCompletionDataProvider(XmlSchemaManager.SchemaCompletionDataItems,
			                                                                   defaultSchemaCompletionData,
			                                                                   defaultNamespacePrefix);
			
			// Attribute value completion.
			if (XmlParser.IsInsideAttributeValue(text, offset)) {
				XmlElementPath path = XmlParser.GetActiveElementStartPath(text, offset);
				if (path.Elements.Count > 0) {
					editor.ShowCompletionWindow(provider.GetAttributeValueCompletionData(path, XmlParser.GetAttributeNameAtIndex(text, offset)));
					return true;
				}
			}
			
			return false;
		}
	}
}
