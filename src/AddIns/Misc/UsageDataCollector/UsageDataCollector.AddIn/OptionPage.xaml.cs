// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
