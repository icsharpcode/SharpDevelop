// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

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
			AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(OnRequestNavigate));
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
			showCollectedDataButton.IsEnabled = acceptRadio.IsChecked == true;
		}
		
		public override bool SaveOptions()
		{
			if (acceptRadio.IsChecked == true)
				AnalyticsMonitor.Enabled = true;
			else if (declineRadio.IsChecked == true)
				AnalyticsMonitor.Enabled = false;
			return base.SaveOptions();
		}
		
		void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			e.Handled = true;
			try {
				Process.Start(e.Uri.ToString());
			} catch {
				// catch exceptions - e.g. incorrectly installed web browser
			}
		}
		
		void ShowCollectedDataButton_Click(object sender, RoutedEventArgs e)
		{
			string data = AnalyticsMonitor.Instance.GetTextForStoredData();
			(new CollectedDataView(data) { Owner = Window.GetWindow(this), ShowInTaskbar = false }).ShowDialog();
		}
	}
}
