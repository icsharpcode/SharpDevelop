// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Sda;

using Debugger;
using Debugger.Expressions;

namespace Debugger.AddIn.TreeModel
{
	public class ErrorNode: AbstractNode, IContextMenu
	{
		Expression targetObject;
		GetValueException error;
		
		public Expression TargetObject {
			get { return targetObject; }
		}
		
		public GetValueException Error {
			get { return error; }
		}
		
		public ErrorNode(Expression targetObject, GetValueException error)
		{
			this.targetObject = targetObject;
			this.error = error;
			
			this.Name = targetObject.CodeTail;
			this.Text = error.Error;
		}
		
		public ContextMenuStrip GetContextMenu()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			
			ToolStripMenuItem showError;
			showError = new ToolStripMenuItem();
			showError.Text = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.ShowFullError}");
			showError.Checked = false;
			showError.Click += delegate {
				using (ExceptionBox box = new ExceptionBox(error, null, false)) {
					box.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				}
			};
			
			menu.Items.AddRange(new ToolStripItem[] {
			                    	showError
			                    });
			
			return menu;
		}
	}
}
