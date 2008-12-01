// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Windows.Forms;
using Debugger.Expressions;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Sda;

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
					box.ShowDialog(WorkbenchSingleton.MainForm);
				}
			};
			
			menu.Items.AddRange(new ToolStripItem[] {
			                    	showError
			                    });
			
			return menu;
		}
	}
}
