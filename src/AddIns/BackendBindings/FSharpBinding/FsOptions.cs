// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System.Windows.Forms;

namespace FSharpBinding
{
	public class FsOptions : AbstractProjectOptionPanel
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
