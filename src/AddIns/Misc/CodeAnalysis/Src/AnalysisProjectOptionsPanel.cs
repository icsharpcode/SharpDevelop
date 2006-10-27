// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeAnalysis
{
	public class AnalysisProjectOptionsPanel : AbstractProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			InitializeHelper();
			
			AnalysisProjectOptions po = new AnalysisProjectOptions();
			po.Dock = DockStyle.Fill;
			Controls.Add(po);
			
			ChooseStorageLocationButton btn;
			btn = helper.BindBoolean(po.EnableCheckBox, "RunCodeAnalysis", false).CreateLocationButton(po.EnableCheckBox);
			ConfigurationGuiBinding binding = po.CreateBinding();
			binding.RegisterLocationButton(btn);
			helper.AddBinding("CodeAnalysisRules", binding);
			
			Control ctl = helper.CreateConfigurationSelector();
			ctl.Dock = DockStyle.Top;
			Controls.Add(ctl);
		}
	}
}
