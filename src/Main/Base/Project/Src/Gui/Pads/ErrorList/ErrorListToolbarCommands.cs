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
				return ErrorList.Instance.ShowErrors;
			}
			set {
				ErrorList.Instance.ShowErrors = value;
			}
		}
	}
	
	public class ShowWarningsToggleButton : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return ErrorList.Instance.ShowWarnings;
			}
			set {
				ErrorList.Instance.ShowWarnings = value;
			}
		}
	}
	
	public class ShowMessagesToggleButton : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return ErrorList.Instance.ShowMessages;
			}
			set {
				ErrorList.Instance.ShowMessages = value;
			}
		}
	}
	
}
