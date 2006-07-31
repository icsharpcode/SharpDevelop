// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Simplified designer generator interface that the WixDesignerLoader calls
	/// when flushing the changes.
	/// </summary>
	public interface IWixDialogDesignerGenerator
	{
		/// <summary>
		/// Passes the updated dialog element and its original id to the generator
		/// so the wix document can be updated.
		/// </summary>
		/// <remarks>The dialog id is passed since it becomes the name of the
		/// form and this can be changed from the designer.</remarks>
		void MergeFormChanges(string dialogId, XmlElement dialogElement);
	}
	
	public class WixDialogDesignerGenerator : IDesignerGenerator, IWixDialogDesignerGenerator
	{
		FormsDesignerViewContent view;
		
		public WixDialogDesignerGenerator()
		{
		}
		
		public CodeDomProvider CodeDomProvider {
			get {
				return new CSharpCodeProvider();
			}
		}
		
		public void Attach(FormsDesignerViewContent viewContent)
		{
			this.view = viewContent;
		}
		
		public void Detach()
		{
			view = null;
		}
		
		/// <summary>
		/// Merges the changes made to the wix document by overwriting the dialog element.
		/// </summary>
		void IWixDialogDesignerGenerator.MergeFormChanges(string dialogId, XmlElement dialogElement)
		{
			// Get the text region we are replacing.
			IDocument document = view.Document;
			DomRegion region = WixDocument.GetDialogElementRegion(new StringReader(document.TextContent), dialogId);
			if (region.IsEmpty) {
				throw new FormsDesignerLoadException(String.Concat("Could not find dialog id '", dialogId, "' in the document."));
			}
			ISegment segment = WixDocument.ConvertRegionToSegment(document, region);
			
			// Get the replacement dialog xml.
			string replacementXml = GetDialogXml(dialogElement, view.TextEditorControl.TextEditorProperties);
			
			// Replace the original dialog xml with the new xml and indent it.
			int originalLineCount = document.LineSegmentCollection.Count;
			document.Replace(segment.Offset, segment.Length, replacementXml.ToString());
			int addedLineCount = document.LineSegmentCollection.Count - originalLineCount;
			
			// Make sure the text inserted is visible.
			TextAreaControl textAreaControl = view.TextEditorControl.ActiveTextAreaControl;
			textAreaControl.JumpTo(region.BeginLine);

			// Indent the xml.
			int insertedCharacterCount = IndentLines(textAreaControl.TextArea, region.BeginLine + 1, region.EndLine + addedLineCount, document.FormattingStrategy);
			
			// Select the text just inserted.
			SelectText(textAreaControl.SelectionManager, document, segment.Offset, replacementXml.Length + insertedCharacterCount);
		}
		
		public void MergeFormChanges(CodeCompileUnit unit)
		{
		}
		
		public bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			file = null;
			position = 0;
			return false;
		}
		
		public ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			return new ArrayList();
		}
		
		public ICollection GetCompatibleMethods(EventInfo edesc)
		{
			return new ArrayList();
		}
		
		/// <summary>
		/// Creates formatted xml from the dialog xml element.
		/// </summary>
		/// <remarks>
		/// Need to remove the extra namespace declaration that the XmlTextWriter
		/// inserts.</remarks>
		static string GetDialogXml(XmlElement dialogElement, ITextEditorProperties properties)
		{
			StringBuilder xml = new StringBuilder();
			StringWriter stringWriter = new StringWriter(xml);
			XmlWriterSettings xmlWriterSettings = CreateXmlWriterSettings(properties);
			using (XmlWriter xmlWriter = XmlTextWriter.Create(stringWriter, xmlWriterSettings)) {
				dialogElement.WriteTo(xmlWriter);
			}
			return xml.ToString().Replace(String.Concat(" xmlns=\"", WixNamespaceManager.Namespace, "\""), String.Empty);
		}
		
		/// <summary>
		/// Creates an XmlWriterSettings based on the text editor properties.
		/// </summary>
		static XmlWriterSettings CreateXmlWriterSettings(ITextEditorProperties properties)
		{
			XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
			xmlWriterSettings.CloseOutput = true;
			xmlWriterSettings.Indent = true;
			xmlWriterSettings.NewLineChars = properties.LineTerminator;
			xmlWriterSettings.OmitXmlDeclaration = true;
			xmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
			
			if (properties.ConvertTabsToSpaces) {
				string spaces = " ";
				xmlWriterSettings.IndentChars = spaces.PadRight(properties.TabIndent);
			} else {
				xmlWriterSettings.IndentChars = "\t";
			}
			return xmlWriterSettings;
		}
		
		/// <summary>
		/// Selects the specified text range.
		/// </summary>
		static void SelectText(SelectionManager selectionManager, IDocument document, int startOffset, int length)
		{
			selectionManager.ClearSelection();
			Point selectionStart = document.OffsetToPosition(startOffset);
			Point selectionEnd = document.OffsetToPosition(startOffset + length);
			Console.WriteLine("StartOffset: " + startOffset);
			Console.WriteLine("EndOffset: " + (startOffset + length).ToString());
			selectionManager.SetSelection(selectionStart, selectionEnd);
		}
						
		/// <summary>
		/// Indents the lines and returns the total number of extra characters added.
		/// </summary>
		public static int IndentLines(TextArea textArea, int begin, int end, IFormattingStrategy formattingStrategy)
		{
			int totalInsertedCharacters = 0;
			
			int redoCount = 0;
			for (int i = begin; i <= end; ++i) {
				int existingCharacterCount = GetIndent(textArea, i);
				int insertedCharacterCount = formattingStrategy.IndentLine(textArea, i) - existingCharacterCount;
				if (insertedCharacterCount > 0) {
					++redoCount;
				}
				totalInsertedCharacters += insertedCharacterCount;
			}
			if (redoCount > 0) {
				textArea.Document.UndoStack.UndoLast(redoCount);
			}
			
			return totalInsertedCharacters;
		}
		
		/// <summary>
		/// Gets the current indentation for the specified line.
		/// </summary>
		static int GetIndent(TextArea textArea, int line)
		{			
			int indentCount = 0;
			string lineText = TextUtilities.GetLineAsString(textArea.Document, line);			
			foreach (char ch in lineText) {
				if (Char.IsWhiteSpace(ch)) {
					indentCount++;
				} else {
					break;
				}
			}
			return indentCount;
		}
	}
}
