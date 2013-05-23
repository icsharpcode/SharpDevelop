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
		}
		
		public override bool SaveOptions()
		{
			bool result = base.SaveOptions();
			if (WindowsDebugger.CurrentDebugger != null)
				WindowsDebugger.CurrentDebugger.ReloadOptions();
			return result;
		}
	}
}