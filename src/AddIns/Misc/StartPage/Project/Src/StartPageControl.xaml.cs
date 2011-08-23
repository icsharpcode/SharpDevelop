// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
