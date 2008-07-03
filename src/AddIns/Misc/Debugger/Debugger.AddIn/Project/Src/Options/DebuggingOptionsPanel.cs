// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Panel for general debugging options
	/// </summary>
	public partial class DebuggingOptionsPanel : AbstractOptionPanel
	{
		public DebuggingOptionsPanel()
		{
			InitializeComponent();
			foreach (Control ctl in Controls.GetRecursive()) {
				ctl.Text = StringParser.Parse(ctl.Text);
			}
		}
		
		public override void LoadPanelContents()
		{
			jmc.Checked = DebuggingOptions.JustMyCodeEnabled;
			obeyDebuggerAttributes.Checked = DebuggingOptions.ObeyDebuggerAttributes;
		}
		
		public override bool StorePanelContents()
		{
			DebuggingOptions.JustMyCodeEnabled = jmc.Checked;
			DebuggingOptions.ObeyDebuggerAttributes = obeyDebuggerAttributes.Checked;
			
			DebuggingOptions.ApplyToCurrentDebugger();
			
			return true;
		}
	}
}
