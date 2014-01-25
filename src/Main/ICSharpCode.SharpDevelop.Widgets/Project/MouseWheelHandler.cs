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
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Widgets
{
	/// <summary>
	/// Accumulates mouse wheel deltas and reports the actual number of lines to scroll.
	/// </summary>
	public sealed class MouseWheelHandler
	{
		// CODE DUPLICATION: See ICSharpCode.TextEditor.Util.MouseWheelHandler
		
		const int WHEEL_DELTA = 120;
		
		int mouseWheelDelta;
		
		public int GetScrollAmount(MouseEventArgs e)
		{
			// accumulate the delta to support high-resolution mice
			mouseWheelDelta += e.Delta;
			Console.WriteLine("Mouse rotation: new delta=" + e.Delta + ", total delta=" + mouseWheelDelta);
			
			int linesPerClick = Math.Max(SystemInformation.MouseWheelScrollLines, 1);
			
			int scrollDistance = mouseWheelDelta * linesPerClick / WHEEL_DELTA;
			mouseWheelDelta %= Math.Max(1, WHEEL_DELTA / linesPerClick);
			return scrollDistance;
		}
		
		public void Scroll(ScrollBar scrollBar, MouseEventArgs e)
		{
			int newvalue = scrollBar.Value - GetScrollAmount(e) * scrollBar.SmallChange;
			scrollBar.Value = Math.Max(scrollBar.Minimum, Math.Min(scrollBar.Maximum - scrollBar.LargeChange + 1, newvalue));
		}
	}
}
