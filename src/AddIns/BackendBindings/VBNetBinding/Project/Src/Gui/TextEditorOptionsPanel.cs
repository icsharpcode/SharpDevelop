// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace VBBinding
{
	public class TextEditorOptionsPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\VBSpecificTextEditorOptions.xfrm"));
			((CheckBox)ControlDictionary["enableEndConstructsCheckBox"]).Checked   = PropertyService.Get("VBBinding.TextEditor.EnableEndConstructs", true);
			((CheckBox)ControlDictionary["enableCasingCheckBox"]).Checked = PropertyService.Get("VBBinding.TextEditor.EnableCasing", true);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.Set("VBBinding.TextEditor.EnableEndConstructs", ((CheckBox)ControlDictionary["enableEndConstructsCheckBox"]).Checked);
			PropertyService.Set("VBBinding.TextEditor.EnableCasing", ((CheckBox)ControlDictionary["enableCasingCheckBox"]).Checked);
			
			return true;
		}
	}
}
