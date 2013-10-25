// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Collections.Generic;

namespace ICSharpCode.Reporting.Test.Expressions.Aggregates
{
	/// <summary>
	/// Description of AggregateFuctionHelper.
	/// </summary>
	internal class AggregateFuctionHelper
	{
		
		public AggregateFuctionHelper()
		{
			this.AggregateCollection = new AggregateCollection();
			this.AggregateCollection.Add (new Aggregate("Value1",1,1.5));
			this.AggregateCollection.Add (new Aggregate("Value2",2,2.5));
			this.AggregateCollection.Add (new Aggregate("Value3",3,3.5));
			this.AggregateCollection.Add (new Aggregate("Value400",400,400.75));
		}
		
		
		public AggregateCollection AggregateCollection {get; private set;}
		
	}
	
	
	class AggregateCollection: List<Aggregate>
	{
	}
	
	
	class Aggregate {
		public Aggregate (string name,int intValue, double amount)
		{
			this.Name = name;
			this.IntValue = intValue;
			this.DoubleValue = amount;
		}
		
		public string Name {get;set;}
		public int IntValue {get;set;}
		public double DoubleValue {get;set;}
	}
}
