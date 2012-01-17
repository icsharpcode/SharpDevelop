// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Core;

namespace ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of QueryAssembly.
	/// </summary>
	public class QueryAssembly:BaseQuery
	{
		public QueryAssembly(AssemblyNode mainModule):base(mainModule)
		{
		}
		
		public override System.Collections.Generic.List<ItemWithFunc> GetQueryList()
		{
			List<ItemWithFunc> items = new List<ItemWithFunc>();
			items.Add(new ItemWithFunc()
			                     {
			                     	Description = "# of NameSpaces",
			                     	Action = ExecuteNotImplemented
			                     });
			
			return items;
		}
		
		
		private List<TreeMapViewModel> ExecuteNotImplemented()
		{
			MessageService.ShowMessage("Not Implemented yet","CodeQualityAnalysis");
			return null;
		}
	}
}
