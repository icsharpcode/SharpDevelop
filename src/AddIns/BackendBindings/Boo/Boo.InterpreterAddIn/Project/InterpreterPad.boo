// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace Boo.InterpreterAddIn

import System
import System.Windows.Forms
import ICSharpCode.Core
import ICSharpCode.SharpDevelop.Gui

class InterpreterPad(AbstractPadContent, IClipboardHandler):
	public static Instance as InterpreterPad:
		get:
			return WorkbenchSingleton.Workbench.GetPad(InterpreterPad).PadContent
	
	_contexts = []
	_currentContext as ContextEntry
	_toolStrip = ToolStrip(GripStyle: ToolStripGripStyle.Hidden)
	_panel = Panel()
	_initDone
	
	def constructor():
		_panel.Controls.Add(_toolStrip)
		for context as InterpreterContext in AddInTree.GetTreeNode("/AddIns/InterpreterAddIn/InterpreterContexts").BuildChildItemsArrayList(self):
			AddInterpreterContext(context)
		ActivateFirstVisibleContext()
		UpdateToolBarVisible()
		_initDone = true
	
	def AddInterpreterContext([required] context as InterpreterContext):
		newContext = ContextEntry(context, self)
		_toolStrip.Items.Add(newContext.ToolBarItem)
		_contexts.Add(newContext)
		UpdateToolBarVisible() if _initDone
		return newContext
	
	def RemoveInterpreterContext([required] context as InterpreterContext):
		for c as ContextEntry in _contexts:
			if c.Context is context:
				RemoveInterpreterContext(c)
				return
	
	def RemoveInterpreterContext([required] context as ContextEntry):
		_contexts.Remove(context)
		_toolStrip.Items.Remove(context.ToolBarItem)
		ActivateFirstVisibleContext() if self.CurrentContext is context
		UpdateToolBarVisible()
	
	def UpdateToolBarVisible():
		count = 0
		for c as ContextEntry in _contexts:
			count += 1 if c.Context.Visible
		_toolStrip.Visible = (count > 1)
	
	def ActivateFirstVisibleContext():
		for c as ContextEntry in _contexts:
			if c.Context.Visible:
				self.CurrentContext = c
				return
		self.CurrentContext = null
	
	Control as Control:
		get:
			return _panel
	
	public Contexts:
		get:
			return System.Collections.ArrayList.ReadOnly(_contexts)
	
	CurrentContext:
		get:
			return _currentContext
		set:
			return if _currentContext is value
			if _currentContext is not null:
				_panel.Controls.Remove(_currentContext.InterpreterControl)
			if value is not null:
				_panel.Controls.Add(value.InterpreterControl)
				_panel.Controls.SetChildIndex(_toolStrip, 1)
			_currentContext = value
			for c as ContextEntry in _contexts:
				c.ToolBarItem.Checked = c is value
	
	CurrentTextArea:
		get:
			return _currentContext.InterpreterControl.ActiveTextAreaControl.TextArea
	
	EnableCut:
		get:
			return CurrentTextArea.ClipboardHandler.EnableCut
	
	EnableCopy:
		get:
			return CurrentTextArea.ClipboardHandler.EnableCopy
	
	EnablePaste:
		get:
			return CurrentTextArea.ClipboardHandler.EnablePaste
	
	EnableDelete:
		get:
			return CurrentTextArea.ClipboardHandler.EnableDelete
	
	EnableSelectAll:
		get:
			return CurrentTextArea.ClipboardHandler.EnableSelectAll
	
	def Cut():
		CurrentTextArea.ClipboardHandler.Cut(null, null)
	
	def Copy():
		CurrentTextArea.ClipboardHandler.Copy(null, null)
	
	def Paste():
		CurrentTextArea.ClipboardHandler.Paste(null, null)
	
	def Delete():
		CurrentTextArea.ClipboardHandler.Delete(null, null)
	
	def SelectAll():
		CurrentTextArea.ClipboardHandler.SelectAll(null, null)
