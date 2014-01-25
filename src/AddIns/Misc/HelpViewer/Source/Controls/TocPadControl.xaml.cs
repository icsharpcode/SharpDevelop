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
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.Core;
using MSHelpSystem.Core;

namespace MSHelpSystem.Controls
{
	public partial class TocPadControl : UserControl
	{
		public TocPadControl()
		{
			InitializeComponent();
			LoadToc();
			Help3Service.ConfigurationUpdated += new EventHandler(Help3ServiceConfigurationUpdated);
		}

		void LoadToc()
		{
			if (!Help3Environment.IsLocalHelp) DataContext = null;
			else DataContext = new[] { new TocEntry("-1") { Title = StringParser.Parse("${res:AddIns.HelpViewer.HelpLibraryRootTitle}") } };
		}

		void Help3TocItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			string topicId = (string)tocTreeView.SelectedValue;
			if (!string.IsNullOrEmpty(topicId)) {
				LoggingService.Debug(string.Format("HelpViewer: TocItemChanged to ID \"{0}\"", topicId));
				DisplayHelp.Page(topicId);
			}
		}
		
		void Help3ServiceConfigurationUpdated(object sender, EventArgs e)
		{
			LoadToc();
		}
	}
}
