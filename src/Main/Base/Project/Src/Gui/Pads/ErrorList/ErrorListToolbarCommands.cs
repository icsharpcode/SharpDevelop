// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ShowErrorsToggleButton : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return ErrorListPad.Instance.ShowErrors;
			}
			set {
				ErrorListPad.Instance.ShowErrors = value;
			}
		}
	}
	
	public class ShowWarningsToggleButton : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return ErrorListPad.Instance.ShowWarnings;
			}
			set {
				ErrorListPad.Instance.ShowWarnings = value;
			}
		}
	}
	
	public class ShowMessagesToggleButton : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return ErrorListPad.Instance.ShowMessages;
			}
			set {
				ErrorListPad.Instance.ShowMessages = value;
			}
		}
	}
}
