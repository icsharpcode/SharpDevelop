/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 20.05.2013
 * Time: 18:09
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace ICSharpCode.Reporting.Test.DataSource
{
	/// <summary>
	/// Description of ContributorsList.
	/// </summary>
	public class ContributorsList
	{
		ContributorCollection contributorCollection;
		
		public ContributorsList()
		{
			this.contributorCollection = CreateContributorsList();
		}
		
		public ContributorCollection ContributorCollection {
			get { return contributorCollection; }
		}
		
		private ContributorCollection CreateContributorsList () {
			
			DateTime d1 = new DateTime(2000,11,11);
			DateTime d2 = new DateTime(2000,01,01);
			DateTime d3 = new DateTime(2000,12,24);
			
			ContributorCollection list = new ContributorCollection();
			
			list.Add(new Contributor("Christoph","Wille","Senior Project Wrangler",17,new DateTime(1960,12,8),"F"));
			list.Add(new Contributor("Bernhard","Spuida","Senior Project Wrangler",25,new DateTime(1962,2,24),"D"));
			
			
			list.Add(new Contributor("Daniel","Grunwald","Technical Lead",12,d1,"F"));
			
			list.Add(new Contributor("Matt","Ward","NUnit",7,d1,"F"));
			list.Add(new Contributor("David","Srbecky","Debugger",1,d1,"C"));			
			list.Add(new Contributor("Peter","Forstmeier","SharpDevelop.Reports",7,d1,"D"));
			
			list.Add(new Contributor("Alexander","Zeitler","SharpDevelop.Reports",3,d2,"D"));
			list.Add(new Contributor("Markus","Palme","Prg.",6,d2,"R"));			
			list.Add(new Contributor("Georg","Brandl","Prg.",5,d2,"R"));
			list.Add(new Contributor("Roman","Taranchenko","",2,d2,"U"));
			list.Add(new Contributor("Denis","Erchoff","",13,d2,"U"));
			
			list.Add(new Contributor("Ifko","Kovacka","",31,d3,"A"));
			list.Add(new Contributor("Nathan","Allen","",5,d3,"A"));
			list.Add(new Contributor("Dickon","Field","DBTools",10,d3,"U"));
			
			list.Add(new Contributor("Troy","Simpson","Prg.",9,d3,"C"));
			list.Add(new Contributor("David","Alpert","Prg.",6,d3,"C"));
			return list;
		}
	}
	
	public class ContributorCollection: List<Contributor>
	{
	}
	
	public class Contributor {
		
	
		public Contributor(string lastname, string firstname,string job,int randomInt,DateTime randomDate,string groupItem)
		{
			this.Lastname = lastname;
			this.Firstname = firstname;
			this.Job = job; 
			this.RandomDate = randomDate;
			this.RandomInt = randomInt;
			this.GroupItem = groupItem;
		}
		
		
		public string Lastname {get;private set;}
		
		
		public string Firstname {get;private set;}
			
		
		public string Job {get; private set;}
		
		
		public int RandomInt {get; private set;}
		
		
		public DateTime RandomDate {get;private set;}
			
		
		public string GroupItem {get; private set;}
		
	}
}
