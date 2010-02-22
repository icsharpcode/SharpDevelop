// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core.WinForms;
using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace HexEditor.Commands
{
	/// <summary>
	/// Description of CopyAsBinary
	/// </summary>
	public class CopyAsBinary : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			Editor editor = Owner as Editor;
			
			if (editor != null) {
				ClipboardWrapper.SetText(editor.CopyAsBinary());
			}
		}
	}
}
