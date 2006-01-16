/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 29.11.2004
 * Time: 16:53
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
//using SharpReport.Printing;

namespace SharpReportCore
{
	/// <summary>
	/// Description of IRender.
	/// </summary>
	public interface IRender{
		void Render (ReportPageEventArgs rpea, float startDrawAt);
		
		float DrawAreaHeight (ReportPageEventArgs rpea);
	}
}
