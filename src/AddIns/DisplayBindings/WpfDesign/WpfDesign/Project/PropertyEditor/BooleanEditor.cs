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

namespace ICSharpCode.WpfDesign.PropertyEditor
{
	/// <summary>
	/// Type editor used to edit bool properties.
	/// </summary>
	sealed class BooleanEditor : CheckBox
	{
		/// <summary>
		/// Creates a new BooleanEditor instance.
		/// </summary>
		public BooleanEditor(IPropertyEditorDataProperty property)
		{
			SetBinding(IsCheckedProperty, PropertyEditorBindingHelper.CreateBinding(this, property));
		}
	}
}
