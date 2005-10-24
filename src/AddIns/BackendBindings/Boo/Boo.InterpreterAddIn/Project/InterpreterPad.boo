// <file>
//     <copyright see="prj:///doc/copyright.txt">2004 Rodrigo B. de Oliveira; 2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace Boo.InterpreterAddIn

import System
import System.Windows.Forms
import ICSharpCode.Core
import ICSharpCode.SharpDevelop.Gui

class InterpreterPad(AbstractPadContent, IClipboardHandler):
"""Description of InterpreterPad"""
	ctl = InteractiveInterpreterControl()
	
	Control as Control:
		get:
			return ctl
	
	EnableCut:
		get:
			return ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut
	
	EnableCopy:
		get:
			return ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy
	
	EnablePaste:
		get:
			return ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste
	
	EnableDelete:
		get:
			return ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete
	
	EnableSelectAll:
		get:
			return ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll
	
	def Cut():
		ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.Cut(null, null)
	
	def Copy():
		ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.Copy(null, null)
	
	def Paste():
		ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.Paste(null, null)
	
	def Delete():
		ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.Delete(null, null)
	
	def SelectAll():
		ctl.ActiveTextAreaControl.TextArea.ClipboardHandler.SelectAll(null, null)
