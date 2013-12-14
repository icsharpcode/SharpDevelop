/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 19.03.2013
 * Time: 20:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Xml
{
	/// <summary>
	/// Description of ModelLoader.
	/// </summary>
	internal class ModelLoader: MycroParser
	{
		protected override Type GetTypeByName(string ns, string name)
		{
			return typeof(BaseSection).Assembly.GetType(typeof(BaseSection).Namespace + "." + name);
		}
	}
}
