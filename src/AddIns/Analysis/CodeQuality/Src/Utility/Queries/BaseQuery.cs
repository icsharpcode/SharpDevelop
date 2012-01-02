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
		
		public Module MainModule {get; private set;}
		
		public virtual List<ItemWithAction> GetQueryList ()
		{
			throw new InvalidOperationException("Must override");
		}
	}
}
