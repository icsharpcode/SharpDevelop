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
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.BaseClasses.Printing
{
	/// <summary>
	/// Description of Layouter.
	/// </summary>
	public class Layouter:ILayouter
	{

		public Rectangle Layout(Graphics graphics,ISimpleContainer container)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (container == null) {
				throw new ArgumentNullException("container");
			}
			
			if (container.Items == null)
			{
				return Rectangle.Empty;
			}
			
			Rectangle desiredContainerRectangle = new Rectangle (container.Location,container.Size);
			
			System.Collections.Generic.IEnumerable<BaseReportItem> canGrowShrinkCollection = from bt in container.Items where bt.CanGrow == true select bt;
			
			if (canGrowShrinkCollection.Count() > 0 ) {
				Rectangle surroundingRec = FindSurroundingRectangle(graphics,canGrowShrinkCollection);
				desiredContainerRectangle = new Rectangle(container.Location.X,
				                                          container.Location.Y,
				                                          container.Size.Width,
				                                          surroundingRec.Size.Height);
				
			}
			return desiredContainerRectangle;
		}
		
		
		public Rectangle Layout(Graphics graphics,BaseSection section)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			
			IEnumerable<BaseReportItem> canGrowShrinkCollection = from bt in section.Items where bt.CanGrow == true select bt;
			
			Rectangle desiredSectionRectangle = new Rectangle(section.Location.X,
			                                                  section .Location.Y,
			                                                  section .Size.Width,
			                                                  section.Size.Height);
			
			if (canGrowShrinkCollection.Count() > 0) {
				
				Rectangle surroundingRec = FindSurroundingRectangle(graphics,canGrowShrinkCollection);
				
				if (surroundingRec.Height > desiredSectionRectangle .Height) {
					
					desiredSectionRectangle = new Rectangle(section.Location.X,
					                                        section .Location.Y,
					                                        section .Size.Width,
					                                        surroundingRec.Size.Height);
				}
			}
			return desiredSectionRectangle;
		}
		
		
		
		private static Rectangle FindSurroundingRectangle (Graphics graphics,IEnumerable<BaseReportItem> canGrowShrinkCollection)
		{
			int top = int.MaxValue;
			
			Rectangle rec = new Rectangle(canGrowShrinkCollection.First().Location.X,
			                              canGrowShrinkCollection.First().Location.Y,
			                              canGrowShrinkCollection.First().Size.Width,
			                              canGrowShrinkCollection.First().Size.Height);
			
			foreach (BaseTextItem elemToLayout in canGrowShrinkCollection) {
				Size textSize = MeasurementService.MeasureReportItem (graphics,elemToLayout);
				elemToLayout.Size = new Size(elemToLayout.Size.Width,textSize.Height);
				rec = Rectangle.Union(rec,new Rectangle(elemToLayout.Location,elemToLayout.Size));
				top = Math.Min(top, elemToLayout.Location.Y);
			}
			return rec;
		}
	}
}
