// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlBinding.Parser;
using ICSharpCode.XmlEditor;
using System.IO;

namespace ICSharpCode.XmlBinding
{
	/// <summary>
	/// Description of XmlCodeCompletionBinding.
	/// </summary>
	public class XmlCodeCompletionBinding : ICodeCompletionBinding
	{
		public XmlCodeCompletionBinding()
		{
		}
		
		public CodeCompletionKeyPressResult HandleKeyPress(ITextEditor editor, char ch)
		{
			string text = String.Concat(editor.Document.GetText(0, editor.Caret.Offset), ch);
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
			return false;
		}
	}
}
