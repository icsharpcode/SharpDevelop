// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
	/// A button that is split into two parts: the left part works like a normal button, the right part opens a drop-down menu when it is clicked.
	/// </summary>
	public class SplitButton : ButtonBase
	{
		public static readonly DependencyProperty DropDownMenuProperty
			= DropDownButton.DropDownMenuProperty.AddOwner(typeof(SplitButton));
		
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
		
		FrameworkElement dropDownArrow;
		
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			dropDownArrow = (FrameworkElement)Template.FindName("PART_DropDownArrow", this);
		}
		
		bool IsOverDropDownArrow(MouseEventArgs e)
		{
			if (dropDownArrow == null)
				return false;
			return e.GetPosition(dropDownArrow).X >= 0;
		}
		
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (IsOverDropDownArrow(e)) {
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
			if (!IsMouseCaptured && IsOverDropDownArrow(e)) {
				e.Handled = true;
			} else {
				base.OnMouseLeftButtonUp(e);
			}
		}
	}
}
