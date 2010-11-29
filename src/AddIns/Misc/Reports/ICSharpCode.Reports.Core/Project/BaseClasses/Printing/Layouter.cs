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
			
//			Console.WriteLine("\tlayouter for container <{0}>",container.ToString());
			Console.WriteLine("\tLayouter for Container");
			Rectangle desiredContainerRectangle = new Rectangle (container.Location,container.Size);
			
			System.Collections.Generic.IEnumerable<BaseReportItem> canGrowShrinkCollection = from bt in container.Items where bt.CanGrow == true select bt;
			
			if (canGrowShrinkCollection.Count() > 0 ) {
				Rectangle surroundingRec = FindSurroundingRectangle(graphics,canGrowShrinkCollection);
//				desiredContainerRectangle = new Rectangle(container.Location.X,
//				                                          container.Location.Y,
//				                                          container.Size.Width,
//				                                          surroundingRec.Size.Height + GlobalValues.ControlMargins.Top + GlobalValues.ControlMargins.Bottom);
//			
				desiredContainerRectangle = new Rectangle(container.Location.X,
				                                          container.Location.Y,
				                                          container.Size.Width,
				                                          surroundingRec.Size.Height);
				
//			var r1 = new Rectangle(container.Location.X,
//				                                          container.Location.Y,
//				                                          container.Size.Width,
//				                                          surroundingRec.Size.Height);
//				                                       
//				Console.WriteLine("Diff {0} - {1} dif  {2}",desiredContainerRectangle,r1,desiredContainerRectangle.Height - r1.Height);
			}
//			Console.WriteLine("\tContainer : {0} - DesiredContainerRectangle {1} ",container.Size,desiredContainerRectangle.Size);
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
		
//				Console.WriteLine("\tLayouter for Container");Console.WriteLine("\tlayouter for section <{0}>",section.Name);
				Console.WriteLine("\tLayouter for Section");
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
//			Console.WriteLine("\tSection : {0} - DesiredContainerRectangle {1} ",section.Size,desiredSectionRectangle.Size);
			return desiredSectionRectangle;
		}
		
	
		
		private static Rectangle FindSurroundingRectangle (Graphics graphics,IEnumerable<BaseReportItem> canGrowShrinkCollection)
		{
			int top = int.MaxValue;
			
			Rectangle rec = new Rectangle(canGrowShrinkCollection.First().Location.X,
			                              canGrowShrinkCollection.First().Location.Y,
			                              canGrowShrinkCollection.First().Size.Width,
			                              canGrowShrinkCollection.First().Size.Height);
			
			foreach (BaseReportItem elemToLayout in canGrowShrinkCollection) {
				Size textSize = MeasurementService.MeasureReportItem (graphics,elemToLayout);
				elemToLayout.Size = new Size(elemToLayout.Size.Width,textSize.Height);
				rec = Rectangle.Union(rec,new Rectangle(elemToLayout.Location,elemToLayout.Size));
				top = Math.Min(top, elemToLayout.Location.Y);
			}
			return rec;
		}
	}
}
