// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

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
			
			switch (ch) {
				case '=':
					// Namespace completion.
					if (XmlParser.IsNamespaceDeclaration(text, text.Length)) {
						editor.ShowCompletionWindow(XmlSchemaManager.SchemaCompletionDataItems.GetNamespaceCompletionData());
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case '<':
					// Child element completion.
					XmlElementPath parentPath = XmlParser.GetParentElementPath(text);
					if (parentPath.Elements.Count > 0) {
						editor.ShowCompletionWindow(provider.GetChildElementCompletionData(parentPath));
						return CodeCompletionKeyPressResult.Completed;
					} else if (defaultSchemaCompletionData != null) {
						editor.ShowCompletionWindow(defaultSchemaCompletionData.GetElementCompletionData(defaultNamespacePrefix));
						return CodeCompletionKeyPressResult.Completed;
					}
					break;
				case ' ':
					// Attribute completion.
					if (!XmlParser.IsInsideAttributeValue(text, text.Length)) {
						XmlElementPath path = XmlParser.GetActiveElementStartPath(text, text.Length);
						if (path.Elements.Count > 0) {
							editor.ShowCompletionWindow(provider.GetAttributeCompletionData(path));
							return CodeCompletionKeyPressResult.Completed;
						}
					}
					break;
				default:
					// Attribute value completion.
					if (XmlParser.IsAttributeValueChar(ch)) {
						string attributeName = XmlParser.GetAttributeName(text, text.Length);
						if (attributeName.Length > 0) {
							XmlElementPath elementPath = XmlParser.GetActiveElementStartPath(text, text.Length);
							if (elementPath.Elements.Count > 0) {
								editor.ShowCompletionWindow(provider.GetAttributeValueCompletionData(elementPath, attributeName));
								return CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion;
							}
						}
					}
					break;
			}
			
			return CodeCompletionKeyPressResult.None;
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
