// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.Core.Services;

namespace ICSharpCode.Core.WinForms
{
	/// <summary>
	/// Message service that sets an owner for dialog boxes.
	/// </summary>
	public interface IDialogMessageService : IMessageService
	{
		IWin32Window DialogOwner { set; get; }
		ISynchronizeInvoke DialogSynchronizeInvoke { set; get; }
	}
}
