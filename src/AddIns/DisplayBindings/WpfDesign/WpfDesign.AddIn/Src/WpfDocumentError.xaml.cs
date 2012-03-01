// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.AddIn
{
	/// <summary>
	/// A friendly error window displayed when the WPF document does not parse correctly.
	/// </summary>
	public partial class WpfDocumentError : UserControl
	{
		public WpfDocumentError(Exception e = null)
		{
			InitializeComponent();
			if (e != null)
				additionalInfo.Text = e.ToString();
			else
				additionalInfoBox.Visibility = Visibility.Collapsed;
		}
		
		void ViewXaml(object sender,RoutedEventArgs e)
		{
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.SwitchView(0);
		}
	}
}
