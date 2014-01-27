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
using ICSharpCode.SharpDevelop.Project;
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
		
		void AddCurrentProjectAssemblyClick(object sender, RoutedEventArgs e)
		{
			if (ProjectService.CurrentProject == null)
				return;
			
			string fileName = ProjectService.CurrentProject.OutputAssemblyFullPath;
			if (string.IsNullOrEmpty(fileName))
			{
				MessageBox.Show("Project output assembly not found! Please build it first!");
				return;
			}

			introBlock.Visibility = Visibility.Collapsed;
			this.fileNames.Add(fileName);
			Analyse(new string[] { fileName });
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
