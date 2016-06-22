/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05.08.2014
 * Time: 20:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Media;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.Reporting.Addin.ReportWizard
{
	/// <summary>
	/// Description of WizardHelper.
	/// </summary>
	class WizardHelper
	{
		
		public static ImageSource GetWizardIcon () {
			return IconService.GetImageSource("GeneralWizardBackground");
		}
	}
}
