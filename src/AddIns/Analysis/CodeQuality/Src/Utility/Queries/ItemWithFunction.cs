/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 14.01.2012
 * Time: 18:54
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;

namespace  ICSharpCode.CodeQualityAnalysis.Utility.Queries
{
	/// <summary>
	/// Description of ItemWithFunction.
	/// </summary>
	public class ItemWithFunc
	{
		public ItemWithFunc()
		{
		}
		public string Description	{get; set;}
		public Func<List<TreeMapViewModel>>  Action {get; set;}
		public  string Metrics {get;set;}
	}
}
