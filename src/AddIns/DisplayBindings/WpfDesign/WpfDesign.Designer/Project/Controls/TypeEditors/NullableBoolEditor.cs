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
		readonly static Entry[] entries = {
			new Entry(SharedInstances.BoxedTrue, bool.TrueString),
			new Entry(SharedInstances.BoxedFalse, bool.FalseString),
			new Entry(null, "Null")
		};
		
		public NullableBoolEditor(IPropertyEditorDataProperty property)
		{
			this.SelectedValuePath = "Value";
			this.ItemsSource = entries;
			SetBinding(ComboBox.SelectedValueProperty, PropertyEditorBindingHelper.CreateBinding(this, property));
		}
		
		sealed class Entry
		{
			object val;
			string description;
			
			public object Value {
				get { return val; }
			}
			
			public Entry(object val, string description)
			{
				this.val = val;
				this.description = description;
			}
			
			public override string ToString()
			{
				return description;
			}
		}
	}
}
