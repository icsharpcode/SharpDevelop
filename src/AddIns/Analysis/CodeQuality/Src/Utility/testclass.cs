/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 25.12.2011
 * Time: 19:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.CodeQualityAnalysis.Utility
{
	/// <summary>
	/// Description of testclass.
	/// </summary>
	public class testclass
	{
		public testclass(Module mainModule)
		{
			MainModule = mainModule;
		}
		
		public Module MainModule {get; private set;}
		
		public ObservableCollection <INode> QueryMethod()
		{
			IEnumerable<INode> list  = new List<INode>();
			list  = from ns in MainModule.Namespaces
				from type in ns.Types
				from method in type.Methods
				select method;
			return new  ObservableCollection <INode>(list);
		}
	}
}
