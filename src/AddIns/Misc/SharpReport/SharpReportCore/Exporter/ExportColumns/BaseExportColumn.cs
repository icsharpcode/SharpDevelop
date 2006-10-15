/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 09.10.2006
 * Time: 09:40
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace SharpReportCore.Exporters
{
	/// <summary>
	/// Description of BaseLineItem.
	/// </summary>
	public class BaseExportColumn :IPerformLine
	{
		BaseStyleDecorator styleDecorator;
		bool isContainer;
		
		#region Constructors
	
		public BaseExportColumn(){
			this.styleDecorator = new BaseStyleDecorator(Color.White,Color.Black);
		}
		
//		public BaseExportColumn(BaseStyleDecorator styleDecorator)
//		{
//			this.styleDecorator = styleDecorator;
//		}
		
		public BaseExportColumn(BaseStyleDecorator itemStyle, bool isContainer)
		{
			this.styleDecorator = itemStyle;
			this.isContainer = isContainer;
		}
		
		#endregion
			
		public virtual BaseStyleDecorator StyleDecorator {
			get {
				return styleDecorator;
			}
			set {
				this.styleDecorator = value;
			}
		}
		
		public bool IsContainer {
			get {
				return isContainer;
			}
			set {
				isContainer = value;
			}
		}
	}
}
