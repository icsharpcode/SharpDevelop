// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Type editor used to edit enum properties.
	/// </summary>
	sealed class StandardValuesComboBoxEditor : ComboBox
	{
		/// <summary>
		/// Creates a new EnumEditor instance.
		/// </summary>
		public StandardValuesComboBoxEditor(IPropertyEditorDataProperty property)
		{
			foreach (object o in property.TypeConverter.GetStandardValues()) {
				this.Items.Add(o);
			}
			SetBinding(ComboBox.SelectedItemProperty, PropertyEditorBindingHelper.CreateBinding(this, property));
		}
	}
}
