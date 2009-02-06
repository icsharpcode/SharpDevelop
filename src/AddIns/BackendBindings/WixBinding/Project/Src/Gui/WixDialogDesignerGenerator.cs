// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using Microsoft.CSharp;

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
		
		public FormsDesignerViewContent ViewContent {
			get { return this.view; }
		}
		
		public IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
		{
			designerCodeFile = this.view.PrimaryFile;
			return new [] {designerCodeFile};
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
			IDocument document = view.DesignerCodeFileDocument;
			DomRegion region = WixDocument.GetElementRegion(new StringReader(document.TextContent), "Dialog", dialogId);
			if (region.IsEmpty) {
				throw new FormsDesignerLoadException(String.Format(StringParser.Parse("${res:ICSharpCode.WixBinding.DialogDesignerGenerator.DialogIdNotFoundMessage}"), dialogId));
			}
			// Get the replacement dialog xml.
			TextEditorControl textEditorControl = ((ITextEditorControlProvider)view.PrimaryViewContent).TextEditorControl;
			ITextEditorProperties properties = textEditorControl.TextEditorProperties;
			string replacementXml = WixDocument.GetXml(dialogElement, properties.LineTerminator, properties.ConvertTabsToSpaces, properties.IndentationSize);

			// Replace the xml and select the inserted text.
			WixDocumentEditor editor = new WixDocumentEditor(textEditorControl.ActiveTextAreaControl);
			editor.Replace(region, replacementXml);
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
	}
}
