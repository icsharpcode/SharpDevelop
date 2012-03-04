// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.CodeQuality.Engine;
using ICSharpCode.CodeQuality.Engine.Dom;
using ICSharpCode.CodeQuality.Reporting;
using ICSharpCode.Reports.Core.WpfReportViewer;
using ICSharpCode.SharpDevelop.Gui;
using Microsoft.Win32;

namespace ICSharpCode.CodeQuality.Gui
{
	/// <summary>
	/// Interaction logic for MainView.xaml
	/// </summary>
	public partial class MainView : UserControl
	{
		AssemblyAnalyzer context;
		List<string> fileNames;
		ReadOnlyCollection <AssemblyNode> list;
		
		
		public MainView()
		{
			InitializeComponent();
			
			context = new AssemblyAnalyzer();
			this.DataContext = context;
			fileNames = new List<string>();
		}
		
		void AddAssemblyClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog fileDialog = new OpenFileDialog {
				Filter = "Component Files (*.dll, *.exe)|*.dll;*.exe",
				Multiselect = true
			};

			if (fileDialog.ShowDialog() != true || fileDialog.FileNames.Length == 0)
				return;
			introBlock.Visibility = Visibility.Collapsed;
			this.fileNames.AddRange(fileDialog.FileNames);
			Analyse(fileDialog.FileNames);
			UpdateUI();
		}
		
		
		void Analyse (string[] fileNames)
		{
			context.AddAssemblyFiles(fileNames);
			using (context.progressMonitor = AsynchronousWaitDialog.ShowWaitDialog("Analysis"))
			{
				list = context.Analyze();
			}
		}
		
		
		void UpdateUI ()
		{
			matrix.Update(list);
			introBlock.Visibility = Visibility.Collapsed;
			matrix.Visibility = Visibility.Visible;
			printMenu.Visibility = Visibility.Visible;
		}
		
		
		void OverviewReport_Click(object sender, RoutedEventArgs e)
		{
			var overviewReport = new OverviewReport(fileNames);
			var reportCreator = overviewReport.Run(list);
			var previewViewModel = new PreviewViewModel(overviewReport.ReportSettings,reportCreator.Pages);
			viewer.SetBinding(previewViewModel);
			ActivateReportTab();
		}

		
		void DependecyReport_Click(object sender, RoutedEventArgs e)
		{
			var dependencyReport = new DependencyReport(fileNames);
			var reportCreator = dependencyReport.Run(list);
			var previewViewModel = new PreviewViewModel(dependencyReport.ReportSettings,reportCreator.Pages);
			viewer.SetBinding(previewViewModel);
			ActivateReportTab();
		}
		
		void ActivateReportTab()
		{
			reportTab.Visibility = Visibility.Visible;
			mainTab.SelectedItem = reportTab;
		}
	}
}