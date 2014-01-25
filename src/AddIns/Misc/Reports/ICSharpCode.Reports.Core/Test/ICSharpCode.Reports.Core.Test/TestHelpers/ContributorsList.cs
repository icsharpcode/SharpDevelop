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
using System.Data;

namespace ICSharpCode.Reports.Core.Test.TestHelpers
{
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
		
		
		public DataTable ContributorTable
		{
			get {
				DataTable t = new DataTable("ContributorTable");
				t.Columns.Add(new DataColumn("First"));
				t.Columns.Add(new DataColumn("Last"));
				t.Columns.Add(new DataColumn("Job"));
				t.Columns.Add(new DataColumn("GroupItem"));
				t.Columns.Add(new DataColumn("RandomInt",typeof(int)));
				t.Columns.Add(new DataColumn("RandomDate",typeof(DateTime)));
				foreach (Contributor c in this.contributorCollection) {
					DataRow r = t.NewRow();
					r["First"] = c.First;
					r["Last"] = c.Last;
					r["Job"] = c.Job;
					r["GroupItem"] = c.GroupItem;
					r["RandomInt"] = c.RandomInt;
					r["RandomDate"] = c.RandomDate;
					t.Rows.Add(r);
				}
				return t;
			}
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
	
	
	public class Contributor {
		string last;
		string first;
		string job;
		
		int randomInt;
		DateTime randomDate;
		
		public Contributor(string last, string first,string job,int randomInt,DateTime randomDate,string groupItem)
		{
			this.last = last;
			this.first = first;
			this.job = job; 
			this.randomDate = randomDate;
			this.randomInt = randomInt;
			this.GroupItem = groupItem;
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
		
		public int RandomInt {
			get { return randomInt; }
		}
		
		
		public DateTime RandomDate {
			get { return randomDate; }
		}
		
		public string GroupItem {get; set;}
		
		public MyDummyClass DummyClass {get;set;}
		
	}
	
	
	
	public class MyDummyClass
	{
		public MyDummyClass()
		{
			
		}
			
		public string DummyString {get;set;}
		public int DummyInt {get;set;}
	}
	
	
	public class ContributorCollection: List<Contributor>
	{
	}
}
