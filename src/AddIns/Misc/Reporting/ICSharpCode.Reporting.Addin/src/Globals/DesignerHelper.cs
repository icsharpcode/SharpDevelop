/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.09.2014
 * Time: 20:12
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.Reporting.Addin.Globals
{
	/// <summary>
	/// Description of DesignerHelper.
	/// </summary>
	public static  class DesignerHelper
	{
		
		public static int AlignCenter (int avaiableWidth, int itemWidth) {
			if (avaiableWidth == 0) {
				throw new ArgumentException("avaiableWidth");
			}
			if (itemWidth == 0) {
				throw new ArgumentException("itemWidth");
			}
			return (avaiableWidth - itemWidth)/2;
		}
		
		public static int AlignRight(int avaiableWidth, int itemWidth) {
			if (avaiableWidth == 0) {
				throw new ArgumentException("avaiableWidth");
			}
			return avaiableWidth - itemWidth;
		}
		
	}
}
