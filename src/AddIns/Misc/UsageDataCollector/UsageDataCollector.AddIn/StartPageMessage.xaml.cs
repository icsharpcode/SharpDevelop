// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.Core.Presentation;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Interaction logic for StartPageMessage.xaml
	/// </summary>
	public partial class StartPageMessage : UserControl
	{
		public StartPageMessage()
		{
			InitializeComponent();
			
			this.SetValueToExtension(HeaderProperty, new LocalizeExtension("AddIns.UsageDataCollector.Title"));
		}
		
		void Radio_Checked(object sender, RoutedEventArgs e)
		{
			saveButton.IsEnabled = true;
		}
		
		void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			bool accepted = acceptRadio.IsChecked == true;
			AnalyticsMonitor.Enabled = accepted;
			mainPanel.IsCollapsed = true;
			acceptedMessage.IsCollapsed = !accepted;
			declinedMessage.IsCollapsed = accepted;
		}
		
		public static readonly DependencyProperty HeaderProperty = HeaderedContentControl.HeaderProperty.AddOwner(typeof(StartPageMessage));
		
		public object Header {
			get { return GetValue(HeaderProperty); }
			set { SetValue(HeaderProperty, value); }
		}
	}
}
