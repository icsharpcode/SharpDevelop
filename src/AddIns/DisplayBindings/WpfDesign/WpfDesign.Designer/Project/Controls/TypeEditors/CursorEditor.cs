// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Controls;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors
{
	[TypeEditor(typeof(System.Windows.Input.Cursor))]
	public class CursorEditor : ComboBox
	{
		public CursorEditor(IPropertyEditorDataProperty property)
		{
			foreach (object o in property.TypeConverter.GetStandardValues()) {
				this.Items.Add(o);
			}
			SetBinding(ComboBox.SelectedItemProperty, PropertyEditorBindingHelper.CreateBinding(this, property));
		}
	}
}
