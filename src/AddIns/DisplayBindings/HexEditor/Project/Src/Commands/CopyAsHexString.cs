// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.Core.WinForms;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace HexEditor.Commands
{
	/// <summary>
	/// Description of CopyAsHexString
	/// </summary>
	public class CopyAsHexString : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			Editor editor = Owner as Editor;
			
			if (editor != null) {
				ClipboardWrapper.SetText(editor.CopyAsHexString());
			}
		}
	}
}
