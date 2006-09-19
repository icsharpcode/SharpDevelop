/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 17.09.2006
 * Time: 16:28
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace SharpReportCore
{
	/// <summary>
	/// Description of MeasurementService.
	/// </summary>
	internal class MeasurementService
	{
		
		
		internal static void FitSectionToItems (BaseSection section,Graphics graphics){
			if (section == null) {
				throw new ArgumentNullException("section");
			}
			if (graphics == null) {
				throw new ArgumentNullException("graphics");
			}

		
			if ((section.CanGrow == true)||(section.CanShrink == true))  {
				MeasurementService.AdjustSection (section,graphics);
			} else {
				MeasurementService.AdjustItems (section,graphics);
				
			}
		}
		
		
		public static void AdjustSection (BaseSection section,Graphics g){
			foreach (BaseReportItem rItem in section.Items) {
				if (!MeasurementService.CheckItemInSection (section,rItem,g)){
					SizeF size = MeasurementService.MeasureReportItem (rItem,g);
					section.Size = new Size (section.Size.Width,
					                         Convert.ToInt32(rItem.Location.Y + size.Height));
					
				}
			}
		}
		
		public static void AdjustItems (BaseSection section,Graphics g){

			int toFit = section.Size.Height;
			foreach (BaseReportItem rItem in section.Items) {
				if (!MeasurementService.CheckItemInSection (section,rItem,g)){
					
					rItem.Size = new Size (rItem.Size.Width,
					                       toFit - rItem.Location.Y);
					
				}
			}
		}
		
		
		public static bool CheckItemInSection (BaseSection section,BaseReportItem item ,Graphics g) {
			Rectangle secRect = new Rectangle (0,0,section.Size.Width,section.Size.Height);

			SizeF size  = MeasurementService.MeasureReportItem(item,g);
			Rectangle itemRect = new Rectangle (item.Location.X,
			                                    item.Location.Y,
			                                    (int)size.Width,
			                                    (int)size.Height);
			if (secRect.Contains(itemRect)) {
				return true;
			}
			return false;
		}
		
		public static SizeF MeasureReportItem(IItemRenderer item,
		                                       Graphics g) {
			SizeF sizeF = new SizeF ();
			BaseTextItem myItem = item as BaseTextItem;
			if (myItem != null) {
				string str = String.Empty;
				
				if (item is BaseTextItem) {
					BaseTextItem it = item as BaseTextItem;
					str = it.Text;
				} else if(item is BaseDataItem) {
					BaseDataItem it = item as BaseDataItem;
					str = it.DbValue;
				}
				
				sizeF = g.MeasureString(str,
				                        myItem.Font,
				                        myItem.Size.Width,
				                        myItem.StringFormat);
			} else {
				sizeF = new SizeF (item.Size.Width,item.Size.Height);
			}
			
			return sizeF;
		}
	}
}
