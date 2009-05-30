// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2229 $</version>
// </file>

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
		List<string> addElementDialogElementNamesReturned = new List<string>();
		DialogResult addElementDialogResult = DialogResult.OK;
		List<string> addAttributeDialogAttributeNamesReturned = new List<string>();
		DialogResult addAttributeDialogResult = DialogResult.OK;
		
		/// <summary>
		/// This is the list of element names that will be returned from
		/// the mock AddElementDialog.
		/// </summary>
		public List<string> AddElementDialogElementNamesReturned {
			get {
				return addElementDialogElementNamesReturned;
			}
		}
		
		/// <summary>
		/// Gets or sets the dialog result for the AddElementDialog.
		/// </summary>
		public DialogResult AddElementDialogResult {
			get {
				return addElementDialogResult;
			}
			set {
				addElementDialogResult = value;
			}
		}
		
		/// <summary>
		/// Gets the list of attribute names that will be returned
		/// from the mock AddAttributeDialog.
		/// </summary>
		public List<string> AddAttributeDialogAttributeNamesReturned {
			get {
				return addAttributeDialogAttributeNamesReturned;
			}
		}
		
		/// <summary>
		/// Gets or sets the dialog result for the AddAttributeDialog.
		/// </summary>
		public DialogResult AddAttributeDialogResult {
			get {
				return addAttributeDialogResult;
			}
			set {
				addAttributeDialogResult = value;
			}
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
			dialog.SetNamesToReturn(addElementDialogElementNamesReturned.ToArray());
			dialog.SetDialogResult(addElementDialogResult);
			return dialog;
		}
		
		/// <summary>
		/// Returns a new MockAddXmlNodeDialog for testing.
		/// </summary>
		protected override IAddXmlNodeDialog CreateAddAttributeDialog(string[] attributeNames)
		{
			MockAddXmlNodeDialog dialog = new MockAddXmlNodeDialog();
			dialog.SetNamesToReturn(addAttributeDialogAttributeNamesReturned.ToArray());
			dialog.SetDialogResult(addAttributeDialogResult);
			return dialog;
		}
	}
}
