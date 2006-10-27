// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop
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
