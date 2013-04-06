/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.04.2013
 * Time: 20:08
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
	internal sealed class SectionFactory
	{
		private SectionFactory ()
		{
			
		}
		public static BaseSection Create(string sectionName) {
			if (String.IsNullOrEmpty(sectionName)) {
				throw new ArgumentException("sectionName");
			}
			return new BaseSection(sectionName);
		}
	}
}
