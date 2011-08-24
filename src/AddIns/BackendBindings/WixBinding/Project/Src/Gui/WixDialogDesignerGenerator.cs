// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
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
		ITextEditor textEditor;
		
		public CodeDomProvider CodeDomProvider {
			get { return new CSharpCodeProvider(); }
		}
		
		public FormsDesignerViewContent ViewContent {
			get { return view; }
		}
		
		public IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
		{
			designerCodeFile = view.PrimaryFile;
			return new [] {designerCodeFile};
		}
		
		public void Attach(FormsDesignerViewContent view)
		{
			this.view = view;
			textEditor = ((ITextEditorProvider)view.PrimaryViewContent).TextEditor;
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
			DomRegion region = GetTextEditorRegionForDialogElement(dialogId);
			if (region.IsEmpty) {
				ThrowDialogElementCouldNotBeFoundError(dialogId);
			}
			
			WixTextWriter writer = new WixTextWriter(textEditor.Options);
			WixDialogElement wixDialogElement = (WixDialogElement)dialogElement;
			string newDialogXml = wixDialogElement.GetXml(writer);
			
			WixDocumentEditor editor = new WixDocumentEditor(textEditor);
			editor.Replace(region, newDialogXml);
		}
		
		DomRegion GetTextEditorRegionForDialogElement(string dialogId)
		{
			IDocument document = view.DesignerCodeFileDocument;
			WixDocumentReader wixReader = new WixDocumentReader(document.Text);
			return wixReader.GetElementRegion("Dialog", dialogId);
		}
		
		void ThrowDialogElementCouldNotBeFoundError(string dialogId)
		{
			string messageFormat = StringParser.Parse("${res:ICSharpCode.WixBinding.DialogDesignerGenerator.DialogIdNotFoundMessage}");
			string message = String.Format(messageFormat, dialogId);
			throw new FormsDesignerLoadException(message);
		}
		
		public void MergeFormChanges(CodeCompileUnit unit)
		{
		}
		
		public void NotifyComponentRenamed(object component, string newName, string oldName)
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
