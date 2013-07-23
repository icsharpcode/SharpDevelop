// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XamlBinding.Options
{
	/// <summary>
	/// Interaction logic for CodeCompletion.xaml
	/// </summary>
	public partial class CodeCompletion : OptionPanel
	{
		public CodeCompletion()
		{
			InitializeComponent();
		}
		
		public override bool SaveOptions()
		{
			if (base.SaveOptions()) {
//				XamlColorizer.RefreshAll();
				return true;
			}
			
			return false;
		}
	}
}
