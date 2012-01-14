/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 25.12.2011
 * Time: 19:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Core;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of testclass.
	/// </summary>
	public class QueryMethod:BaseQuery
	{
		public QueryMethod(Module mainModule):base(mainModule)
		{	
		}
		
		private List <Method> MethodQuery()
		{
			IEnumerable<Method> query  = new List<Method>();
			query  = from ns in MainModule.Namespaces
				from type in ns.Types
				from method in type.Methods
				select method;
			return query.ToList();
		}
		
		
		public override List<ItemWithFunc> GetQueryList()
		{
			List<ItemWithFunc> items = new List<ItemWithFunc>();
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of IL Instructions",			                 
			                     	Action = ExecuteMethodILInstructions
			                     });
		
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "IL Cyclomatic Complexity",
			                     	Action = ExecuteMethodComplexity
			                     });
			                
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of Variables",
			                     	Action = ExecuteMethodVariables
			                     });
			                   
			return items;
		}
		
		
		private List<TreeMapViewModel> ExecuteMethodILInstructions()
		{
			var intermediate = MethodQuery();
			int i = 0;
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count)
			                               });

			var filtered = base.EliminateZeroValues(list);
			Console.WriteLine("{0} - {1} - {2}",intermediate.Count,list.Count(),filtered.Count());
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteMethodComplexity ()
		{
			var intermediate = MethodQuery();

			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.CyclomaticComplexity
			                               });
			var filtered = base.EliminateZeroValues(list);
			Console.WriteLine("{0} - {1} - {2}",intermediate.Count,list.Count(),filtered.Count());
			return filtered.ToList();
		}
	
		
		private List<TreeMapViewModel> ExecuteMethodVariables ()
		{
			var intermediate = MethodQuery();
			
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue =  m.Variables
			                               });
			var filtered = base.EliminateZeroValues(list);
			Console.WriteLine("{0} - {1} - {2}",intermediate.Count,list.Count(),filtered.Count());
			return filtered.ToList();
		}
	}
}
