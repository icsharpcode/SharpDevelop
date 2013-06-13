/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 23.05.2013
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
namespace ICSharpCode.Reporting.DataSource.Comparer
{
	/// <summary>
	/// Description of SortExtension.
	/// </summary>
	internal static class SortExtension
	{
		
		public static IOrderedQueryable<BaseComparer> AscendingOrder(this IQueryable<BaseComparer> source )
		{  
			
			return source.OrderBy(x => x.ObjectArray[0]);
		}
		
		public static IOrderedQueryable<BaseComparer> DescendingOrder(this IQueryable<BaseComparer> source )
		{  
			
			return source.OrderByDescending(x => x.ObjectArray[0]);
		}
	}
	
}
