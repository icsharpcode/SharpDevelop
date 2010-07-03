// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Peter Forstmeier" email="peter.forstmeier@t-online.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.ObjectModel;

/// <summary>
///
/// </summary>
/// <remarks>
/// 	created by - Forstmeier Peter
/// 	created on - 28.11.2005 13:43:22
/// </remarks>

namespace ICSharpCode.Reports.Core
{
	public  class GroupComparer :SortComparer
	{
		
		public GroupComparer (Collection<AbstractColumn> owner, int listIndex, object[] values):base(owner,listIndex,values)
		{
			IndexList = new IndexList();
		}
		
		public IndexList IndexList {get;set;}
	}
}
