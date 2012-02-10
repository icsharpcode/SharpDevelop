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
		
		public MainView()
		{
			InitializeComponent();
			
			context = new AssemblyAnalyzer();
			this.DataContext = context;
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
			context.AddAssemblyFiles(fileDialog.FileNames);
			RefreshClick(null, null);
		}
		
		ReadOnlyCollection <AssemblyNode> list;
		
		void RefreshClick(object sender, RoutedEventArgs e)
		{
			introBlock.Visibility = Visibility.Collapsed;
			using (context.progressMonitor = AsynchronousWaitDialog.ShowWaitDialog("Analysis"))
				list = context.Analyze();
			Report(list);
				matrix.Update(list);
			matrix.Visibility = Visibility.Visible;
		}
		
	/*
		void RefreshClick(object sender, RoutedEventArgs e)
		{
			introBlock.Visibility = Visibility.Collapsed;
			using (context.progressMonitor = AsynchronousWaitDialog.ShowWaitDialog("Analysis"))
				matrix.Update(context.Analyze());
			matrix.Visibility = Visibility.Visible;
		}
		*/
		
		
		void Report (ReadOnlyCollection<AssemblyNode> list)
		{
			foreach (var ass in list)
			{
				
				Console.WriteLine("{0} - {1}",ass.Name,ass.namespaces);
				foreach (var child in ass.Children)
				{
					Console.WriteLine("\t {0}",child.Name);
					foreach (var subchild in child.Children) {
						Console.WriteLine("\t\t {0}",subchild.Name);
					}
				}
			}
		}
	}
}