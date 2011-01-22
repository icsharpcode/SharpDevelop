// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;

namespace ICSharpCode.PackageManagement
{
	public partial class LicenseAcceptanceView : Window
	{
		public LicenseAcceptanceView()
		{
			InitializeComponent();
		}
		
		void AcceptButtonClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}