// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn
{
	public class HistoryView : AbstractSecondaryViewContent
	{
		HistoryViewPanel historyViewPanel;
		
		public override Control Control {
			get {
				return historyViewPanel;
			}
		}
		
		public HistoryView(IViewContent viewContent) : base(viewContent)
		{
			this.TabPageText = "${res:AddIns.Subversion.History}";
			this.historyViewPanel = new HistoryViewPanel(viewContent.PrimaryFileName);
		}
		
		protected override void LoadFromPrimary()
		{
		}
		protected override void SaveToPrimary()
		{
		}
		
		public override bool IsViewOnly {
			get { return true; }
		}
	}
}
