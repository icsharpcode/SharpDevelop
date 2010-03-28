/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 07.01.2010
 * Zeit: 19:46
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reports.Core;

namespace SharpReportSamples
{
	/// <summary>
	/// Description of ContributorsList.
	/// </summary>
	public class ContributorsReportData
	{

		public static ContributorCollection CreateContributorsList () {
			ContributorCollection list = new ContributorCollection();
			
			list.Add(new Contributor("Christoph","Wille","Senior Project Wrangler"));
			list.Add(new Contributor("Bernhard","Spuida","Senior Project Wrangler"));
		
			list.Add(new Contributor("Daniel","Grunwald","Technical Lead"));
			list.Add(new Contributor("Matt","Ward","NUnit"));
			list.Add(new Contributor("David","Srbecky","Debugger"));
			list.Add(new Contributor("Peter","Forstmeier","SharpDevelop.Reports"));
			list.Add(new Contributor("Markus","Palme","Prg."));
			list.Add(new Contributor("Itai","Bar-Haim","Prg."));
			list.Add(new Contributor("Russel","Wilkins","Prg."));
			list.Add(new Contributor("Justin","Dearing","Prg."));
			list.Add(new Contributor("Christian","Hornung","Prg."));
			list.Add(new Contributor("Siegfried","Pammer","Prg."));
			list.Add(new Contributor("Georg","Brandl","Prg."));
			list.Add(new Contributor("Roman","Taranchenko",""));
			list.Add(new Contributor("Denis","Erchoff",""));
			list.Add(new Contributor("Ifko","Kovacka",""));
			list.Add(new Contributor("Nathan","Allen",""));
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
