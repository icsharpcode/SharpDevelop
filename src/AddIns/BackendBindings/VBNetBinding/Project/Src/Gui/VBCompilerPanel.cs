// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Markus Palme" email="MarkusPalme@gmx.de"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace VBBinding
{
	public class VBCompilerPanel : AbstractOptionPanel
	{
		VBCompilerParameters config = null;
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\VBCompilerPanel.xfrm"));
			
			this.config = (VBCompilerParameters)((Properties)CustomizationObject).Get("Config");
			
			
			((ComboBox)ControlDictionary["compilerVersionComboBox"]).Items.Add("Standard");
			foreach (string runtime in FileUtility.GetAvaiableRuntimeVersions()) {
				((ComboBox)ControlDictionary["compilerVersionComboBox"]).Items.Add(runtime);
			}
			
			((ComboBox)ControlDictionary["compilerVersionComboBox"]).Text = config.VBCompilerVersion.Length == 0 ? "Standard" : config.VBCompilerVersion;
		}

		public override bool StorePanelContents()
		{
			config.VBCompilerVersion = ControlDictionary["compilerVersionComboBox"].Text;
			return true;
		}
	}
}
