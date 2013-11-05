// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.FormsDesigner;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
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
	
	public class WixDialogDesignerGenerator : IWixDialogDesignerGenerator
	{
		readonly IWixDialogDesigner view;
		
		public WixDialogDesignerGenerator(IWixDialogDesigner view)
		{
			this.view = view;
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
			
			var textEditor = view.PrimaryViewContentTextEditor;
			WixTextWriter writer = new WixTextWriter(textEditor.Options);
			WixDialogElement wixDialogElement = (WixDialogElement)dialogElement;
			string newDialogXml = wixDialogElement.GetXml(writer);
			
			WixDocumentEditor editor = new WixDocumentEditor(textEditor);
			editor.Replace(region, newDialogXml);
		}
		
		DomRegion GetTextEditorRegionForDialogElement(string dialogId)
		{
			WixDocumentReader wixReader = new WixDocumentReader(view.GetDocumentXml());
			return wixReader.GetElementRegion("Dialog", dialogId);
		}
		
		void ThrowDialogElementCouldNotBeFoundError(string dialogId)
		{
			string messageFormat = StringParser.Parse("${res:ICSharpCode.WixBinding.DialogDesignerGenerator.DialogIdNotFoundMessage}");
			string message = String.Format(messageFormat, dialogId);
			throw new FormsDesignerLoadException(message);
		}
	}
}
