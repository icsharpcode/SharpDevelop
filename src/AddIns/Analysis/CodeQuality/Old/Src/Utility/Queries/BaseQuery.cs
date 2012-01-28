// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		public BaseQuery(AssemblyNode mainModule)
		{
			MainModule = mainModule;
		}
		protected string NameSpaceCount = "No of Namespaces";
		protected string InstructionsCount = "No of Instructions";
		protected string Complexity = "Complexity";
		protected string MethodsCount = "No of Methods";
		protected string FieldsCount = "No of Fields";
		protected string VariablesCount = "No of Variables";
		protected string TypesCount = "No of Types";
		
		protected AssemblyNode MainModule {get; private set;}
	
		public int TotalElements {get; private set;}
		public int RemovedElements {get; private set;}
				
		public virtual List<ItemWithFunc> GetQueryList ()
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
		
		
		protected void SetToolstripText (IEnumerable<TreeMapViewModel> list,string tooltipText)
		{
			foreach (var element in list) {
				element.ToolTipString = string.Format("{0} <{1}>",tooltipText , element.NumericValue);
			}
		}
	}
}
