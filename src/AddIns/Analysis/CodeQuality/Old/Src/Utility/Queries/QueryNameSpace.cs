// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;
using System.Linq;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of QueryNameSpace.
	/// </summary>
	public class QueryNameSpace:BaseQuery
	{
		public QueryNameSpace(AssemblyNode mainModule):base (mainModule)
		{
		}
		
		private List<NamespaceNode> NameSpaceQuery()
		{
			IEnumerable<NamespaceNode> query = from ns
				in MainModule.Namespaces
				select ns;
			return query.ToList();
		}
		
		public override List<ItemWithFunc> GetQueryList()
		{
			List<ItemWithFunc> items = new List<ItemWithFunc>();
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of IL Instructions",
			                     	Action = ExecuteILInstructions
			                     });
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of Methods",
			                     	Action = ExecuteMethodsCount
			                     });
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of Fields",
			                     	Action = ExecuteFieldsCount
			                     });
			
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of Types",
			                     	Action = ExecuteTypesCount
			                     });
			return items;
		}
		
		
		private List<TreeMapViewModel> ExecuteILInstructions ()
		{
			var intermediate = this.NameSpaceQuery();
			var i = 0;
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
<<<<<<< HEAD
			                               	NumericValue = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count)
=======
//			                               	Numval = m.GetAllMethods().Aggregate(i, (current, x) => current + x.Instructions.Count)
>>>>>>> started implementation of new DOM based on NRefactory.TypeSystem
			                               });
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText (filtered,base.InstructionsCount);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteMethodsCount()
		{
			var intermediate = this.NameSpaceQuery();
			
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
<<<<<<< HEAD
			                               	NumericValue = m.GetAllMethods().ToList().Count
=======
//			                               	Numval = m.GetAllMethods().ToList().Count
>>>>>>> started implementation of new DOM based on NRefactory.TypeSystem
			                               });
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText (filtered,base.MethodsCount);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteFieldsCount()
		{
			var intermediate = this.NameSpaceQuery();
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
<<<<<<< HEAD
			                               	NumericValue = m.GetAllFields().ToList().Count
=======
//			                               	Numval = m.GetAllFields().ToList().Count
>>>>>>> started implementation of new DOM based on NRefactory.TypeSystem
			                               });
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText (filtered,base.FieldsCount);
			return filtered.ToList();
		}
		
		
		private List<TreeMapViewModel> ExecuteTypesCount()
		{
			var intermediate = this.NameSpaceQuery();
			
			var list = intermediate.Select(m =>  new TreeMapViewModel()
			                               {
			                               	Name = m.Name,
<<<<<<< HEAD
			                               	NumericValue = m.GetAllTypes().ToList().Count
=======
//			                               	Numval = m.GetAllTypes().ToList().Count
>>>>>>> started implementation of new DOM based on NRefactory.TypeSystem
			                               });
			var filtered = base.EliminateZeroValues(list);
			base.SetToolstripText (filtered,base.TypesCount);
			return filtered.ToList();
		}
	}
}
