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
using System.Collections.Generic;

namespace ICSharpCode.Reporting.Test.DataSource
{
	/// <summary>
	/// Description of ContributorsList.
	/// </summary>
	public class ContributorsList
	{

		
		public ContributorsList()
		{
			ContributorCollection = CreateContributorsList();
		}
		
		
		public ContributorCollection ContributorCollection {get; private set;}
		
//		list.Add(new Contributor("Ifko","Kovacka","",31,d3,"A"));
//		list.Add(new Contributor("Nathan","Allen","",5,d3,"A"));
		
//		list.Add(new Contributor("Dickon","Field","DBTools",10,d3,"U"));	
//		list.Add(new Contributor("Roman","Taranchenko","",2,d2,"U"));
//		list.Add(new Contributor("Denis","Erchoff","",13,d2,"U"));
			
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
			list.Add(new Contributor("Mike","Krüger","Mono",9,d3,"C"));
			list.Add(new Contributor("Andrea","Krüger","Mono",9,d3,"C"));
			list.Add(new Contributor("Andreas","Weizel","Prg.",9,d3,"C"));
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
