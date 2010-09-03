// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Svn
{
	public class HistoryView : AbstractSecondaryViewContent
	{
		HistoryViewPanel historyViewPanel;
		
		public override object Control {
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
