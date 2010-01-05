// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Data;
using System.Windows.Forms;

using ICSharpCode.Reports.Core;

namespace ReportSamples
{
    /// <summary>
    /// Description of BaseSample.
    /// </summary>
    public class BaseSample
    {
        //		string msdeConnection = @"Provider=SQLOLEDB.1;Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=Northwind";

        ReportEngine engine = new ReportEngine();

        string reportName;

        public BaseSample()
        {
            engine = new ReportEngine();
        }

        public virtual void Run()
        {
            try
            {
                using (OpenFileDialog dg = new OpenFileDialog())
                {
                    dg.Filter = "SharpDevelop Reports report templates|*.srd";
                    dg.Title = "Select a report file";

                    if (dg.ShowDialog() == DialogResult.OK)
                    {
                        this.reportName = dg.FileName;
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString(), "MainForm");
            }
        }

        
        protected DataTable SelectData()
        {
            OpenFileDialog dg = new OpenFileDialog();
            dg.Filter = "SharpDevelop Reports data files|*.xsd";
            dg.Title = "Select a data file";
            if (dg.ShowDialog() == DialogResult.OK)
            {
                DataSet ds = new DataSet();
                ds.ReadXml(dg.FileName);
                return ds.Tables[0];
            }
            return null;
        }


        public ReportEngine Engine
        {
            get
            {
                return engine;
            }
        }


        public string ReportName
        {
            get
            {
                return reportName;
            }
        }


        //		protected string MSDEConnection {
        //			get {
        //				return msdeConnection;
        //			}
        //		
        //		}

    }
}

