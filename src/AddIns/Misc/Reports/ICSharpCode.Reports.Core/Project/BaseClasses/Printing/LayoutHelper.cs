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
using System.Drawing;
using ICSharpCode.Reports.Core.Globals;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.BaseClasses.Printing
{
	
	internal sealed class LayoutHelper
	{
		public static Rectangle CalculateSectionLayout(Graphics graphics,BaseSection section)
		{
			ILayouter layouter = (ILayouter)ServiceContainer.GetService(typeof(ILayouter));
			var desiredRectangle = layouter.Layout(graphics, section);
			return desiredRectangle;
		}
		
		
		public static void FixSectionLayout(Rectangle desiredRectangle, BaseSection section)
		{
			Rectangle sectionRectangle = new Rectangle(section.Location, section.Size);
			if (!sectionRectangle.Contains(desiredRectangle)) {
				section.Size = new Size(section.Size.Width, 
				                        desiredRectangle.Size.Height + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);
			}
		}
		
		
		
		public static void SetLayoutForRow (Graphics graphics, ILayouter layouter,ISimpleContainer row)
		{
			Console.WriteLine("SetLayoutForRow");
			Rectangle textRect = layouter.Layout(graphics,row);
			if (textRect.Height > row.Size.Height) {
				row.Size = new Size(row.Size.Width,textRect.Height + 5);
			}
		}
		
	}
}
