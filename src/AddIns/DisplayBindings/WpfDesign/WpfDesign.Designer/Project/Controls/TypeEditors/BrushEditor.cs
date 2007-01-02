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
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Controls.TypeEditors
{
	/// <summary>
	/// Type editor used to edit Brush properties.
	/// </summary>
	[TypeEditor(typeof(Brush))]
	public sealed class BrushEditor : Border
	{
		/// <summary>
		/// Creates a new BooleanEditor instance.
		/// </summary>
		public BrushEditor(IPropertyEditorDataProperty property)
		{
			this.BorderBrush = Brushes.Black;
			this.BorderThickness = new Thickness(1);
			SetBinding(BackgroundProperty, PropertyEditorBindingHelper.CreateBinding(this, property));
		}
	}
}
