// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.Core;

namespace ICSharpCode.Profiler.AddIn.Dialogs
{
	/// <summary>
	/// Interaction logic for ProfilerControlWindow.xaml
	/// </summary>
	public partial class ProfilerControlWindow : Window
	{
		ProfilerRunner runner;
		public bool AllowClose { get; set; }
		
		public ProfilerControlWindow(ProfilerRunner runner)
		{
			InitializeComponent();
			
			this.runner = runner;
			this.collectData.IsChecked = runner.Profiler.ProfilerOptions.EnableDCAtStart;
		}
		
		void CollectDataChecked(object sender, RoutedEventArgs e)
		{
			try {
				runner.Profiler.EnableDataCollection();
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		void CollectDataUnchecked(object sender, RoutedEventArgs e)
		{
			try {
				runner.Profiler.DisableDataCollection();
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		void ShutdownClick(object sender, RoutedEventArgs e)
		{
			AllowClose = true;
			runner.Stop();
		}
		
		void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = !AllowClose;
		}
	}
}
