// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

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
	public partial class DebuggingSymbolsPanel : AbstractOptionPanel
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
			Process proc = WindowsDebugger.CurrentProcess;
			if (proc != null) {
				proc.Debugger.ReloadModuleSymbols();
				proc.Debugger.ResetJustMyCodeStatus();
			}
			return true;
		}
	}
}
