/*
 * Created by SharpDevelop.
 * User: Susanne Jooss
 * Date: 10.09.2005
 * Time: 17:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using SharpReportCore;

namespace ReportGenerator
{
	/// <summary>
	/// Description of IReportLayout.
	/// </summary>
	public interface IReportLayout{
		void BuildLayout ();
		void BuildLayout (ReportModel reportModel);
	}
}
