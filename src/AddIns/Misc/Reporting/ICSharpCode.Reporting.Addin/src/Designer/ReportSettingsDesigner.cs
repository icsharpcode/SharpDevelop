/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 15.03.2014
 * Time: 18:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace ICSharpCode.Reporting.Addin.Designer
{
	/// <summary>
	/// Description of ReportSettingsDesigner.
	/// </summary>
	class ReportSettingsDesigner:ComponentDesigner
	{
		static string settingsName = "ReportSettings";
		public ReportSettingsDesigner()
		{
		}
		
		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			component.Site.Name = ReportSettingsDesigner.settingsName;
		}
	}
}
