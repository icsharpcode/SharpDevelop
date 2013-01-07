// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ICSharpCode.SharpDevelop.Widgets
{
	/// <summary>
	/// A popup that is only visible while the parent control or the popup itself has the keyboard focus.
	/// </summary>
	public class ExtendedPopup : Popup
	{
		readonly UIElement parent;
		
		public ExtendedPopup(UIElement parent)
		{
			if (parent == null)
				throw new ArgumentNullException("parent");
			this.parent = parent;
		}
		
		/// <summary>
		/// Gets whether the popup is currently visible.
		/// </summary>
		public new bool IsOpen {
			get { return base.IsOpen; }
			// Prevent consumers from accessing the setter directly
		}
		
		bool openIfFocused;
		
		public bool IsOpenIfFocused {
			get { return openIfFocused; }
			set {
				if (openIfFocused != value) {
					openIfFocused = value;
					if (value) {
						parent.IsKeyboardFocusedChanged += parent_IsKeyboardFocusedChanged;
					} else {
						parent.IsKeyboardFocusedChanged -= parent_IsKeyboardFocusedChanged;
					}
					OpenOrClose();
				}
			}
		}
		
		void parent_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			OpenOrClose();
		}
		
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
		{
			base.OnIsKeyboardFocusWithinChanged(e);
			OpenOrClose();
		}
		
		void OpenOrClose()
		{
			bool newIsOpen = openIfFocused && (parent.IsKeyboardFocused || this.IsKeyboardFocusWithin);
			base.IsOpen = newIsOpen;
		}
	}
}
