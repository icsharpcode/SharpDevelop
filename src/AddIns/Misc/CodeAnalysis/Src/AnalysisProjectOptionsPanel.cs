/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 28.02.2006
 * Time: 15:29
 */

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
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
