// <file>
//     <copyright see="prj:///doc/copyright.txt">2004 Rodrigo B. de Oliveira; 2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

"""
Interactive Forms-based Console
"""
namespace Boo.InterpreterAddIn

import System
import System.Drawing
import System.IO
import System.Windows.Forms
import ICSharpCode.TextEditor
import ICSharpCode.TextEditor.Document
import ICSharpCode.TextEditor.Actions
import ICSharpCode.TextEditor.Gui.CompletionWindow
import Boo.Lang.Compiler.TypeSystem

interface ICompletionWindowImageProvider:
	ImageList as ImageList:
		get
	NamespaceIndex as int:
		get
	ClassIndex as int:
		get
	InterfaceIndex as int:
		get
	EnumIndex as int:
		get
	StructIndex as int:
		get
	CallableIndex as int:
		get
	MethodIndex as int:
		get
	FieldIndex as int:
		get
	LiteralIndex as int:
		get
	PropertyIndex as int:
		get
	EventIndex as int:
		get

class InteractiveInterpreterControl(TextEditorControl):
	
	enum InputState:
		SingleLine = 0
		Block = 1
		
	class NullCompletionWindowImageProvider(ICompletionWindowImageProvider):
		
		public static final Instance = NullCompletionWindowImageProvider()
		
		[getter(ImageList)]
		_imageList = System.Windows.Forms.ImageList()
		
		NamespaceIndex as int:
			get:
				return 0
		ClassIndex as int:
			get:
				return 0
		InterfaceIndex as int:
			get:
				return 0
		EnumIndex as int:
			get:
				return 0
		StructIndex as int:
			get:
				return 0
		CallableIndex as int:
			get:
				return 0
		MethodIndex as int:
			get:
				return 0
		FieldIndex as int:
			get:
				return 0
		LiteralIndex as int:
			get:
				return 0
		PropertyIndex as int:
			get:
				return 0
		EventIndex as int:
			get:
				return 0
			
	class LineHistory:
	
		_lines = []
		_current = 0
		
		event CurrentLineChanged as EventHandler
		
		def Add([required] line as string):
			if len(line) > 0 and line != LastLine:
				_lines.Add(line)
			_current = len(_lines)
			
		LastLine as string:
			get:
				return null if 0 == len(_lines)
				return _lines[-1]
		
		CurrentLine as string:
			get:
				return null if 0 == len(_lines)
				return _lines[_current]
	
		def Up():		
			MoveTo(_current - 1)
			
		def Down():
			MoveTo(_current + 1)
			
		def MoveTo(index as int):
			return if 0 == len(_lines)
			old = _current
			_current = index % len(_lines)
			if old != _current:
				CurrentLineChanged(self, EventArgs.Empty)
		
	_state = InputState.SingleLine
	
	_block = System.IO.StringWriter()
	
	[getter(Interpreter)]
	_interpreter as Boo.Lang.Interpreter.InteractiveInterpreter

	_codeCompletionWindow as CodeCompletionWindow
	
	[property(CompletionWindowImageProvider, value is not null)]
	_imageProvider as ICompletionWindowImageProvider = NullCompletionWindowImageProvider()
	
	_lineHistory as LineHistory
	
	// HACK: when the completion window is shown through CTRL+SPACE
	// it behaves very strangely, the best we can do right now is
	// to prevent the user from typing
	_blockKeys = false
	
	def constructor():
		self._interpreter = Boo.Lang.Interpreter.InteractiveInterpreter(
								RememberLastValue: true,
								Print: self.print)
		self._interpreter.SetValue("cls", cls)
		self._lineHistory = LineHistory(CurrentLineChanged: _lineHistory_CurrentLineChanged)
		self.Document.HighlightingStrategy = GetBooHighlighting()
		self.EnableFolding =  false
		self.ShowLineNumbers =  false
		self.ShowSpaces = false
		self.ShowTabs =  true
		self.ShowEOLMarkers = false
		self.AllowCaretBeyondEOL = false
		self.ShowInvalidLines = false
		self.Dock = DockStyle.Fill
		
	CaretColumn:
		get:
			return self.ActiveTextAreaControl.Caret.Column
			
	CurrentLineText:
		get:
			segment = GetLastLineSegment()
			return self.Document.GetText(segment)[4:]	
		
	override def OnLoad(args as EventArgs):
		super(args)
		prompt()
		
	def Eval(code as string):
		try:
			_interpreter.LoopEval(code)			
		ensure:
			_state = InputState.SingleLine
		
	private def ConsumeCurrentLine():		
		text as string = CurrentLineText # was accessing Control.text member
		_lineHistory.Add(text)
		print("")
		return text
		
	private def GetLastLineSegment():
		return self.Document.GetLineSegment(self.Document.LineSegmentCollection.Count)
		
	private def SingleLineInputState():
		code = ConsumeCurrentLine()
		if code[-1:] in ":", "\\":
			_state = InputState.Block
			_block.GetStringBuilder().Length = 0
			_block.WriteLine(code)
		else:
			Eval(code)
			
	private def BlockInputState():
		code = ConsumeCurrentLine()
		if 0 == len(code):
			Eval(_block.ToString())			
		else:
			_block.WriteLine(code)

	def print(msg):
		AppendText("${msg}\r\n")		
				
	def prompt():
		AppendText((">>> ", "... ")[_state])

	def ClearLine():
		segment = GetLastLineSegment()
		self.Document.Replace(segment.Offset + 4,
			self.CurrentLineText.Length,
			"")

	def AppendText(text as string):
		segment = GetLastLineSegment()
		self.Document.Insert(segment.Offset + segment.TotalLength, text)
		MoveCaretToEnd()
		
	def MoveCaretToEnd():
		segment = GetLastLineSegment()
		newOffset = segment.Offset + segment.TotalLength
		MoveCaretToOffset(newOffset)
		
	def MoveCaretToOffset(offset as int):
		self.ActiveTextAreaControl.Caret.Position = self.Document.OffsetToPosition(offset)

	override def InitializeTextAreaControl(newControl as TextAreaControl):
		super(newControl)
		newControl.TextArea.DoProcessDialogKey += HandleDialogKey
		newControl.TextArea.KeyEventHandler += HandleKeyPress
		
	InCodeCompletion:
		get:
			return _codeCompletionWindow is not null and not _codeCompletionWindow.IsDisposed

	private def DotComplete(ch as System.Char):
		ShowCompletionWindow(
			CodeCompletionDataProvider(_imageProvider, GetSuggestions()),
			ch)
			
	private def ShowCompletionWindow(completionDataProvider, ch as System.Char):
		_codeCompletionWindow = CodeCompletionWindow.ShowCompletionWindow(
					self.ParentForm, 
					self, 
					"<code>",
					completionDataProvider,
					ch)
		if _codeCompletionWindow is not null:
			_codeCompletionWindow.Closed += def():
				_blockKeys = false
					
	private def CtrlSpaceComplete():
		_blockKeys = true
		ShowCompletionWindow(
			GlobalsCompletionDataProvider(_imageProvider, self._interpreter),
			Char.MinValue)
					
	private def GetSuggestions():		
		code = CurrentLineText.Insert(self.CaretColumn-4, ".__codecomplete__")
		code = code.Insert(0, _block.ToString()) if InputState.Block == _state
		return _interpreter.SuggestCodeCompletion(code)
		
	private def HandleDialogKey(key as Keys):
		return false if InCodeCompletion
		
		if key == Keys.Enter:
			try:
				(SingleLineInputState, BlockInputState)[_state]()
			except x as System.Reflection.TargetInvocationException:
				print(x.InnerException)
			except x:
				print(x)
			prompt()
			return true
			
		if key == Keys.Up:
			_lineHistory.Up()
			return true
		if key == Keys.Down:
			_lineHistory.Down()
			return true
			
		if key == (Keys.Control | Keys.Space):
			CtrlSpaceComplete()
			return true
			
		if key in Keys.Home, Keys.Shift|Keys.Home, Keys.Control|Keys.Home:			
			MoveCaretToOffset(GetLastLineSegment().Offset + 4)
			return true
			
		if key == Keys.Escape:
			ClearLine()
			return true
			
		if key in Keys.Back, Keys.Left:
			if self.CaretColumn < 5:
				return true
		else:
			if self.CaretColumn < 4:
				MoveCaretToEnd()
				
		return false
		
	private def HandleKeyPress(ch as System.Char) as bool:
		return true if _blockKeys
		
		if InCodeCompletion:
			_codeCompletionWindow.ProcessKeyEvent(ch)
		
		if ch == "."[0]:
			DotComplete(ch)
			
		return false
		
	private def cls():
		self.Document.TextContent = ""
		self.ActiveTextAreaControl.Refresh()
	
	private def _lineHistory_CurrentLineChanged():
		segment = GetLastLineSegment()
		self.Document.Replace(segment.Offset + 4,
			self.CurrentLineText.Length,
			_lineHistory.CurrentLine)
		
	def GetBooHighlighting():
		return HighlightingManager.Manager.FindHighlighter("Boo")
		
	static def InstallDefaultSyntaxModeProvider():
		HighlightingManager.Manager.AddSyntaxModeFileProvider(
			FileSyntaxModeProvider(Path.GetDirectoryName(Application.ExecutablePath)))

