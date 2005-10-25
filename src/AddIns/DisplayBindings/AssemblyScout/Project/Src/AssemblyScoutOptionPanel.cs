// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	public class AssemblyScoutOptionPanel : AbstractOptionPanel
	{
		Control[] combos = new Control[] {};
		
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("AssemblyScoutOptions.xfrm"));
			
			combos = new Control[] {
				ControlDictionary["privateTypesBox"],
				ControlDictionary["internalTypesBox"],
				ControlDictionary["privateMembersBox"],
				ControlDictionary["internalMembersBox"]
			};
			
			foreach(ComboBox combo in combos) {
				Debug.Assert(combo != null);
				combo.Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.AssemblyScout.Show}"));
				combo.Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.AssemblyScout.GreyOut}"));
				combo.Items.Add(StringParser.Parse("${res:Dialog.Options.IDEOptions.AssemblyScout.Hide}"));
				
				combo.SelectedIndex = PropertyService.Get("AddIns.AssemblyScout." + combo.Name, 1);
			}
			
			((CheckBox)ControlDictionary["showReturnTypesBox"]).Checked     
				= PropertyService.Get("AddIns.AssemblyScout.ShowReturnTypes", true);
			((CheckBox)ControlDictionary["showResourcePreviewBox"]).Checked 
				= PropertyService.Get("AddIns.AssemblyScout.ShowResPreview", true);
			((CheckBox)ControlDictionary["showSpecialMethodsBox"]).Checked 
				= PropertyService.Get("AddIns.AssemblyScout.ShowSpecialMethods", true);
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.Set("AddIns.AssemblyScout.ShowReturnTypes", ((CheckBox)ControlDictionary["showReturnTypesBox"]).Checked);
			PropertyService.Set("AddIns.AssemblyScout.ShowResPreview",  ((CheckBox)ControlDictionary["showResourcePreviewBox"]).Checked);
			PropertyService.Set("AddIns.AssemblyScout.ShowSpecialMethods", ((CheckBox)ControlDictionary["showSpecialMethodsBox"]).Checked);
			
			foreach(ComboBox combo in combos) {
				PropertyService.Set("AddIns.AssemblyScout." + combo.Name, combo.SelectedIndex);
			}
			
			return true;
		}
	}
}
