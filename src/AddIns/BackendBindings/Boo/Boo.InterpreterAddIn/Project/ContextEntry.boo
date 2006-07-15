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
