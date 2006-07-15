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

class ContextEntry:
	[getter(InterpreterControl)]
	_ctl as InteractiveInterpreterControl
	
	[getter(Context)]
	_context as InterpreterContext
	
	[getter(ToolBarItem)]
	_item as ToolStripButton
	
	_parentPad as InterpreterPad
	
	def constructor([required] context as InterpreterContext, [required] parentPad as InterpreterPad):
		_context = context
		_parentPad = parentPad
		
		_ctl = InteractiveInterpreterControl(context)
		_ctl.Dock = DockStyle.Fill
		
		_item = ToolStripButton(StringParser.Parse(context.Title), context.Image)
		_item.ToolTipText = context.ToolTipText
		_item.Visible = context.Visible
		_item.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
		
		context.ImageChanged += AutoInvoke() do:
			_item.Image = _context.Image
		context.TitleChanged += AutoInvoke() do:
			_item.Text = StringParser.Parse(_context.Title)
		context.ToolTipTextChanged += AutoInvoke() do:
			_item.ToolTipText = StringParser.Parse(_context.ToolTipText)
		context.VisibleChanged += AutoInvoke() do:
			_item.Visible = _context.Visible
			_parentPad.UpdateToolBarVisible()
			if _parentPad.CurrentContext is self and _context.Visible == false:
				_parentPad.ActivateFirstVisibleContext()
		_item.Click += def:
			_parentPad.CurrentContext = self
	
	private static def AutoInvoke(what as callable()) as EventHandler:
		return def(sender as object, e as EventArgs):
			WorkbenchSingleton.SafeThreadAsyncCall(what)

class InterpreterPad(AbstractPadContent, IClipboardHandler):
	[getter(Contexts)]
	_contexts = []
	
	_currentContext as ContextEntry
	_toolStrip = ToolStrip(GripStyle: ToolStripGripStyle.Hidden)
	_panel = Panel()
	
	def constructor():
		_panel.Controls.Add(_toolStrip)
		for context as InterpreterContext in AddInTree.GetTreeNode("/AddIns/InterpreterAddIn/InterpreterContexts").BuildChildItemsArrayList(self):
			newContext = ContextEntry(context, self)
			_toolStrip.Items.Add(newContext.ToolBarItem)
			_contexts.Add(newContext)
		ActivateFirstVisibleContext()
		UpdateToolBarVisible()
	
	def UpdateToolBarVisible():
		count = 0
		for c as ContextEntry in self.Contexts:
			count += 1 if c.Context.Visible
		_toolStrip.Visible = (count > 1)
	
	def ActivateFirstVisibleContext():
		for c as ContextEntry in self.Contexts:
			if c.Context.Visible:
				self.CurrentContext = c
				return
	
	Control as Control:
		get:
			return _panel
	
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
			for c as ContextEntry in self.Contexts:
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
