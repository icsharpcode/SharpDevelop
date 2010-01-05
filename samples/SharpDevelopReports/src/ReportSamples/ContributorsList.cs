// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Reports.Core;
namespace ReportSamples
{
	/// <summary>
	/// Description of ContributorsList.
	/// </summary>
	public class ContributorsList:BaseSample
	{
		ContributorCollection contributorCollection;
		
		public ContributorsList()
		{
		}
		
		public override void Run()
		{
			base.Run();
			if (!String.IsNullOrEmpty(base.ReportName)) {
				this.contributorCollection = CreateContributorsList();
			}
		}
		
		private void PushPrinting (object sender,SectionRenderEventArgs e) {

			switch (e.CurrentSection) {
				case GlobalEnums.ReportSection.ReportHeader:
					break;

				case GlobalEnums.ReportSection.ReportPageHeader:
					break;
					
				case GlobalEnums.ReportSection.ReportDetail:
					BaseRowItem ri = e.Section.Items[0] as BaseRowItem;
//					if (ri != null) {
//						BaseDataItem r = (BaseDataItem)ri.Items.Find("unbound1");
//						if (r != null) {
//							System.Console.WriteLine("ubound1");
//				
//						}
//					}
					break;
					
				case GlobalEnums.ReportSection.ReportPageFooter:
					break;
					
				case GlobalEnums.ReportSection.ReportFooter:
					break;
					
				default:
					break;
			}
		}
		
		public ContributorCollection ContributorCollection {
			get { return contributorCollection; }
		}
		
		
		private ContributorCollection CreateContributorsList () {
			ContributorCollection list = new ContributorCollection();
			
			list.Add(new Contributor("Christoph","Wille","Senior Project Wrangler"));
			list.Add(new Contributor("Bernhard","Spuida","Senior Project Wrangler"));
			
			
			list.Add(new Contributor("Daniel","Grunwald","Technical Lead"));
			list.Add(new Contributor("Matt","Ward","NUnit"));
			list.Add(new Contributor("David","Srbecky","Debugger"));
			list.Add(new Contributor("Peter","Forstmeier","SharpDevelop.Reports"));
			list.Add(new Contributor("Alexander","Zeitler","SharpDevelop.Reports"));
			list.Add(new Contributor("Markus","Palme","Prg."));
			list.Add(new Contributor("Georg","Brandl","Prg."));
			list.Add(new Contributor("Roman","Taranchenko",""));
			list.Add(new Contributor("Denis","Erchoff",""));
			list.Add(new Contributor("Ifko","Kovacka",""));
			list.Add(new Contributor("Nathan","Allen",""));
			list.Add(new Contributor("Dickon","Field","DBTools"));
			list.Add(new Contributor("Troy","Simpson","Prg."));
			list.Add(new Contributor("David","Alpert","Prg."));
			return list;
		}

	}
	public class Contributor {
		string last;
		string first;
		string job;
		
		public Contributor(string last, string first,string job)
		{
			this.last = last;
			this.first = first;
			this.job = job;
		}
		
		public string Last {
			get {
				return last;
			}
		
		}
		
		public string First {
			get {
				return first;
			}
			
		}
		
		public string Job {
			get {
				return job;
			}
		}
		
	}
	
	public class ContributorCollection: List<Contributor>{
	}

}


