// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Kumar Devvrat"/>
//     <version>$Revision: $</version>
// </file>
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
		public WpfDocumentError()
		{
			InitializeComponent();
		}
		
		void ViewXaml(object sender,RoutedEventArgs e)
		{
			WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.SwitchView(0);
		}
	}
}