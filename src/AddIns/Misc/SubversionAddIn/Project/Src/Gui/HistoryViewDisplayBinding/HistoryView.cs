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
		
		#region ICSharpCode.SharpDevelop.Gui.AbstractSecondaryViewContent abstract class implementation
		public override Control Control {
			get {
				return historyViewPanel;
			}
		}
		
		public override string TabPageText {
			get {
				return "History"; // TODO: Translate
			}
		}
		#endregion
		
		public HistoryView(IViewContent viewContent)
		{
			this.historyViewPanel = new HistoryViewPanel(viewContent);
		}
	}
}
