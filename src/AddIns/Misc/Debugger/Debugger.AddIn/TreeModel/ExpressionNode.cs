// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Debugger.AddIn.Visualizers;
using Debugger.AddIn.Visualizers.Utils;
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
		
		/// <summary> Used to determine available VisualizerCommands </summary>
		private DebugType expressionType;
		/// <summary> Used to determine available VisualizerCommands </summary>
		private bool valueIsNull = true;
		
		private IEnumerable<IVisualizerCommand> visualizerCommands;
		public override IEnumerable<IVisualizerCommand> VisualizerCommands {
			get {
				if (visualizerCommands == null) {
					visualizerCommands = getAvailableVisualizerCommands();
				}
				return visualizerCommands;
			}
		}
		
		private IEnumerable<IVisualizerCommand> getAvailableVisualizerCommands()
		{
			if (!evaluated) EvaluateExpression();
			
			if (this.expressionType == null) {
				// no visualizers if EvaluateExpression failed
				yield break;
			}
			if (this.valueIsNull) {
				// no visualizers if evaluated value is null
				yield break;
			}
			/*if (this.expressionType.IsPrimitive || this.expressionType.IsSystemDotObject() || this.expressionType.IsEnum()) {
				// no visualizers for primitive types
				yield break;
			}*/
			
			foreach (var descriptor in VisualizerDescriptors.GetAllDescriptors()) {
				if (descriptor.IsVisualizerAvailable(this.expressionType)) {
					yield return descriptor.CreateVisualizerCommand(this.Expression);
				}
			}
		}

		public ExpressionNode(IImage image, string name, Expression expression)
		{
			this.IconImage = image;
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
			
			this.expressionType = val.Type;
			this.Type = val.Type.Name;
			this.valueIsNull = val.IsNull;
			
			// Note that these return enumerators so they are lazy-evaluated
			if (val.IsNull) {
			} else if (val.Type.IsClass || val.Type.IsValueType) {
				if (val.Type.FullNameWithoutGenericArguments == typeof(List<>).FullName) {
					this.ChildNodes = Utils.LazyGetItemsOfIList(this.expression);
				} else {
					this.ChildNodes = Utils.LazyGetChildNodesOfObject(this.Expression, val.Type);
				}
			} else if (val.Type.IsArray) {
				this.ChildNodes = Utils.LazyGetChildNodesOfArray(this.Expression, val.ArrayDimensions);
			} else if (val.Type.IsPointer) {
				Value deRef = val.Dereference();
				if (deRef != null) {
					this.ChildNodes = new ExpressionNode [] { new ExpressionNode(this.IconImage, "*" + this.Name, this.Expression.AppendDereference()) };
				}
			}
			
			if (DebuggingOptions.Instance.ICorDebugVisualizerEnabled) {
				TreeNode info = ICorDebug.GetDebugInfoRoot(val.AppDomain, val.CorValue);
				this.ChildNodes = Utils.PrependNode(info, this.ChildNodes);
			}
			
			// Do last since it may expire the object
			if ((val.Type.IsClass || val.Type.IsValueType) && !val.IsNull) {
				try {
					fullText = val.InvokeToString();
				} catch (GetValueException e) {
					error = e;
					this.Text = e.Message;
					return;
				}
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
				string msg = string.Format(format, newText, val.Type.PrimitiveType);
				MessageService.ShowMessage(msg ,"${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.Title}");
			} catch (COMException) {
				// COMException (0x80131330): Cannot perfrom SetValue on non-leaf frames.
				// Happens if trying to set value after exception is breaked
				MessageService.ShowMessage("${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.UnknownError}",
				                           "${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.Title}");
			}
			return false;
		}
		
		public static IImage GetImageForThis()
		{
			return DebuggerResourceService.GetImage("Icons.16x16.Parameter");
		}
		
		public static IImage GetImageForParameter()
		{
			return DebuggerResourceService.GetImage("Icons.16x16.Parameter");
		}
		
		public static IImage GetImageForLocalVariable()
		{
			return DebuggerResourceService.GetImage("Icons.16x16.Local");
		}
		
		public static IImage GetImageForArrayIndexer()
		{
			return DebuggerResourceService.GetImage("Icons.16x16.Field");
		}
		
		public static IImage GetImageForMember(IDebugMemberInfo memberInfo)
		{
			string name = string.Empty;
			if (memberInfo.IsPublic) {
			} else if (memberInfo.IsAssembly) {
				name += "Internal";
			} else if (memberInfo.IsFamily) {
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
			return DebuggerResourceService.GetImage("Icons.16x16." + name);
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
				MessageService.ShowException(error, null);
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
