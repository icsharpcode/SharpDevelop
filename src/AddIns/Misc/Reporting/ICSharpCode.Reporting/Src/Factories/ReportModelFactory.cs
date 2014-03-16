/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 16.03.2014
 * Time: 17:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Reporting.Globals;
using ICSharpCode.Reporting.Items;

namespace ICSharpCode.Reporting.Factories
{
	/// <summary>
	/// Description of ReportModelFactory.
	/// </summary>
	public class ReportModelFactory
	{
		public static ReportModel Create() 
		{
			ReportModel m = new ReportModel();
			foreach (GlobalEnums.ReportSection sec in Enum.GetValues(typeof(GlobalEnums.ReportSection))) {
				m.SectionCollection.Add (SectionFactory.Create(sec.ToString()));
			}
			return m;
		}
	}
}
