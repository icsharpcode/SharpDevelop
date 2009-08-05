// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// Node in the tree which can be defined by a debugger expression.
	/// The expression will be lazily evaluated when needed.
	/// </summary>
	public class ExpressionNode: TreeNode, ISetText, IContextMenu
	{
		bool evaluated;
		
		Expression expression;
		bool canSetText;
		GetValueException error;
		
		string fullText;
		
		public Expression Expression {
			get { return expression; }
		}
		
		public bool CanSetText {
			get {
				if (!evaluated) EvaluateExpression();
				return canSetText;
			}
		}
		
		public GetValueException Error {
			get {
				if (!evaluated) EvaluateExpression();
				return error;
			}
		}
		
		public override string Text {
			get {
				if (!evaluated) EvaluateExpression();
				return base.Text;
			}
		}
		
		public override string Type {
			get {
				if (!evaluated) EvaluateExpression();
				return base.Type;
			}
		}
		
		public override IEnumerable<TreeNode> ChildNodes {
			get {
				if (!evaluated) EvaluateExpression();
				return base.ChildNodes;
			}
		}
		
		public override bool HasChildNodes {
			get { 
				if (!evaluated) EvaluateExpression();
				return base.HasChildNodes; 
			}
		}
		
		public ExpressionNode(Image image, string name, Expression expression)
		{
			this.Image = image;
			this.Name = name;
			this.expression = expression;
		}
		
		void EvaluateExpression()
		{
			evaluated = true;
			
			Value val;
			try {
				val = expression.Evaluate(WindowsDebugger.DebuggedProcess);
			} catch (GetValueException e) {
				error = e;
				this.Text = e.Message;
				return;
			}
			
			this.canSetText = val.Type.IsPrimitive;
			
			if (DebuggingOptions.Instance.ShowValuesInHexadecimal && val.Type.IsInteger) {
				fullText = String.Format("0x{0:X}", val.PrimitiveValue);
			} else if (val.Type.IsPointer) {
				fullText = String.Format("0x{0:X}", val.PointerAddress);
			} else {
				fullText = val.AsString;
			}
			
			this.Type = val.Type.Name;
			
			// Note that these return enumerators so they are lazy-evaluated
			if (val.IsNull) {
			} else if (val.Type.IsClass || val.Type.IsValueType) {
				this.ChildNodes = Utils.LazyGetChildNodesOfObject(this.Expression, val.Type);
			} else if (val.Type.IsArray) {
				this.ChildNodes = Utils.LazyGetChildNodesOfArray(this.Expression, val.ArrayDimensions);
			} else if (val.Type.IsPointer) {
				Value deRef = val.Dereference();
				if (deRef != null) {
					this.ChildNodes = new ExpressionNode [] { new ExpressionNode(this.Image, "*" + this.Name, this.Expression.AppendDereference()) };
				}
			}
			
			if (DebuggingOptions.Instance.ICorDebugVisualizerEnabled) {
				TreeNode info = ICorDebug.GetDebugInfoRoot(val.AppDomain, val.CorValue);
				this.ChildNodes = Utils.PrependNode(info, this.ChildNodes);
			}
			
			// Do last since it may expire the object
			if ((val.Type.IsClass || val.Type.IsValueType) && !val.IsNull) {
				fullText = val.InvokeToString();
			}
			
			this.Text = (fullText.Length > 256) ? fullText.Substring(0, 256) + "..." : fullText;
		}
		
		public bool SetText(string newText)
		{
			Value val = null;
			try {
				val = this.Expression.Evaluate(WindowsDebugger.DebuggedProcess);
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
		
		public static Image GetImageForThis()
		{
			return IconService.GetBitmap("Icons.16x16.Parameter");
		}
		
		public static Image GetImageForParameter()
		{
			return IconService.GetBitmap("Icons.16x16.Parameter");
		}
		
		public static Image GetImageForLocalVariable()
		{
			return IconService.GetBitmap("Icons.16x16.Local");
		}
		
		public static Image GetImageForArrayIndexer()
		{
			return IconService.GetBitmap("Icons.16x16.Field");
		}
		
		public static Image GetImageForMember(MemberInfo memberInfo)
		{
			string name = string.Empty;
			if (memberInfo.IsPublic) {
			} else if (memberInfo.IsInternal) {
				name += "Internal";
			} else if (memberInfo.IsProtected) {
				name += "Protected";
			} else if (memberInfo.IsPrivate) {
				name += "Private";
			}
			if (memberInfo is FieldInfo) {
				name += "Field";
			} else if (memberInfo is PropertyInfo) {
				name += "Property";
			} else if (memberInfo is MethodInfo) {
				name += "Method";
			} else {
				throw new DebuggerException("Unknown member type " + memberInfo.GetType().FullName);
			}
			return IconService.GetBitmap("Icons.16x16." + name);
		}
		
		public ContextMenuStrip GetContextMenu()
		{
			if (this.Error != null) return GetErrorContextMenu();
			
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
		
		public ContextMenuStrip GetErrorContextMenu()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			
			ToolStripMenuItem showError;
			showError = new ToolStripMenuItem();
			showError.Text = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.ShowFullError}");
			showError.Checked = false;
			showError.Click += delegate {
				MessageService.ShowError(error, null);
			};
			
			menu.Items.AddRange(new ToolStripItem[] {
			                    	showError
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
