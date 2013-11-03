// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;

namespace MSHelpSystem.Controls
{
	public class HelpLibraryManagerNotFoundForm : ToolNotFoundDialog
	{
		public HelpLibraryManagerNotFoundForm()
			: base(StringParser.Parse("${res:AddIns.HelpViewer.HLMNotAvailableDownloadWinSDK}"),
			       "http://www.microsoft.com/en-us/download/details.aspx?id=8279", null)
		{
		}
	}
}
