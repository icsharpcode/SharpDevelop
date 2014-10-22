/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.08.2014
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.Reporting.BaseClasses;

namespace ICSharpCode.Reporting.Addin.ReportWizard.ViewModels
{
	/// <summary>
	/// Description of PushModelContent.
	/// </summary>
	/// 
	public class PushModelContext:IWizardContext
	{
		public List<AbstractColumn> Items {get;set;}
	}
	
}
