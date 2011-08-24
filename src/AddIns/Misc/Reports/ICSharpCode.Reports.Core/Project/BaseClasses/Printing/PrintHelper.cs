// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Drawing;
using System.Globalization;
using System.Linq;

using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Interfaces;

namespace ICSharpCode.Reports.Core.BaseClasses.Printing
{
	public sealed class PrintHelper
	{
		private PrintHelper()
		{
		}
		
		#region Section's
		
		public  static void AdjustParent (ISimpleContainer parent,ReportItemCollection items)
		{
			foreach (BaseReportItem item in items) {
				item.Parent = parent as BaseReportItem;
				ISimpleContainer container = item as ISimpleContainer;
				if (container != null) {
					AdjustParentInternal(container.Items,container);
				} else {
					AdjustParentInternal(items,parent as ISimpleContainer);
				}
			}
		}
		
		private static void AdjustParentInternal (ReportItemCollection items,ISimpleContainer parent)
		{
			foreach(BaseReportItem item in items) {
				item.Parent = parent as BaseReportItem;
			}
		}
		
		
		public static void AdjustSectionLocation (BaseReportItem section)
		{
			section.Location = new Point(section.Location.X,section.SectionOffset );
		}
		
		
		#endregion
		
		
		public static void AdjustChildLocation (BaseReportItem item,Point offset)
		{
			item.Location = new Point(item.Location.X + offset.X,
			                          offset.Y + item.Location.Y);
		}
		
		
		public static Rectangle DrawingAreaRelativeToParent (BaseReportItem parent,ISimpleContainer item)
		{
			if ( parent == null) {
				throw new ArgumentNullException("parent");
			}
			if (item == null) {
				throw new ArgumentNullException ("item");
			}
			BaseReportItem bri = (BaseReportItem) item;
			
			return new Rectangle(parent.Location.X + bri.Location.X ,
				                     bri.Location.Y + bri.SectionOffset,
				                     bri.Size.Width,bri.Size.Height);
			
		}
		
		
		public static bool IsRoomForFooter(SectionBounds sectionBounds,Point loc)
		{
			Rectangle r =  new Rectangle( sectionBounds.ReportFooterRectangle.Left,
			                             loc.Y,
			                             sectionBounds.ReportFooterRectangle.Width,
			                             sectionBounds.ReportFooterRectangle.Height);
			
			Rectangle s = new Rectangle (sectionBounds.ReportFooterRectangle.Left,
			                             loc.Y,
			                             
			                             sectionBounds.ReportFooterRectangle.Width,
			                             sectionBounds.PageFooterRectangle.Top - loc.Y -1);
			return s.Contains(r);
		}
		
		
		public static void InitPage (ISinglePage singlePage,ReportSettings reportSettings)
		{
			singlePage.ReportFileName = reportSettings.FileName;
			singlePage.ReportName = reportSettings.ReportName;
			singlePage.ParameterHash = reportSettings.ParameterCollection.CreateHash();	
		}
		
		
		public static bool IsTextOnlyRow (ISimpleContainer item)
		{
			var res =  from r in item.Items where r is BaseDataItem
				select ((BaseTextItem)r);
			if (res.Count() > 0) {
				return false;	
			}
			else {
				return true;
			}
		}
		
		
		public static ISimpleContainer FindContainer (ReportItemCollection items)
		{
			ISimpleContainer container = null;
			
			foreach (BaseReportItem item in items) {
				 container = item as ISimpleContainer;
				if (container != null) {
					break;
				}
			}
			return 
				container;
		}
		
		#region PageBreak
		
		public static Rectangle CalculatePageBreakRectangle(BaseReportItem simpleContainer,Point curPos)
		{
			return new Rectangle(new Point (simpleContainer.Location.X,curPos.Y), simpleContainer.Size);
		}
		
		
		public static bool IsPageFull (Rectangle rectangle,SectionBounds bounds)
		{
			if (rectangle.Bottom > bounds.PageFooterRectangle.Top) {
				return true;
			}
			return false;
		}
		
		
		#endregion
		
		public static Point ConvertRectangleToCurentPosition (Rectangle rectangle)
		{
			return new Point(rectangle.Left,rectangle.Bottom);
		}
		
		
		
		#region Debug Code
		
		///<summary>
		/// Use this function to draw controlling rectangles
		/// For debugging only
		/// </summary>	
		
		public static void DebugRectangle (Graphics graphics,Pen pen,Rectangle rectangle)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			
			graphics.DrawRectangle (pen,rectangle);
			string s = String.Format(CultureInfo.InvariantCulture,
//			                         "Left: {0} , Top: {1}",rectangle.Left.ToString(CultureInfo.CurrentCulture),rectangle.Top.ToString(CultureInfo.CurrentCulture));
			"Sectionrectangle : {0} ",rectangle.ToString());
			Font f = new Font(FontFamily.GenericSerif,10);
			graphics.DrawString(s, f,
			                    Brushes.Red,
			                    rectangle.Left,rectangle.Top - f.Height/2);
			f.Dispose();
		}
		
		
		public static void DebugRectangle (Graphics graphics,Rectangle rectangle)
		{
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}
			graphics.DrawRectangle (Pens.LightGray,rectangle);
			string s = String.Format(CultureInfo.InvariantCulture,
//			                         "Left: {0} , Top: {1}",rectangle.Left.ToString(CultureInfo.CurrentCulture),rectangle.Top.ToString(CultureInfo.CurrentCulture));
			"Sectionrectangle : {0} ",rectangle.ToString());
			Font f = new Font(FontFamily.GenericSerif,10);
			graphics.DrawString(s, f,
			                    Brushes.Red,
			                    rectangle.Left,rectangle.Top - f.Height/2);
			f.Dispose();
		}
		
		/*
		public static void Displaychain (ReportItemCollection items)
		{
			foreach(BaseReportItem i in items)
			{
				ISimpleContainer ic = i as ISimpleContainer;
				if (ic != null) {
					Console.WriteLine("recursive with <{0}> as parent",i.ToString());
					Displaychain(ic.Items);
				} else {
					Console.WriteLine("{0}",i.ToString());
				}
			}
		}
		*/
		
		public static void ShowLocations (ExporterCollection items)
		{
			foreach (BaseExportColumn element in items) {
				ExportContainer cont = element as ExportContainer;
				if (cont != null) {
					ShowLocations(cont.Items);
				}
			}
		}
		
		#endregion
	}
}
