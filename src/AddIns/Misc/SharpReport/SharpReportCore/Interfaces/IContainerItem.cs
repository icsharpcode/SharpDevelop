/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 06.03.2006
 * Time: 09:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Forms;
using System.Collections.ObjectModel;
namespace SharpReportCore
{
	/// <summary>
	/// Description of IContainerControl.
	/// </summary>
	public interface IContainerItem{
	
		bool IsValidChild (BaseReportItem childControl);
		
		Padding Padding
		{get;set;}
		
		ReportItemCollection Items
		{get;}
	}
}
