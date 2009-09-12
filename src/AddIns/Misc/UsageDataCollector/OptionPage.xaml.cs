// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Interaction logic for OptionPage.xaml
	/// </summary>
	public partial class OptionPage : OptionPanel
	{
		public OptionPage()
		{
			InitializeComponent();
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			if (!AnalyticsMonitor.EnabledIsUndecided) {
				if (AnalyticsMonitor.Enabled)
					acceptRadio.IsChecked = true;
				else
					declineRadio.IsChecked = true;
			}
		}
		
		public override bool SaveOptions()
		{
			if (acceptRadio.IsChecked ?? false)
				AnalyticsMonitor.Enabled = true;
			else if (declineRadio.IsChecked ?? false)
				AnalyticsMonitor.Enabled = false;
			return base.SaveOptions();
		}
	}
}