/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07/12/2014
 * Time: 18:20
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Globals;
using Xceed.Wpf.Toolkit;

namespace ICSharpCode.Reporting.Addin.ReportWizard.ViewModels
{
	/// <summary>
	/// Description of PageOneViewModel.
	/// </summary>
	public class PageOneContext:IWizardContext
	{
		public PushPullModel DataModel {get;set;}
//		public ReportType ReportType {get;set;}
		public string ReportName {get;set;}
		public string FileName {get;set;}
		public bool Legal {get;set;}
		public bool Landscape {get;set;}
	}
}
