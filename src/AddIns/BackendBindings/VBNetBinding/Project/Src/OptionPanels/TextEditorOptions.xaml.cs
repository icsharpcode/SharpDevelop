// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.Core;

namespace ICSharpCode.VBNetBinding.OptionPanels
{
	/// <summary>
	/// Interaction logic for TextEditorOptions.xaml
	/// </summary>
	public partial class TextEditorOptions
	{
		public TextEditorOptions()
		{
			InitializeComponent();
		}
		
		public static bool EnableEndConstructs {
			get { return PropertyService.Get("VBBinding.TextEditor.EnableEndConstructs", true); }
			set { PropertyService.Set("VBBinding.TextEditor.EnableEndConstructs", value); }
		}
		
		public static bool EnableCasing {
			get { return PropertyService.Get("VBBinding.TextEditor.EnableCasing", true); }
			set { PropertyService.Set("VBBinding.TextEditor.EnableCasing", value); }
		}
	}
}
