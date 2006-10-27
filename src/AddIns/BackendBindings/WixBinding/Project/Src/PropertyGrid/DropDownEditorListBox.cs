// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace ICSharpCode.WixBinding
{
	public class DropDownEditorListBox : ListBox
	{
		IWindowsFormsEditorService editorService;
		string dropDownValue = String.Empty;
		WixXmlAttributePropertyDescriptor propertyDescriptor;
		
		public DropDownEditorListBox(ITypeDescriptorContext context, IWindowsFormsEditorService editorService)
		{
			this.editorService = editorService;
			BorderStyle = BorderStyle.None;
			if (context != null) {
				propertyDescriptor = context.PropertyDescriptor as WixXmlAttributePropertyDescriptor;
			}
			AddDropDownItems();
		}
		
		public string Value {
			get {
				return dropDownValue;
			}
			set {
				dropDownValue = value;
				SelectListItem(dropDownValue);
			}
		}
		
		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			int index = IndexFromPoint(e.Location);
			if (index != -1) {
				dropDownValue = (string)SelectedItem;
				editorService.CloseDropDown();
			}
		}
		
		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (e.KeyData == Keys.Return) {
				if (SelectedIndex != -1) {
					dropDownValue = (string)SelectedItem;
				}
				editorService.CloseDropDown();
			}
		}
		
		void AddDropDownItems()
		{
			if (propertyDescriptor != null && propertyDescriptor.WixXmlAttribute.HasValues) {
				foreach (string item in propertyDescriptor.WixXmlAttribute.Values) {
					Items.Add(item);
				}
			}
		}
		
		void SelectListItem(string item)
		{
			int index = Items.IndexOf(item);
			SelectedIndex = index;
		}
	}
}
