// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace Boo.InterpreterAddIn

import System
import System.Reflection

class DefaultBooInterpreterContext(InterpreterContext):
	_interpreter as Boo.Lang.Interpreter.InteractiveInterpreter
	_isAdditionalTab as bool
	
	def constructor():
		self.Image = ICSharpCode.Core.ResourceService.GetBitmap("Boo.ProjectIcon")
	
	def constructor(isAdditionalTab as bool):
		self()
		if isAdditionalTab:
			self.Title = "New " + self.Title
			_isAdditionalTab = true
	
	private def AddAssemblies(main as Assembly):
		_interpreter.References.Add(main)
		for reference in main.GetReferencedAssemblies():
			_interpreter.References.Add(Assembly.Load(reference.FullName))
	
	private def InitInterpreter():
		_interpreter = Boo.Lang.Interpreter.InteractiveInterpreter(
								RememberLastValue: true,
								Print: self.PrintLine)
		AddAssemblies(typeof(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton).Assembly)
		_interpreter.SetValue("cls", RaiseClear)
		_interpreter.SetValue("newTab") do():
			InterpreterPad.Instance.AddInterpreterContext(c = DefaultBooInterpreterContext(true))
			return c
		if _isAdditionalTab:
			_interpreter.SetValue("thisTab", self)
		
		_interpreter.LoopEval("""
import System
import System.IO
import System.Text
""")
	
	def RunCommand(code as string):
		InitInterpreter() if _interpreter is null
		try:
			_interpreter.LoopEval(code)
		except x as System.Reflection.TargetInvocationException:
			PrintLine(x.InnerException.ToString())
	
	def GetGlobals():
		InitInterpreter() if _interpreter is null
		return _interpreter.globals()
	
	def CloseTab():
		InterpreterPad.Instance.RemoveInterpreterContext(self)
