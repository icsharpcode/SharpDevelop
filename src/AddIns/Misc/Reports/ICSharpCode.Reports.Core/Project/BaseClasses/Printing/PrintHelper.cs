/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 02.03.2009
 * Zeit: 19:20
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing;
using System.Globalization;
using System.Linq;

using ICSharpCode.Reports.Core.Interfaces;
using ICSharpCode.Reports.Core.old_Exporter;
using ICSharpCode.Reports.Expressions.ReportingLanguage;

namespace ICSharpCode.Reports.Core
{
	public sealed class PrintHelper
	{
		private PrintHelper()
		{
		}
		
		#region Layout
		
		public static void SetLayoutForRow (Graphics graphics, ILayouter layouter,ISimpleContainer row)
		{
			BaseReportItem item = row as BaseReportItem;
			int extend = item.Size.Height - row.Items[0].Size.Height;
			Rectangle textRect = layouter.Layout(graphics,row);
			if (textRect.Height > item.Size.Height) {
				item.Size = new Size(item.Size.Width,textRect.Height + extend );
			}
		}
		
		#endregion
		
		#region AdjustParentInSection
		
		public  static void AdjustParent (BaseReportItem parent,ReportItemCollection items)
		{
			foreach (BaseReportItem i in items) {
				i.Parent = parent;
				ISimpleContainer ic = i as ISimpleContainer;
				if (ic != null) {
					AdjustParentInternal(ic.Items,i);
				} else {
					AdjustParentInternal(items,parent);
				}
			}
		}
		
		
		private static void AdjustParentInternal (ReportItemCollection items,BaseReportItem parent)
		{
			foreach(BaseReportItem item in items) {
				item.Parent = parent;
			}
		}
		
		#endregion
		
		
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
			singlePage.StartRow = -1;
			singlePage.EndRow = -1;
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
		
		public static bool IsPageFull (Rectangle rectangle,SectionBounds bounds)
		{
			if (rectangle.Bottom > bounds.PageFooterRectangle.Top) {
				return true;
			}
			return false;
		}
		
		
		public static IExpressionEvaluatorFacade  SetupEvaluator (ISinglePage singlePage,IDataNavigator dataNavigator)
		{
			if (singlePage == null) {
			
				throw new ArgumentNullException("singlePage");
			}
			if (dataNavigator == null) {
				throw new ArgumentNullException("dataNavigator");
			}
			IExpressionEvaluatorFacade evaluatorFacade = new ExpressionEvaluatorFacade();
			evaluatorFacade.SinglePage = singlePage;
			evaluatorFacade.SinglePage.IDataNavigator = dataNavigator;
			return evaluatorFacade;
		}
		
		
		public static IExpressionEvaluatorFacade  SetupEvaluator ()
		{
			return new ExpressionEvaluatorFacade();
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
		
		
		public static void Displaychain (ReportItemCollection items)
		{
			Console.WriteLine();
			Console.WriteLine("BasePager:Displaychain");
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
		
		
		public static void ShowLocations (ExporterCollection items)
		{
			Console.WriteLine("");
			
			foreach (BaseExportColumn element in items) {
				ExportContainer cont = element as ExportContainer;
				if (cont != null) {
					Console.WriteLine("-------Container ----{0}-----",cont.StyleDecorator.Location);
					ShowLocations(cont.Items);
				}
				System.Console.WriteLine ("{0} - {1}",element.ToString(),element.StyleDecorator.Location.ToString());
			}
		}
		
		#endregion
	}
}
