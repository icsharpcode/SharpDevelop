// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		public QueryType(AssemblyNode mainModule):base(mainModule)
		{
		}
		
		private List <Type> TypeQuery()
		{
			IEnumerable<Type> query  = new List<Type>();
			query  = from ns in MainModule.Namespaces
				from type in ns.Types
				select type;
			return query.ToList();
		}
		
		public override System.Collections.Generic.List<ItemWithFunc> GetQueryList()
		{
			List<ItemWithFunc> items = new List<ItemWithFunc>();
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of IL Instructions",
			                     	Action = ExecuteILInstructions
			                     });
			
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "IL Cyclomatic Complexity",
			                     	Action = ExecuteMethodComplexity
			                     });
			                   
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of Methods",
			                     	Action = ExecuteNumberOfMethods
			                     });
			                       
				items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of Fields",
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
			                               	NumericValue = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count),
			                               });
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText (filtered,base.InstructionsCount);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteMethodComplexity ()
		{
			var intermediate = this.TypeQuery();
			int i = 0;
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.GetAllMethods().Aggregate(i, (current, x) => current + x.CyclomaticComplexity)
			                               });
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText (filtered,base.Complexity);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteNumberOfMethods ()
		{
			var intermediate = this.TypeQuery();
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.GetAllMethods().ToList().Count
			                               });
		
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText (filtered,base.MethodsCount);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteNumberOfFields ()
		{
			var intermediate = this.TypeQuery();
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
			                               	NumericValue = m.GetAllFields().ToList().Count
			                               });
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText (filtered,base.FieldsCount);
			return filtered.ToList();
		}
	}
}
