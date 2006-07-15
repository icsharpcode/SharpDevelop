// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace Boo.InterpreterAddIn

import System

abstract class InterpreterContext:
	[property(Visible, Observable: true)]
	private _visible as bool = true
	
	[property(Image, Observable: true)]
	private _image as System.Drawing.Image
	
	[property(Title, Observable: true)]
	private _title as string = '${res:ICSharpCode.BooInterpreter}'
	
	[property(ToolTipText, Observable: true)]
	private _toolTipText as string
	
	event LinePrinted as callable(string)
	"""Callback when interpreter outputs a text line.
	You can raise the event on any thread, InterpreterAddIn takes care of invoking"""
	
	event Cleared as callable()
	"""Callback when interpreter clears the text box.
	You can raise the event on any thread, InterpreterAddIn takes care of invoking"""
	
	protected def RaiseClear():
	"""Raise the Cleared event."""
		Cleared()
	
	protected def PrintLine(line as string):
	"""Raise LinePrinted event to output a line in the interpreter"""
		LinePrinted(line)
	
	abstract def RunCommand(code as string) as void:
	"""Execute the passed code."""
		pass
	
	virtual def GetGlobals() as (string):
	"""Gets a list of globally available types/variables. Used for Ctrl+Space completion"""
		return null
	
	virtual def SuggestCodeCompletion(code as string) as (string):
	"""Gets list of available members for completion on the passed expression. Used for '.' completion"""
		return null
