// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
