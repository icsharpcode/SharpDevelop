// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

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
			pathList.LoadList(DebuggingOptions.SymbolsSearchPaths);
		}
		
		public override bool StorePanelContents()
		{
			DebuggingOptions.SymbolsSearchPaths = pathList.GetList();
			
			DebuggingOptions.ApplyToCurrentDebugger();
			
			return true;
		}
	}
}
