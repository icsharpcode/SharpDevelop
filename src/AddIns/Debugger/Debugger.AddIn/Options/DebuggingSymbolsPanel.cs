// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System.Windows.Forms;
using Debugger;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Options panel which allows user to specify where to look
	/// for symbols (pdb files) and source codes
	/// </summary>
	public partial class DebuggingSymbolsPanel : XmlFormsOptionPanel
	{
		public DebuggingSymbolsPanel()
		{
			InitializeComponent();
			foreach (Control ctl in Controls.GetRecursive()) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
		}
		
		public override void LoadPanelContents()
		{
			pathList.LoadList(DebuggingOptions.Instance.SymbolsSearchPaths);
		}
		
		public override bool StorePanelContents()
		{
			DebuggingOptions.Instance.SymbolsSearchPaths = pathList.GetList();
			DebuggingOptions.ResetStatus(
				proc => {
					proc.Debugger.ReloadModuleSymbols();
					proc.Debugger.ResetJustMyCodeStatus();
				});
			return true;
		}
	}
}
