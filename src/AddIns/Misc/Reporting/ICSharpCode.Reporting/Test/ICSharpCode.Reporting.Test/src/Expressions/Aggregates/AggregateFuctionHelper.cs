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
