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
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Derived version of the XmlTreeViewContainerControl which 
	/// allows us to override the code that shows the various dialogs
	/// that ask for user input so we can fake the data allowing us
	/// to test the class.
	/// </summary>
	public class DerivedXmlTreeViewContainerControl : XmlTreeViewContainerControl
	{
		public List<string> AddElementDialogElementNamesReturned = new List<string>();
		public DialogResult AddElementDialogResult = DialogResult.OK;
		public List<string> AddAttributeDialogAttributeNamesReturned = new List<string>();
		public DialogResult AddAttributeDialogResult = DialogResult.OK;
		public XmlSchemaCompletionCollection Schemas;
		
		public DerivedXmlTreeViewContainerControl()
			: this(new XmlSchemaCompletionCollection())
		{
		}
		
		public DerivedXmlTreeViewContainerControl(XmlSchemaCompletionCollection schemas)
			: base(schemas, null)
		{
			this.Schemas = schemas;
		}
				
		/// <summary>
		/// Allows us to call the XmlTreeViewContainerControl's 
		/// TextBoxChanged method to fake the user typing in text 
		/// into the text box.
		/// </summary>
		public void CallTextBoxTextChanged()
		{
			base.TextBoxTextChanged(this, new EventArgs());
		}
		
		/// <summary>
		/// Allows us to call the XmlTreeViewContainerControl's
		/// AttributesGridPropertyValueChanged to fake the user
		/// changing the property value. 
		/// </summary>
		public void CallAttributesGridPropertyValueChanged()
		{
			base.AttributesGridPropertyValueChanged(this, new PropertyValueChangedEventArgs(null, null));
		}
	
		/// <summary>
		/// Allows us to call the XmlTreeViewContainerControl's
		/// XmlElementTreeViewAfterSelect to fake the user selecting
		/// a tree node.
		/// </summary>
		public void CallXmlElementTreeViewAfterSelect()
		{
			base.XmlElementTreeViewAfterSelect(this, new TreeViewEventArgs(null, TreeViewAction.ByMouse));
		}
		
		/// <summary>
		/// Allows us to call the XmlTreeViewContainer's 
		/// XmlElementTreeViewDeleteKeyPressed method to fake the user 
		/// pressing the delete key in the xml tree view control.
		/// </summary>
		public void CallXmlElementTreeViewDeleteKeyPressed()
		{
			base.XmlElementTreeViewDeleteKeyPressed(this, new EventArgs());
		}
		
		/// <summary>
		/// Calls the XmlTreeViewContainer's CreateAddAttributeDialog method.
		/// </summary>
		public IAddXmlNodeDialog CallCreateAddAttributeDialog(string[] names)
		{
			return base.CreateAddAttributeDialog(names);
		}
		
		/// <summary>
		/// Calls the XmlTreeViewContainer's CreateAddElementDialog method.
		/// </summary>
		public IAddXmlNodeDialog CallCreateAddElementDialog(string[] names)
		{
			return base.CreateAddElementDialog(names);
		}
		
		/// <summary>
		/// Returns a new MockAddXmlNodeDialog for testing.
		/// </summary>
		protected override IAddXmlNodeDialog CreateAddElementDialog(string[] elementNames)
		{
			MockAddXmlNodeDialog dialog = new MockAddXmlNodeDialog();
			dialog.SetNamesToReturn(AddElementDialogElementNamesReturned.ToArray());
			dialog.SetDialogResult(AddElementDialogResult);
			return dialog;
		}
		
		/// <summary>
		/// Returns a new MockAddXmlNodeDialog for testing.
		/// </summary>
		protected override IAddXmlNodeDialog CreateAddAttributeDialog(string[] attributeNames)
		{
			MockAddXmlNodeDialog dialog = new MockAddXmlNodeDialog();
			dialog.SetNamesToReturn(AddAttributeDialogAttributeNamesReturned.ToArray());
			dialog.SetDialogResult(AddAttributeDialogResult);
			return dialog;
		}
	}
}
