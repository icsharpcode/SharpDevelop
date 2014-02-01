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
using System.Reflection;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.StartPage
{
	/// <summary>
	/// Interaction logic for StartPageControl.xaml
	/// </summary>
	public partial class StartPageControl : UserControl
	{
		public StartPageControl()
		{
			InitializeComponent();
			List<object> items = AddInTree.BuildItems<object>("/SharpDevelop/ViewContent/StartPage/Items", this, false);
			// WPF does not use DataTemplates if the item already is a UIElement; so we 'box' it.
			List<BoxEntry> entries = items.ConvertAll(control => new BoxEntry { Control = control } );
			startPageItems.ItemsSource = entries;
			
			var aca = (AssemblyCopyrightAttribute)typeof(CommonAboutDialog).Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
			copyrightText.Text = aca.Copyright;
			
			versionTextBlock.Text = "SharpDevelop " + RevisionClass.FullVersion;
		}
		
		sealed class BoxEntry
		{
			public object Control { get; set; }
		}
	}
}
