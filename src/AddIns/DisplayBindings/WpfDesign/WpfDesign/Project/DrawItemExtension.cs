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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace ICSharpCode.WpfDesign
{
	/// <summary>
	/// Behavior interface implemented by container elements to support resizing
	/// drawing new Elements
	/// </summary>
	public interface IDrawItemExtension
	{
		/// <summary>
		/// Returns if the specified type can be drawn.
		/// </summary>
		/// <param name="createItemType">The type to check.</param>
		/// <returns>True if the specified type can be drawn, otherwise false.</returns>
		bool CanItemBeDrawn(Type createItemType);
		
		/// <summary>
		/// Starts to draw.
		/// </summary>
		/// <param name="clickedOn">The item.</param>
		/// <param name="createItemType">The item type.</param>
		/// <param name="panel">The design panel to draw on.</param>
		/// <param name="e">The <see cref="MouseEventArgs"/> argument that initiated the draw operation.</param>
		void StartDrawItem(DesignItem clickedOn, Type createItemType, IDesignPanel panel, MouseEventArgs e);
	}
}
