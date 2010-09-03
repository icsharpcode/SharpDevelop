// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
