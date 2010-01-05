/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter Forstmeier
 * Datum: 15.10.2007
 * Zeit: 11:26
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Reports.Addin.Commands
{
	public class Cut : AbstractClipboardCommand
	{
		protected override bool GetEnabled(IClipboardHandler editable) {
			return editable.EnableCut;
		}
		protected override void Run(IClipboardHandler editable) {
			editable.Cut();
		}
	}
	
	public class Copy : AbstractClipboardCommand
	{
		protected override bool GetEnabled(IClipboardHandler editable) {
			return editable.EnableCopy;
		}
		protected override void Run(IClipboardHandler editable) {
			editable.Copy();
		}
	}
	
	public class Delete : AbstractClipboardCommand
	{
		protected override bool GetEnabled(IClipboardHandler editable) {
			return editable.EnableDelete;
		}
		protected override void Run(IClipboardHandler editable) {
			editable.Delete();
		}
	}
	
	
	public class Paste : AbstractClipboardCommand
	{
		protected override bool GetEnabled(IClipboardHandler editable) {
			return editable.EnablePaste;
		}
		protected override void Run(IClipboardHandler editable) {
			editable.Paste();
		}
	}
}
