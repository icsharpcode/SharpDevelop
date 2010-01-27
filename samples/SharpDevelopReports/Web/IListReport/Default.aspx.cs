using System;
using System.Collections.Generic;
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
        contributors = CreateTestList();


        // instance reporting engine
        // assign parameters

        ReportEngine engine = new ReportEngine();

        string reportPath = Server.MapPath("ContributorList.srd");

        ReportModel reportModel = ReportEngine.LoadReportModel(reportPath);

        PageBuilder pageBuilder = engine.CreatePageBuilder(reportModel, contributors);
        pageBuilder.BuildExportList();
        string outputPath = Server.MapPath("ContributorList.pdf");

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


    private TestList contributors;

    public TestList Contributors
    {
        get { return contributors; }
    }

    private TestList CreateTestList()
    {
        TestList list = new TestList();

        list.Add(new LastFirst("Christoph", "Wille", "Senior Project Wrangler"));
        list.Add(new LastFirst("Bernhard", "Spuida", "Senior Project Wrangler"));


        list.Add(new LastFirst("Daniel", "Grunwald", "Technical Lead"));
        list.Add(new LastFirst("Matt", "Ward", "NUnit"));
        list.Add(new LastFirst("David", "Srbecky", "Debugger"));
        list.Add(new LastFirst("Peter", "Forstmeier", "SharpReport"));
        list.Add(new LastFirst("Markus", "Palme", "Prg."));
        list.Add(new LastFirst("Georg", "Brandl", "Prg."));
        list.Add(new LastFirst("Roman", "Taranchenko", ""));
        list.Add(new LastFirst("Denis", "Erchoff", ""));
        list.Add(new LastFirst("Ifko", "Kovacka", ""));
        list.Add(new LastFirst("Nathan", "Allen", ""));
        list.Add(new LastFirst("Dickon", "Field", "DBTools"));
        list.Add(new LastFirst("Troy", "Simpson", "Prg."));
        list.Add(new LastFirst("David", "Alpert", "Prg."));
        return list;
    }


    public class LastFirst
    {
        private string last;
        private string first;
        private string job;

        public LastFirst(string last, string first, string job)
        {
            this.last = last;
            this.first = first;
            this.job = job;
        }

        public string Last
        {
            get { return last; }
        }

        public string First
        {
            get { return first; }
        }

        public string Job
        {
            get { return job; }
        }
    }

    public class TestList : List<LastFirst>
    {
    }
}