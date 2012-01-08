/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 03.01.2012
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using System.Linq;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of QueryType.
	/// </summary>
	public class QueryType:BaseQuery
	{
		public QueryType(Module mainModule):base(mainModule)
		{
		}
		
		private List <Type> TypeQuery()
		{
			IEnumerable<Type> query  = new List<Type>();
			query  = from ns in MainModule.Namespaces
				from type in ns.Types
				//from method in type.Methods
				select type;
			return query.ToList();
		}
		
		public override System.Collections.Generic.List<ItemWithAction> GetQueryList()
		{
			List<ItemWithAction> items = new List<ItemWithAction>();
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of IL Instructions",
			                     	Metrics =  "Instructions.Count",
			                     	Action = ExecuteILInstructions
			                     });
			
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "IL Cyclomatic Complexity",
			                     	Metrics = Metrics.CyclomaticComplexity.ToString(),
			                     	Action = ExecuteMethodComplexity
			                     });
			                   
			items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Methods",
			                     	Metrics = Metrics.CyclomaticComplexity.ToString(),
			                     	Action = ExecuteNumberOfMethods
			                     });
			                       
				items.Add(new ItemWithAction()
			                     {
			                     	Description = "# of Fields",
			                     	Metrics = Metrics.Variables.ToString(),
			                     	Action = ExecuteNumberOfFields
			                     });
		
			return items;
		}
		
		
		private List<TreeMapViewModel> ExecuteILInstructions ()
		{
			var intermediate = this.TypeQuery();
			int i = 0;
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	Numval = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count)
			                               });
			var filtered = base.EliminateZeroValues(list);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteMethodComplexity ()
		{
			var intermediate = this.TypeQuery();
			int i = 0;
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	Numval = m.GetAllMethods().Aggregate(i, (current, x) => current + x.CyclomaticComplexity)
			                               });
			var filtered = base.EliminateZeroValues(list);
			Console.WriteLine("{0} - {1} - {2}",intermediate.Count,list.Count(),filtered.Count());
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteNumberOfMethods ()
		{
			var intermediate = this.TypeQuery();
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	Numval = m.GetAllMethods().ToList().Count
			                               });
		
			var filtered = base.EliminateZeroValues(list);
			Console.WriteLine("{0} - {1} - {2}",intermediate.Count,list.Count(),filtered.Count());
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteNumberOfFields ()
		{
			var intermediate = this.TypeQuery();
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	Numval = m.GetAllFields().ToList().Count
			                               });
			var filtered = base.EliminateZeroValues(list);
			Console.WriteLine("{0} - {1} - {2}",intermediate.Count,list.Count(),filtered.Count());
			return filtered.ToList();
		}
	}
}
