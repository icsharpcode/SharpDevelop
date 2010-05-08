/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 18.07.2007
 * Zeit: 09:40
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Drawing.Printing;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.factories;


namespace ICSharpCode.Reports.Core.Exporter.ExportRenderer
{
	/// <summary>
	/// Description of Pdf2Renderer.
	/// </summary>
	public class PdfRenderer:BaseExportRenderer,IDisposable
	{

		Document document;
		PdfWriter pdfWriter;
		string fileName;
		bool showOutput;
		PdfUnitConverter pdfUnitConverter;
		ReportSettings reportSettings;
		
		
		public static PdfRenderer CreateInstance (IReportCreator basePager,string fileToSave,bool showOutput) {
//		public static PdfRenderer CreateInstance (BasePager basePager,string fileToSave,bool showOutput) {
			if ( basePager == null) {
				throw new ArgumentNullException("basePager");
			}
			if (String.IsNullOrEmpty(fileToSave)) {
				throw new ArgumentNullException("fileTosave");
			}
			BasePager bp = basePager as BasePager;
			
			return PdfRenderer.CreateInstance(bp.ReportModel.ReportSettings,bp.Pages,fileToSave,showOutput);
		}
		
		
		/// <summary>
		/// For internal use only
		/// </summary>
		/// <param name="reportSettings"></param>
		/// <param name="pages"></param>
		/// <param name="fileName"></param>
		/// <param name="showOutput"></param>
		/// <returns></returns>
		public static PdfRenderer CreateInstance (ReportSettings reportSettings,PagesCollection pages,string fileName,bool showOutput) {	
		                                          
			if ((pages == null) ||(pages.Count == 0)) {
				throw new ArgumentNullException("pages");
			}
			if (String.IsNullOrEmpty(fileName)) {
				throw new ArgumentNullException("fileName");
			}
			
			PdfRenderer instance = new PdfRenderer(pages);
			instance.fileName = fileName;
			instance.reportSettings = reportSettings;
			instance.showOutput = showOutput;
			return instance;
		}
		
		private PdfRenderer(PagesCollection pages):base(pages)
		{
		}
		
		
		public override void Start()
		{
			base.Start();
			this.document = (this.reportSettings.Landscape)? new Document(PageSize.A4.Rotate(),0,0,0,0) : new Document(PageSize.A4,0,0,0,0);
			this.pdfUnitConverter = new PdfUnitConverter(this.document.PageSize,this.reportSettings);
			this.pdfWriter = PdfWriter.GetInstance(document, new FileStream(this.fileName,FileMode.Create));
			document.AddCreator(GlobalValues.ResourceString("ApplicationName"));
			FontFactory.RegisterDirectories();
			MyPageEvents events = new MyPageEvents();
			this.pdfWriter.PageEvent = events;
		}
		
		
		public override void RenderOutput()
		{
			bool firstPage = true;
			base.RenderOutput();
			foreach (ExporterPage page in base.Pages) {
				
				if (firstPage) {
					document.Open();
					firstPage = false;
				} else {
					document.NewPage();
				}
				DrawPage(page.Items);
			}
			document.Close();
		}
		
		
		private void DrawPage (ExporterCollection items)
		{
			foreach (ICSharpCode.Reports.Core.Exporter.BaseExportColumn baseExportColumn in items) {
				if (baseExportColumn != null) {

					IExportContainer container = baseExportColumn as ExportContainer;
					if (container == null) {
						baseExportColumn.DrawItem(this.pdfWriter,this.pdfUnitConverter);
					} else {
						container.DrawItem(this.pdfWriter,this.pdfUnitConverter);
						this.DrawPage(container.Items);
					}
				}
			}
		}
		
//		
//		private static void DebugRectangle (PdfContentByte cb,Rectangle r)
//		{
//			cb.Rectangle(r.Left,r.Bottom,r.Right,r.Top);
//			cb.Stroke();
//		}
		
		
		public override void End()
		{
			base.End();
	
			if (document.IsOpen()) {
				document.Close();
			}
			
			if (this.showOutput) {
				System.Diagnostics.Process.Start(this.fileName);
			}
		}
		
		
		#region IDisposable
		
		public void Dispose()
		{
			Dispose (true);
		}
		
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				
				// free managed resources
//				if (this.document != null)
//				{
//					this.document.Dispose();
//					this.document = null;
//				}
			}
		}
		
		#endregion
	}
	
	
	public class PdfUnitConverter {
		Rectangle pageSize;
		float lowerLeftX;
		float lowerleftY;
		float upperRightX;
		float upperRightY;
		ReportSettings reportsettings;
		
		public PdfUnitConverter (Rectangle pageSize,ReportSettings reportsettings)
		{
			if (pageSize == null) {
				throw new ArgumentNullException("pageSize");
			}
			if (reportsettings == null) {
				throw new ArgumentNullException("reportsettings");
			}
			this.pageSize = pageSize;
			this.reportsettings = reportsettings;
			this.lowerLeftX = UnitConverter.FromPixel(this.reportsettings.LeftMargin);
			
			this.lowerleftY = UnitConverter.FromPixel(this.reportsettings.BottomMargin);
			
			this.upperRightX = PageSize.A4.Width;
			this.upperRightY = PageSize.A4.Height;
		}
		
		
		public float LowerLeftX {
			get { return lowerLeftX; }
		}
		
		public float LowerleftY {
			get { return lowerleftY; }
		}
		
		public float UpperRightX {
			get { return upperRightX; }
		}
		
		public float UpperRightY {
			get { return upperRightY; }
		}
		
		public float PageTop{
			get{
				return this.pageSize.Height;
			}
		}
		
		
		public float LeftBorder {
			get {
				return this.lowerLeftX;
			}
		}
	}
	
	
	public class MyPageEvents : PdfPageEventHelper 
	{ 
		
		public override void OnCloseDocument(PdfWriter writer, Document document)
		{
			base.OnCloseDocument(writer, document);
		}
		
		
		public override void OnEndPage(PdfWriter writer, Document document)
		{
			base.OnEndPage(writer, document);
		}
		
		
		public override void OnOpenDocument(PdfWriter writer, Document document)
		{
			base.OnOpenDocument(writer, document);
		}
		
		
		public override void OnStartPage(PdfWriter writer, Document document)
		{
			base.OnStartPage(writer, document);
		}
	} 
}
