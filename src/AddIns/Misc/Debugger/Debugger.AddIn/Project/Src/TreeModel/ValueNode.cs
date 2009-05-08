// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Debugger.Expressions;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// Represents the data in a row in a TreeViewNode.
	/// </summary>
	public class ValueNode: AbstractNode, ISetText, IContextMenu
	{
		Expression expression;
		bool canSetText;
		string fullText;
		
		public Expression Expression {
			get { return expression; }
		}
		
		/// <remarks>HACK for WatchPad</remarks>
		public void SetName(string name)
		{
			this.Name = name;
		}
		
		/// <summary>
		/// Factory method to create an instance.
		/// </summary>
		/// <param name="expression">The expression containing the value you wish to display.</param>
		/// <returns>
		/// Returns a ValueNode if it can successfully evaluate expression or
		/// ErrorNode if it fails to do so.
		/// </returns>
		/// <see cref=ErrorNode""/>
		public static AbstractNode Create(Expression expression)
		{
			try {
				Value val = expression.Evaluate(WindowsDebugger.DebuggedProcess);
				return new ValueNode(val);
			} catch (GetValueException e) {
				return new ErrorNode(expression, e);
			}
		}
		
		/// <summary>
		/// Constructor used by the factory method Create()
		/// </summary>
		/// <param name="val"></param>
		/// <exception cref="System.Management.Automation.GetValueException">
		/// Can be thrown by InvokeToString()
		/// </exception>
		public ValueNode(Value val)
		{
			this.expression = val.Expression;
			
			canSetText = false;
			if (val.Type.IsInteger) {
				canSetText =
					(val.Expression is LocalVariableIdentifierExpression) ||
					(val.Expression is ParameterIdentifierExpression) ||
					(val.Expression is ArrayIndexerExpression) ||
					(val.Expression is MemberReferenceExpression && ((MemberReferenceExpression)val.Expression).MemberInfo is FieldInfo);
			}
			
			this.Image = IconService.GetBitmap("Icons.16x16." + GetImageName(val));
			
			this.Name = val.Expression.CodeTail;
			
			if (DebuggingOptions.Instance.ShowValuesInHexadecimal && val.Type.IsInteger) {
				fullText = String.Format("0x{0:X}", val.PrimitiveValue);
			} else if (val.Type.IsPointer) {
				fullText = String.Format("0x{0:X}", val.PointerAddress);
			} else {
				fullText = val.AsString;
			}
			
			if (val.Type != null) {
				this.Type = val.Type.Name;
			} else {
				this.Type = String.Empty;
			}
			
			// Note that these return enumerators so they are lazy-evaluated
			this.ChildNodes = null;
			if (val.IsNull) {
			} else if (val.Type.IsClass || val.Type.IsValueType) {
				this.ChildNodes = Utils.GetChildNodesOfObject(this.Expression, val.Type);
			} else if (val.Type.IsArray) {
				this.ChildNodes = Utils.GetChildNodesOfArray(this.Expression, val.ArrayDimensions);
			} else if (val.Type.IsPointer) {
				Value deRef = val.Dereference();
				if (deRef != null) {
					this.ChildNodes = new AbstractNode [] { new ValueNode(deRef) };
				}
			}
			
			if (DebuggingOptions.Instance.ICorDebugVisualizerEnabled) {
				AbstractNode info = ICorDebug.GetDebugInfoRoot(val.Process, val.CorValue);
				this.ChildNodes = PrependNode(info, this.ChildNodes);
			}
			
			// Do last since it may expire the object
			if ((val.Type.IsClass || val.Type.IsValueType) && !val.IsNull) {
				fullText = val.InvokeToString();
			}
			
			this.Text = (fullText.Length > 256) ? fullText.Substring(0, 256) + "..." : fullText;
		}
		
		IEnumerable<AbstractNode> PrependNode(AbstractNode node, IEnumerable<AbstractNode> rest)
		{
			yield return node;
			if (rest != null) {
				foreach(AbstractNode absNode in rest) {
					yield return absNode;
				}
			}
		}
		
		public bool CanSetText {
			get {
				return canSetText;
			}
		}
		
		public bool SetText(string newText)
		{
			Value val = null;
			try {
				val = this.Expression.Evaluate(WindowsDebugger.DebuggedProcess.SelectedStackFrame);
				if (val.Type.IsInteger && newText.StartsWith("0x")) {
					try {
						val.PrimitiveValue = long.Parse(newText.Substring(2), NumberStyles.HexNumber);
					} catch (FormatException) {
						throw new NotSupportedException();
					} catch (OverflowException) {
						throw new NotSupportedException();
					}
				} else {
					val.PrimitiveValue = newText;
				}
				this.Text = newText;
				return true;
			} catch (NotSupportedException) {
				string format = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.CannotSetValue.BadFormat");
				string msg = String.Format(format, newText, val.Type.PrimitiveType.ToString());
				MessageService.ShowMessage(msg ,"${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.Title}");
			} catch (COMException) {
				// COMException (0x80131330): Cannot perfrom SetValue on non-leaf frames.
				// Happens if trying to set value after exception is breaked
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.UnknownError}",
				                           "${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.Title}");
			}
			return false;
		}
		
		string GetImageName(Value val)
		{
			Expression expr = val.Expression;
			if (expr is ThisReferenceExpression) {
				if (val.Type.IsClass) {
					return "Class";
				}
				if (val.Type.IsValueType) {
					return "Struct";
				}
			}
			if (expr is ParameterIdentifierExpression) {
				return "Parameter";
			}
			if (expr is MemberReferenceExpression) {
				MemberInfo memberInfo = ((MemberReferenceExpression)expr).MemberInfo;
				string prefix;
				if (memberInfo.IsPublic) {
					prefix = "";
				} else if (memberInfo.IsInternal) {
					prefix = "Internal";
				} else if (memberInfo.IsProtected) {
					prefix = "Protected";
				} else if (memberInfo.IsPrivate) {
					prefix = "Private";
				} else {
					prefix = "";
				}
				if (memberInfo is FieldInfo) {
					return prefix + "Field";
				}
				if (memberInfo is PropertyInfo) {
					return prefix + "Property";
				}
				if (memberInfo is MethodInfo) {
					return prefix + "Method";
				}
			}
			if (expr is LocalVariableIdentifierExpression) {
				return "Local";
			}
			if (expr is ArrayIndexerExpression) {
				return "Field";
			}
			return "Field";
		}
		
		public ContextMenuStrip GetContextMenu()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			
			ToolStripMenuItem copyItem;
			copyItem = new ToolStripMenuItem();
			copyItem.Text = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.CopyToClipboard");
			copyItem.Checked = false;
			copyItem.Click += delegate {
				ClipboardWrapper.SetText(fullText);
			};
			
			ToolStripMenuItem hexView;
			hexView = new ToolStripMenuItem();
			hexView.Text = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.ShowInHexadecimal");
			hexView.Checked = DebuggingOptions.Instance.ShowValuesInHexadecimal;
			hexView.Click += delegate {
				// refresh all pads that use ValueNode for display
				DebuggingOptions.Instance.ShowValuesInHexadecimal = !DebuggingOptions.Instance.ShowValuesInHexadecimal;
				// always check if instance is null, might be null if pad is not opened
				if (LocalVarPad.Instance != null)
					LocalVarPad.Instance.RefreshPad();
				if (WatchPad.Instance != null)
					WatchPad.Instance.RefreshPad();
			};
			
			menu.Items.AddRange(new ToolStripItem[] {
			                    	copyItem,
			                    	hexView
			                    });
			
			return menu;
		}
		
		public static WindowsDebugger WindowsDebugger {
			get {
				return (WindowsDebugger)DebuggerService.CurrentDebugger;
			}
		}
	}
}
