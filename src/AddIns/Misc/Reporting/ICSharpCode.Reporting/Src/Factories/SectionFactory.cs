/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.03.2014
 * Time: 17:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Factories
{
	/// <summary>
	/// Description of SectionFactory.
	/// </summary>
	static class SectionFactory
	{

		public static BaseSection Create(string name) {
			if (name == null)
				throw new ArgumentNullException("name");
			
			return new BaseSection(name);
		}
		
	}
}
