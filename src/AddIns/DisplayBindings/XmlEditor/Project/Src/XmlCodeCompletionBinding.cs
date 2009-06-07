// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace ICSharpCode.XmlEditor
{
	public class XmlCodeCompletionBinding : ICodeCompletionBinding
	{
		static XmlCodeCompletionBinding instance;
		
		public static XmlCodeCompletionBinding Instance {
			get {
				if (instance == null) {
					instance = new XmlCodeCompletionBinding();
				}
				return instance;
			}
		}
		
		public XmlCodeCompletionBinding()
		{
		}
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{			
			string text = editor.Document.GetText(0, editor.Caret.Offset);
			XmlCompletionDataProvider provider = GetProvider(editor.FileName);
			ICompletionItemList completionItems = provider.GenerateCompletionData(text, ch);
			ICompletionListWindow completionWindow = editor.ShowCompletionWindow(completionItems);
			if (completionWindow != null) {
				XmlCompletionItem item = completionItems.SuggestedItem as XmlCompletionItem;
				if (item.DataType == XmlCompletionDataType.NamespaceUri) {
					completionWindow.Width = double.NaN;
				}
			}
			
			if ((ch == '<') || (ch == ' ') || (ch == '=')) {
				return CodeCompletionKeyPressResult.Completed;
			}
			return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
		}
		
		public bool CtrlSpace(ITextEditor editor)
		{	
			// Attribute value completion.
			string text = editor.Document.Text;
			int offset = editor.Caret.Offset;
			if (XmlParser.IsInsideAttributeValue(text, offset)) {
				XmlElementPath path = XmlParser.GetActiveElementStartPath(text, offset);
				if (path.Elements.Count > 0) {
					XmlCompletionDataProvider provider = GetProvider(editor.FileName);
					editor.ShowCompletionWindow(provider.GetAttributeValueCompletionData(path, XmlParser.GetAttributeNameAtIndex(text, offset)));
					return true;
				}
			}
			return false;
		}
		
		public static XmlCompletionDataProvider GetProvider(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (PropertyService.DataDirectory != null) {
				XmlSchemaCompletionDataCollection schemas = XmlSchemaManager.SchemaCompletionDataItems;
				string defaultNamespacePrefix = XmlSchemaManager.GetNamespacePrefix(extension);
				XmlSchemaCompletionData defaultSchemaCompletionData = XmlSchemaManager.GetSchemaCompletionData(extension);
				return new XmlCompletionDataProvider(schemas, defaultSchemaCompletionData, defaultNamespacePrefix);
			}

			// for unit tests
			return new XmlCompletionDataProvider(new XmlSchemaCompletionDataCollection(), null, String.Empty);
		}
	}
}
