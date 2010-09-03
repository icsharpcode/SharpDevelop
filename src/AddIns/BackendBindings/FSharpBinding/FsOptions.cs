// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System.Windows.Forms;

namespace FSharpBinding
{
	public class FsOptions : AbstractXmlFormsProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(typeof(FsOptions).Assembly.GetManifestResourceStream("FSharpBinding.Resources.FsOptions.xfrm"));
			InitializeHelper();
			helper.BindBoolean(Get<CheckBox>("standalone"), "Standalone", false);
			helper.BindBoolean(Get<CheckBox>("nomllib"), "NoMLLib", false);
		}
	}
}
