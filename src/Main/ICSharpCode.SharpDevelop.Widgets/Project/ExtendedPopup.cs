// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
