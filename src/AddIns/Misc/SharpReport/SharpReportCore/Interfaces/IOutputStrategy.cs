/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 05.09.2005
 * Time: 15:30
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Drawing;

namespace SharpReportCore
{
	/// <summary>
	/// Description of IOutputStrategy.
	/// </summary>
	
	public interface IOutputStrategy {
		/// <summary>
		/// Measure the Size of the currnet Item
		/// </summary>
		SizeF MeasureItem(System.Drawing.Graphics graphics);
		/// <summary>
		/// Format the current (TextBased)
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		void FormatItem(System.Drawing.Graphics graphics );
		
		/// <summary>
		/// Print them out to ....
		/// </summary>
		/// <param name="e"></param>
		void OutputItem (System.Drawing.Graphics graphics);
	}
}
