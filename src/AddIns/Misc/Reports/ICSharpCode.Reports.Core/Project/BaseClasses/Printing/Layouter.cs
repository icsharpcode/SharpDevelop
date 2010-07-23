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
			
			BaseReportItem containerItem = container as BaseReportItem;
			
			Rectangle desiredContainerRectangle = new Rectangle (containerItem.Location,containerItem.Size);
			
			System.Collections.Generic.IEnumerable<BaseReportItem> canGrowShrinkCollection = from bt in container.Items where bt.CanGrow == true select bt;
			
			if (canGrowShrinkCollection.Count() > 0 ) {
				
				int bottomPadding = containerItem.Size.Height - (container.Items[0].Location.Y + container.Items[0].Size.Height);
				
				if (bottomPadding < 0) {
					
					Console.WriteLine(bottomPadding);
				}
				Rectangle surroundingRec = FindSurroundingRectangle(graphics,canGrowShrinkCollection);
				/*
				Console.WriteLine ("surrounding  {0} - desired {1} - bottom {2}",
				                   surroundingRec.Height + bottomPadding,
				                   desiredContainerRectangle.Height -1,
				                   bottomPadding);
				
				Console.WriteLine ("extend");
				*/
				
				desiredContainerRectangle = new Rectangle(containerItem.Location.X,
				                                          containerItem  .Location.Y,
				                                          containerItem .Size.Width,
				                                          surroundingRec.Size.Height + bottomPadding );
//				Console.WriteLine ("extend to {0}",desiredContainerRectangle);
				
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
				
				//Console.WriteLine ("xx layout section");
				
				int bottomPadding = section.Size.Height - (section.Items[0].Size.Height + section.Items[0].Location.Y);
				
				Rectangle surroundingRec = FindSurroundingRectangle(graphics,canGrowShrinkCollection);
				
				/*
				Console.WriteLine ("xx surrounding  {0} - desired {1} - bottom {2}",
				                   surroundingRec.Height + bottomPadding,
				                   desiredSectionRectangle.Height -1,
				                  bottomPadding);
				*/
				if (surroundingRec.Height > desiredSectionRectangle .Height) {
					desiredSectionRectangle = new Rectangle(section.Location.X,
					                                        section .Location.Y,
					                                        section .Size.Width,
					                                        surroundingRec.Size.Height + bottomPadding);
				}
				//Console.WriteLine ("xx extend to {0}",desiredSectionRectangle);
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
