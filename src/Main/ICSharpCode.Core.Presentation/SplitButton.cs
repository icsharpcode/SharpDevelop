// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A button with drop-down menu.
	/// </summary>
	public class SplitButton : ButtonBase
	{
		public static readonly DependencyProperty DropDownMenuProperty
			= DependencyProperty.Register("DropDownMenu", typeof(ContextMenu),
			                              typeof(SplitButton), new FrameworkPropertyMetadata(null));
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
		protected static readonly DependencyPropertyKey IsDropDownMenuOpenPropertyKey
			= DependencyProperty.RegisterReadOnly("IsDropDownMenuOpen", typeof(bool),
			                                      typeof(SplitButton), new FrameworkPropertyMetadata(false));
		
		public static readonly DependencyProperty IsDropDownMenuOpenProperty = IsDropDownMenuOpenPropertyKey.DependencyProperty;
		
		static SplitButton()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
		}
		
		public ContextMenu DropDownMenu {
			get { return (ContextMenu)GetValue(DropDownMenuProperty); }
			set { SetValue(DropDownMenuProperty, value); }
		}
		
		public bool IsDropDownMenuOpen {
			get { return (bool)GetValue(IsDropDownMenuOpenProperty); }
			protected set { SetValue(IsDropDownMenuOpenPropertyKey, value); }
		}
		
		FrameworkElement dropDownButton;
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			dropDownButton = (FrameworkElement)Template.FindName("PART_DropDownButton", this);
		}
		
		bool IsOverDropDownButton(MouseEventArgs e)
		{
			if (dropDownButton == null)
				return false;
			return e.GetPosition(dropDownButton).X >= 0;
		}
		
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (IsOverDropDownButton(e)) {
				e.Handled = true;
				if (DropDownMenu != null) {
					DropDownMenu.Placement = PlacementMode.Bottom;
					DropDownMenu.PlacementTarget = this;
					DropDownMenu.IsOpen = true;
					DropDownMenu.Closed += DropDownMenu_Closed;
					this.IsDropDownMenuOpen = true;
				}
			} else {
				base.OnMouseLeftButtonDown(e);
			}
		}

		void DropDownMenu_Closed(object sender, RoutedEventArgs e)
		{
			((ContextMenu)sender).Closed -= DropDownMenu_Closed;
			this.IsDropDownMenuOpen = false;
		}
		
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (!IsMouseCaptured && IsOverDropDownButton(e)) {
				e.Handled = true;
			} else {
				base.OnMouseLeftButtonUp(e);
			}
		}
	}
}
