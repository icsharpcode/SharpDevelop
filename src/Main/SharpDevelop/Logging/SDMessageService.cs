// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Logging
{
	sealed class SDMessageService : WinFormsMessageService
	{
		public override void ShowException(Exception ex, string message)
		{
			SD.Log.Error(message, ex);
			SD.Log.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
			if (ex != null)
				ExceptionBox.ShowErrorBox(ex, message);
			else
				ShowError(message);
		}
	}
}
