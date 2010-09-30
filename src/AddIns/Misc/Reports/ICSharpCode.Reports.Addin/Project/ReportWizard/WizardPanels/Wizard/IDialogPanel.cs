// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	public enum DialogMessage {
		OK,
		Cancel,
		Help,
		Next,
		Prev,
		Finish,
		Activated
	}
	
	public interface IDialogPanel
	{
		/// <summary>
		/// Some panels do get an object which they can customize, like
		/// Wizard Dialogs. Check the dialog description for more details
		/// about this.
		/// </summary>
		object CustomizationObject {
			get;
			set;
		}
		
		Control Control {
			get;
		}
		
		bool EnableFinish {
			get;
		}
		
		/// <returns>
		/// true, if the DialogMessage could be executed.
		/// </returns>
		bool ReceiveDialogMessage(DialogMessage message);
		
		event EventHandler EnableFinishChanged;
	}
}
