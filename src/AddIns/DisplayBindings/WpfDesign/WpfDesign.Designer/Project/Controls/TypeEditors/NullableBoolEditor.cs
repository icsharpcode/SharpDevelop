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
	[TypeEditor(typeof(bool?))]
	public class NullableBoolEditor : ComboBox
	{
		const string NullString = "Null";
		
		public NullableBoolEditor(IPropertyEditorDataProperty property)
		{
			// the UI must show the difference between an ambiguous property value
			// and null, so we use a combo box bound to a string property
			property = new NullableBoolToStringProperty(property);
			this.ItemsSource = new string[] { NullString, false.ToString(), true.ToString() };
			SetBinding(ComboBox.SelectedItemProperty, PropertyEditorBindingHelper.CreateBinding(this, property));
		}
		
		/// <summary>
		/// views a bool? property as string property
		/// </summary>
		sealed class NullableBoolToStringProperty : ProxyPropertyEditorDataProperty
		{
			public NullableBoolToStringProperty(IPropertyEditorDataProperty data)
				: base(data)
			{
			}
			
			public override object Value {
				get {
					object v = base.Value;
					if (v == null)
						return IsAmbiguous ? null : NullString;
					else
						return v.ToString();
				}
				set {
					string v = (string)value;
					if (v == NullString)
						base.Value = null;
					else
						base.Value = SharedInstances.Box(bool.Parse(v));
				}
			}
		}
	}
}
