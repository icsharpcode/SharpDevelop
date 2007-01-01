// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ICSharpCode.WpfDesign.PropertyEditor;

namespace ICSharpCode.WpfDesign.Designer.Controls
{
	/// <summary>
	/// The "dot" button appearing after dependency properties to specified advanced values like data bindings.
	/// </summary>
	public class DependencyPropertyDotButton : ButtonBase
	{
		/*
		/// <summary>
		/// Dependency property for <see cref="DataProperty"/>.
		/// </summary>
		public static readonly DependencyProperty DataPropertyProperty
			= DependencyProperty.Register("DataProperty", typeof(IPropertyEditorDataProperty), typeof(DependencyPropertyDotButton));
		 */
		
		/// <summary>
		/// Dependency property for <see cref="Checked"/>.
		/// </summary>
		public static readonly DependencyProperty CheckedProperty
			= DependencyProperty.Register("Checked", typeof(bool), typeof(DependencyPropertyDotButton),
			                              new FrameworkPropertyMetadata(false));
		
		
		static DependencyPropertyDotButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DependencyPropertyDotButton), new FrameworkPropertyMetadata(typeof(DependencyPropertyDotButton)));
		}
		
		/*
		/// <summary>
		/// Gets/Sets the property the button is used for.
		/// </summary>
		public IPropertyEditorDataProperty DataProperty {
			get { return (IPropertyEditorDataProperty)GetValue(DataPropertyProperty); }
			set { SetValue(DataPropertyProperty, value); }
		}
		 */
		
		/// <summary>
		/// Gets/Sets if the button looks checked.
		/// </summary>
		public bool Checked {
			get { return (bool)GetValue(CheckedProperty); }
			set { SetValue(CheckedProperty, value); }
		}
		
		/// <summary>
		/// Fires the Click event and opens the context menu.
		/// </summary>
		protected override void OnClick()
		{
			base.OnClick();
			if (ContextMenu != null) {
				ContextMenu.IsOpen = true;
			}
		}
	}
}
