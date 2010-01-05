using System;
using System.Web.UI;

// usings for SharpDevelop Reports for .NET
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Exporter;
using ICSharpCode.Reports.Core.Exporter.ExportRenderer;


public partial class _Default : Page 
{
	protected void Page_Load(object sender, EventArgs e) 
    {
	}

    protected void Button1_Click(object sender, EventArgs e)
    {
        // instance reporting engine
        // assign parameters
        ReportEngine engine = new ReportEngine();
        string reportPath = Server.MapPath("SalesByYear.srd");
		PageBuilder pageBuilder = engine.CreatePageBuilder(reportPath);
		pageBuilder.BuildExportList();
        string outputPath = Server.MapPath("SalesByYear.Pdf");

        // render report
		PdfRenderer pdfRenderer =
			PdfRenderer.CreateInstance(pageBuilder.Pages, outputPath, false);
		pdfRenderer.Start();
		pdfRenderer.RenderOutput();
		pdfRenderer.End();

        // send report to the client
		Response.ContentType = "Application/pdf";
		Response.WriteFile(outputPath);
		Response.End();
    }
}