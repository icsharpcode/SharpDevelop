// <file>
//     <copyright see="prj:///doc/copyright.txt">2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace Boo.InterpreterAddIn

import System
import System.Windows.Forms

class InterpreterWrapper:
	_interpreter as Boo.Lang.Interpreter.InteractiveInterpreter
	
	def constructor():
		_interpreter = Boo.Lang.Interpreter.InteractiveInterpreter(
								RememberLastValue: true,
								Print: self.OnPrintLine)
		_interpreter.SetValue("cls", { Cleared() })
	
	event LinePrinted as callable(string)
	event Cleared as MethodInvoker
	
	private def OnPrintLine(text as string):
		LinePrinted(text)
	
	def RunCommand(code as string):
		_interpreter.LoopEval(code)
	
	def SuggestCodeCompletion(code as string):
		// David: the code completion items have to be passed as strings;
		// but it's not important, you can return null if you want.
		return _interpreter.SuggestCodeCompletion(code)

