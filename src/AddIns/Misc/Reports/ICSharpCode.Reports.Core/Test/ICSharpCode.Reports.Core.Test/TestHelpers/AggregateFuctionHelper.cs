/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 28.07.2009
 * Zeit: 19:27
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.Collections.Generic;
using System.Data;

namespace ICSharpCode.Reports.Core.Test.TestHelpers
{
	/// <summary>
	/// Description of AggregateFuctionHelper.
	/// </summary>
	public class AggregateFuctionHelper
	{
		AggregateCollection agCol;
		
		public AggregateFuctionHelper()
		{
			this.agCol = new AggregateCollection();
			this.agCol.Add (new Aggregate("Value1",1,1.5));
			this.agCol.Add (new Aggregate("Value2",2,2.5));
			this.agCol.Add (new Aggregate("Value3",3,3.5));
			this.agCol.Add (new Aggregate("Value400",400,400.5));
		}
		
		
		public DataTable AggregateTable
		{
			get {
				DataTable t = new DataTable("ContributorTable");
				t.Columns.Add(new DataColumn("Name"));
				t.Columns.Add(new DataColumn("IntValue"));
				t.Columns.Add(new DataColumn("Amount"));
				
				foreach (Aggregate a in this.agCol)
				{
					DataRow r = t.NewRow();
					r["Name"] = a.Name;
					r["IntValue"] = a.IntValue;
					r["Amount"] = a.Amount;
					t.Rows.Add(r);
				}
				return t;
			}
		}
				
		
		public AggregateCollection AggregateCollection
		{
			get {return this.agCol;}
		}
	}
	
	public class Aggregate {
		public Aggregate (string name,int intValue, double amount)
		{
			this.Name = name;
			this.IntValue = intValue;
			this.Amount = amount;
		}
		
		public string Name {get;set;}
		public int IntValue {get;set;}
		public double Amount {get;set;}
	}


	public class AggregateCollection: List<Aggregate>
	{
	}

}
