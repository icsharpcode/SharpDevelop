// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)
using System;
using System.Windows.Controls;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public partial class SharpDevelopUIOptions : OptionPanel
	{
		public SharpDevelopUIOptions()
		{
			InitializeComponent();
		}
		
		public static bool ExpandReferences {
			get { return PropertyService.Get("ExpandReferences", false); }
			set { PropertyService.Set("ExpandReferences", value); }
		}
	}
}