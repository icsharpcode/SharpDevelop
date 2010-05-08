// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

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
	
namespace ICSharpCode.Reports.Core {
	public sealed class GlobalValues
	{
//		private static string reportString = "Report";
		private static string reportExtension = ".srd";
		private static string xsdExtension = "xsd";
		private static string reportFileName = "Report1";
		private const string unbound = "unbound";
		private const string tableName = "Table";
		private const int enlargeControl = 5;
		private const int defaultZoomfactor = 1;
		private const int gapBetweenRows = 1;
		
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
	
		#region Formatting
		/// <summary>
		/// Build a StringFormat witch is used allover the code to
		/// Format all String's the same way.
		/// </summary>
		/// <returns>a StringFormat object</returns>
		/*
		public static StringFormat StandardStringFormat()
		{
			StringFormat sFormat = StringFormat.GenericTypographic;
			sFormat.FormatFlags |= StringFormatFlags.LineLimit;
			return sFormat;
		}
		*/
		#endregion
		
		
		#region numeric Constant
		/// <summary>
		/// The value on witch the Control is drawing bigger than the text inside
		/// </summary>
		/*
		public static int EnlargeControl
		{
			get {return enlargeControl;}
		}
		*/
		
		public static Margins ControlMargins 
		{
			get { return new Margins(5,5,5,5); }
		}
		
		
		///<summary>
		/// The default Size of a Section
		/// used in <see cref="ReportGenerator"
		/// </summary>
		public static int DefaultSectionHeight 
		{
			get {
				return 60;
			}
		}
		
		/// <summary>
		/// Default zoom Factor for all typ's of preview
		/// </summary>
		/// 
		public static int DefaultZoomFactor
		{
			get {return defaultZoomfactor;}
		}
		
		
		
		public static Size PreferedSize 
		{
			get {
				return new Size(100,20);
			}
		}
		                                  
	
		public static Size DefaultPageSize {get {return new Size(827,1169);}}
		
		public static Margins DefaultPageMargin {get {return new Margins(50,50,50,50);}}
		
		#endregion
		
		
		#region String Constant's
		
		/*
		public static string ReportString
		{
  			get {
  				return reportString;
  			}
  		}
		*/
		
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
		
		public static string UnboundName 
		{
			get {return unbound;}
		}
		
		public static string TableName
		{
			get {
				return tableName;
			}
		}
		
		/*
		public static string FunctionStartTag
		{
			get {return "{=";}
		}
		
		
		public static string FunctionEndTag
		{
			get {return "}";}
		}
		*/
		
		/*
		public static string StringParserStartTag
		{
		get {return "${";}
			
		}
		*/
		
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
