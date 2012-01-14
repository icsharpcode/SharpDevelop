/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 02.01.2012
 * Time: 20:26
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of BaseQuery.
	/// </summary>
	public class BaseQuery
	{
		public BaseQuery(Module mainModule)
		{
			MainModule = mainModule;
		}
		
		protected Module MainModule {get; private set;}
	
		public int TotalElements {get; private set;}
		public int RemovedElements {get; private set;}
				
		public virtual List<ItemWithAction> GetQueryList ()
		{
			return null;
		}
		
		
		protected List<TreeMapViewModel> EliminateZeroValues(IEnumerable<TreeMapViewModel> list)
		{
			var filtered =  from viewModel in list
				where viewModel.NumericValue > 0
				select viewModel;
			var result = filtered.ToList();
			TotalElements = list.Count();
			RemovedElements = TotalElements - result.Count();
			return result;
		}
	}
}
