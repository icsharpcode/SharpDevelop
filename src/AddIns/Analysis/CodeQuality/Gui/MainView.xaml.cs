// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.CodeQuality.Engine;
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
			
			context.AddAssemblyFiles(fileDialog.FileNames);
			matrix.Update(context.Analyze());
		}
	}
}