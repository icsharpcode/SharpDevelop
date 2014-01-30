// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
