// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace VBNetBinding.OptionPanels
{
	public class VBNetTextEditorPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("Resources.VBNetTextEditorOptions.xfrm"));
			Get<CheckBox>("enableEndConstructs").Checked = PropertyService.Get<bool>("VBBinding.TextEditor.EnableEndConstructs", true);
			Get<CheckBox>("enableCasing").Checked = PropertyService.Get<bool>("VBBinding.TextEditor.EnableCasing", true);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.Set<bool>("VBBinding.TextEditor.EnableEndConstructs", Get<CheckBox>("enableEndConstructs").Checked);
			PropertyService.Set<bool>("VBBinding.TextEditor.EnableCasing", Get<CheckBox>("enableCasing").Checked);
			return true;
		}
	}
}
