// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeAnalysis
{
	public class AnalysisProjectOptionsPanel : AbstractXmlFormsProjectOptionPanel
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
