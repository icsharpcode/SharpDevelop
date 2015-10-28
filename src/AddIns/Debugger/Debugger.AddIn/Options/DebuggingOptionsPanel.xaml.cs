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
using System.Linq;
using System.Windows;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.Options
{
	/// <summary>
	/// Interaction logic for DebuggingOptionsPanel.xaml
	/// </summary>
	public partial class DebuggingOptionsPanel : OptionPanel
	{
		IList<ExceptionFilterEntry> exceptionFilterList;
		
		public DebuggingOptionsPanel()
		{
			InitializeComponent();
		}
		
		public override bool SaveOptions()
		{
			bool result = base.SaveOptions();
			if (WindowsDebugger.CurrentDebugger != null)
				WindowsDebugger.CurrentDebugger.ReloadOptions();
			DebuggingOptions.Instance.ExceptionFilterList = exceptionFilterList;
			return result;
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			exceptionFilterList = DebuggingOptions.Instance.ExceptionFilterList.ToList();
			
			if (exceptionFilterList.Count == 0) {
				exceptionFilterList.Add(new ExceptionFilterEntry("System.Exception"));
			}
		}
		
		void ChooseExceptionsClick(object sender, RoutedEventArgs e)
		{
			var dialog = new ChooseExceptionsDialog(exceptionFilterList);
			dialog.Owner = Window.GetWindow(this);
			if (dialog.ShowDialog() == true) {
				exceptionFilterList = dialog.ExceptionFilterList;
			}
		}
	}
}