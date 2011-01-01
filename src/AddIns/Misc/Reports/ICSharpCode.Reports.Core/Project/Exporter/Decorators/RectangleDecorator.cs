/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 30.12.2010
 * Time: 19:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reports.Core.Exporter;

namespace ICSharpCode.Reports.Core.Exporter
{
	/// <summary>
	/// Description of RectangleDecorator.
	/// </summary>
	public class aaRectangleDecorator: GraphicStyleDecorator
	{
		public aaRectangleDecorator(BaseShape shape):base(shape)
		{
		}
		
		private int corner;
		
		public int CornerRadius {
			get {
				
				return corner;
			}
			set { corner = value; }
		}
		
		
	}
}
