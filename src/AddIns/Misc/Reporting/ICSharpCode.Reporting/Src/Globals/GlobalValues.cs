/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 17.03.2013
 * Time: 17:57
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
namespace ICSharpCode.Reporting.Globals
{
	/// <summary>
	/// Description of GlobalValues.
	/// </summary>
	public static class GlobalValues
	{
		public static string ReportExtension {get {return ".srd";}}
		
		public static string DefaultReportName {get { return "Report1";}}
		
		public static Size DefaultPageSize {get {return new Size(827,1169);}}
		
		public static string PlainFileName
		{
			get {
				return DefaultReportName + ReportExtension;
			}
		}
		
		
		public static Font DefaultFont
		{
			get {
				return new Font("Microsoft Sans Serif",
				               10,
				               FontStyle.Regular,
				               GraphicsUnit.Point);
			}
		}
	}
}
