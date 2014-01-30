// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.ServiceProcess;
using System.Text;

//// Alex: for spooler

	
/// <summary>
/// This Class contains Global Icons and Constantes
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 19.01.2005 10:57:33
/// </remarks>
	
namespace ICSharpCode.Reports.Core.Globals
{
	public sealed class GlobalValues
	{

		private static string reportExtension = ".srd";
		private static string xsdExtension = "xsd";
		private static string reportFileName = "Report1";
		private const string unbound = "unbound";
		private const string tableName = "Table";
		private const int defaultZoomfactor = 1;
		private const int gapBetweenRows = 1;
		//WPF
		private const double defaultCornerRadius = 2;
		private static  double defaulThickness = 2;
		
		private GlobalValues()
		{
		}
		
		
		#region some usefull functions and methodes
		
		private static bool IsSpoolerRunning()
		{
			ServiceController[] services;
			services = ServiceController.GetServices();
			foreach (ServiceController sc in services) {
				if (String.Compare(sc.ServiceName, "spooler", true,CultureInfo.InvariantCulture) == 0){
					if (sc.Status == ServiceControllerStatus.Running) {
						return true;
					} else {
						return false;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// We need at least one installed printer to run SharpReport
		/// </summary>
		public static bool IsValidPrinter ()
		{
			if (IsSpoolerRunning()) {
				if (PrinterSettings.InstalledPrinters.Count == 0) {
					return false;
				} else {
					return true;
				}
			} else {
				return false;
			}
		}
		
		#endregion
	
		
		#region numeric Constant
		/// <summary>
		/// The value on witch the Control is drawing bigger than the text inside
		/// </summary>
		
		
		public static Margins ControlMargins 
		{
			get { return new Margins(5,5,5,5); }
		}
		
		
		///<summary>
		/// The default Size of a Section
		/// used in <see cref="ReportGenerator"
		/// </summary>
		public static int DefaultSectionHeight {get {return 60;}}
		
		
		/// <summary>
		/// Default zoom Factor for all typ's of preview
		/// </summary>
		/// 
		public static int DefaultZoomFactor
		{
			get {return defaultZoomfactor;}
		}
		
		
		
		public static Size PreferedSize {get {return new Size(100,20);}}
		                                  
	
		public static Size DefaultPageSize {get {return new Size(827,1169);}}
		
		public static Margins DefaultPageMargin {get {return new Margins(50,50,50,50);}}
		
		#endregion
		
		
		#region String Constant's
		
		public static string ReportExtension 
		{
			get {
				return reportExtension;
			}
		}
		
		public static string DefaultReportName 
		{
			get {
				return reportFileName;
			}
		}
		
		public static string PlainFileName
		{
			get {
				return reportFileName + reportExtension;
			}
		}
		

		
		public static string TableName
		{
			get {
				return tableName;
			}
		}
		
		#endregion
		
		
		#region Printing
		
		public static int GapBetweenContainer {
			get {return gapBetweenRows;}
		}
		
		#endregion
		
		
		#region Message's


		/// <summary>
		/// FileFilter for store and load of SharpReport Files
		/// </summary>
		public static string FileFilter
		{
			get {
				StringBuilder str = new StringBuilder ("SharpReports (*.xml)" + "|" + "*.xml");
				str.Append( "|");
				str.Append("SharpReports (*.srd)" + "|" + "*.srd");
				str.Append("|");
				str.Append("All Files (*.*)" +  "|" + "*.*");
				
				return str.ToString();
			}
		}
		
		public static string ReportFileFilter
		{
			get {
				return "SharpReports (*.srd)" + "|" + "*.srd";
			}
		}
		/// <summary>
		/// File Filter for *.Xsd Files
		/// </summary>
		public static string XsdFileFilter {
			get {
				return "XSD Files | *.xsd";
			}
			
		}
		
		public static string XsdExtension 
		{
			get {
				return xsdExtension;
			}
		}
		
		
		public static string UnkownFunctionMessage (string functionName)
		{
			 return String.Format(System.Globalization.CultureInfo.InvariantCulture,
					                      "!! Can't find <{0}>  !! ",functionName);
		}
		#endregion
		
		#region Color's
		
		public static Color DefaultBackColor
		{
			get {
				return Color.White;
			}
		}
		
		#endregion
		
		#region Font
		
		public static Font DefaultFont
		{
			get {
				return new Font("Microsoft Sans Serif",
				               10,
				               FontStyle.Regular,
				               GraphicsUnit.Point);
			}
		}
		
		#endregion
		
		
		#region Wpf
		
		public static double DefaultCornerRadius {get {return defaultCornerRadius;}}
		public static double DefaultBorderThickness {get {return defaulThickness;}}
					
		#endregion
		
		
		#region Icons
		
		/// <summary>
		/// ToolboxIcon for ReportRectangle
		/// </summary>
		/// <returns>Bitmap</returns>
		public static Bitmap RectangleBitmap()
		{
			Bitmap b = new Bitmap (16,16);
			using (Graphics g = Graphics.FromImage (b)){
				g.DrawRectangle (new Pen(Color.Black, 1),
				                 1,1,14,14);
			}
			return b;
		}
		
		/// <summary>
		/// ToolboxIcon for ReportCircle
		/// </summary>
		/// <returns>Bitmap</returns>
		public static Bitmap CircleBitmap()
		{
			Bitmap b = new Bitmap (19,19);
			using (Graphics g = Graphics.FromImage (b)){
				g.DrawEllipse (new Pen(Color.Black, 1),
				               1,1,
				               17,17);
			}
			return b;
		}
		#endregion
		
		#region Resources
		
		/*
		public static void ShowResources ()
		{
			string NL = Environment.NewLine;

			System.Reflection.Assembly thisExe = System.Reflection.Assembly.GetExecutingAssembly();
			string[] resources = thisExe.GetManifestResourceNames();
			string list = String.Empty;
			foreach (string resource in resources) list += resource + NL;
//			System.Windows.Forms.MessageBox.Show("Verfügbare Ressource-Namen:" + NL + list);

			
		}
		*/
		
		public static string ResourceString (string name)
		{
			System.Resources.ResourceManager r = new System.Resources.ResourceManager("ICSharpCode.Reports.Core.Project.Resources.CoreResource",
			                                                                          System.Reflection.Assembly.GetExecutingAssembly());
			return r.GetString(name);
		}
		
		#endregion
	}
}
