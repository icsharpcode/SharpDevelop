// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

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
