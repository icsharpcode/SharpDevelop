/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 12.11.2009
 * Zeit: 19:40
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;

using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core
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
			
			BaseReportItem containerItem = container as BaseReportItem;
//			container.Items.SortByLocation();
			Rectangle desiredContainerRectangle = new Rectangle (containerItem.Location,containerItem.Size);
			
			System.Collections.Generic.IEnumerable<BaseReportItem> canGrowShrinkCollection = from bt in container.Items where bt.CanGrow == true select bt;
			
			if (canGrowShrinkCollection.Count() > 0 ) {
				int extend = containerItem.Size.Height - canGrowShrinkCollection.First().Size.Height;
				Rectangle surroundingRec = FindSurroundingRectangle(graphics,canGrowShrinkCollection);
				
				if (surroundingRec.Height > desiredContainerRectangle.Height) {
					desiredContainerRectangle = new Rectangle(containerItem.Location.X,
					                                          containerItem  .Location.Y,
					                                          containerItem .Size.Width,
					                                          surroundingRec.Size.Height + extend);
				}
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
			//section.Items.SortByLocation();
		
			IEnumerable<BaseReportItem> canGrowShrinkCollection = from bt in section.Items where bt.CanGrow == true select bt;
			
			Rectangle desiredSectionRectangle = new Rectangle(section.Location.X,
			                                                  section .Location.Y,
			                                                  section .Size.Width,
			                                                  section.Size.Height);
			
			if (canGrowShrinkCollection.Count() > 0) {
				int extend = section.Size.Height - canGrowShrinkCollection.First().Size.Height;
				Rectangle surroundingRec = FindSurroundingRectangle(graphics,canGrowShrinkCollection);
				if (surroundingRec.Height > desiredSectionRectangle .Height) {
					desiredSectionRectangle = new Rectangle(section.Location.X,
					                                        section .Location.Y,
					                                        section .Size.Width,
					                                        surroundingRec.Size.Height + extend);
				}
			}
			return desiredSectionRectangle;
		}
		
	
		
		private static Rectangle FindSurroundingRectangle_2 (Graphics graphics,IEnumerable<BaseReportItem> canGrowShrinkCollection)
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
