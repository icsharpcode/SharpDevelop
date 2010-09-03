// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
