// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.PackageManagement
{
	public partial class SelectProjectsView : Window
	{
		public SelectProjectsView()
		{
			InitializeComponent();
		}
		
		void AcceptButtonClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}