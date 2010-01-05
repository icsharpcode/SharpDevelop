/*
 * Created by SharpDevelop.
 * User: PeterForstmeier
 * Date: 3/31/2007
 * Time: 2:31 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace ICSharpCode.Reports.Core
{
	public class BaseItemLoader : MycroParser
	{
		protected override Type GetTypeByName(string ns, string name)
		{
			return typeof(BaseSection).Assembly.GetType(typeof(BaseSection).Namespace + "." + name);
		}
	}
}
