// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Options
{
	/// <summary>
	/// Interaction logic for DebuggingOptionsPanel.xaml
	/// </summary>
	public partial class DebuggingOptionsPanel : OptionPanel
	{
		public DebuggingOptionsPanel()
		{
			InitializeComponent();
			ChbStepOverAllProperties_CheckedChanged(null, null);
		}
		
		/// <summary>
		/// If stepOverAllProperties is true when the panel is opened then the CheckChanged event is
		/// fired during the InitializeComponent method call before the remaining check boxes are created.
		/// So check if the remaining check boxes have been created.
		/// </summary>
		void ChbStepOverAllProperties_CheckedChanged(object sender, RoutedEventArgs e)
		{
			if (IsPanelInitialized()) {
				bool stepOverAllProperties = chbStepOverAllProperties.IsChecked.GetValueOrDefault(false);
				chbStepOverSingleLineProperties.IsEnabled = !stepOverAllProperties;
				chbStepOverFieldAccessProperties.IsEnabled = !stepOverAllProperties;
			}
		}
		
		bool IsPanelInitialized()
		{
			return chbStepOverSingleLineProperties != null;
		}
		
		public override bool SaveOptions()
		{
			bool result = base.SaveOptions();
			DebuggingOptions.ResetStatus(proc => proc.Debugger.ResetJustMyCodeStatus());
			return result;
		}
	}
}