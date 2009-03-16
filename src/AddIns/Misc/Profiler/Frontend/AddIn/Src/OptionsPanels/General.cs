// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.Windows.Forms.Integration;

namespace ICSharpCode.Profiler.AddIn.OptionsPanels
{
	/// <summary>
	/// Description of General
	/// </summary>
	public class General : AbstractOptionPanel
	{
		GeneralOptionsPanel panel;
		
		public General()
		{
			ElementHost host = new ElementHost();
			host.Dock = DockStyle.Fill;
			host.Child = panel = new GeneralOptionsPanel();
			this.Controls.Add(host);
		}
		
		public override void LoadPanelContents()
		{
			base.LoadPanelContents();
		}
		
		public override bool StorePanelContents()
		{
			return base.StorePanelContents();
		}
	}
}
