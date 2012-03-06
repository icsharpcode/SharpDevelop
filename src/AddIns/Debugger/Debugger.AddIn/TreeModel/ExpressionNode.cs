// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Debugger.AddIn.Visualizers;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;
using TreeNode = Debugger.AddIn.TreeModel.TreeNode;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// Node in the tree which can be defined by a debugger expression.
	/// The expression will be lazily evaluated when needed.
	/// </summary>
	public class ExpressionNode: TreeNode, ISetText, INotifyPropertyChanged
	{
		bool evaluated;
		
		Expression expression;
		bool canSetText;
		GetValueException error;
		string fullText;
		
		public bool Evaluated {
			get { return evaluated; }
			set { evaluated = value; }
		}
		
		public Expression Expression {
			get { return expression; }
		}
		
		public override bool CanSetText {
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
		
		public string FullText {
			get { return fullText; }
		}
		
		public override string Text {
			get {
				if (!evaluated) EvaluateExpression();
				return base.Text;
			}
			set {
				if (value != base.Text) {
					base.Text = value;
					NotifyPropertyChanged("Text");
				}
			}
		}
		
		public override string FullName {
			get {
				return this.expression.PrettyPrint() ?? Name.Trim();
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

		public ExpressionNode(TreeNode parent, IImage image, string name, Expression expression)
			: base(parent)
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
				var process = WindowsDebugger.DebuggedProcess;
				if (process == null) return;
				StackFrame frame = process.GetCurrentExecutingFrame();
				if (frame == null) return;
				var debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
				object data = debugger.debuggerDecompilerService.GetLocalVariableIndex(frame.MethodInfo.DeclaringType.MetadataToken,
				                                                                       frame.MethodInfo.MetadataToken,
				                                                                       Name);
				
				if (expression is MemberReferenceExpression) {
					var memberExpression = (MemberReferenceExpression)expression;
					memberExpression.TargetObject.UserData = data;
				} else {
					expression.UserData = data;
				}
				// evaluate expression
				val = expression.Evaluate(process);
			} catch (GetValueException e) {
				error = e;
				this.Text = e.Message;
				return;
			}
			
			this.canSetText = val.Type.IsPrimitive;
			
			this.expressionType = val.Type;
			this.Type = val.Type.Name;
			this.valueIsNull = val.IsNull;
			
			// Note that these return enumerators so they are lazy-evaluated
			if (val.IsNull) {
			} else if (val.Type.IsPrimitive || val.Type.FullName == typeof(string).FullName) { // Must be before IsClass
			} else if (val.Type.IsArray) { // Must be before IsClass
				if (val.ArrayLength > 0)
					this.childNodes = Utils.LazyGetChildNodesOfArray(this, this.Expression, val.ArrayDimensions);
			} else if (val.Type.IsClass || val.Type.IsValueType) {
				if (val.Type.FullNameWithoutGenericArguments == typeof(List<>).FullName) {
					if ((int)val.GetMemberValue("_size").PrimitiveValue > 0)
						this.childNodes = Utils.LazyGetItemsOfIList(this, this.expression);
				} else {
					this.childNodes = Utils.LazyGetChildNodesOfObject(this, this.Expression, val.Type);
				}
			} else if (val.Type.IsPointer) {
				Value deRef = val.Dereference();
				if (deRef != null) {
					this.childNodes = new ExpressionNode [] { new ExpressionNode(this, this.IconImage, "*" + this.Name, this.Expression.AppendDereference()) };
				}
			}
			
			if (DebuggingOptions.Instance.ICorDebugVisualizerEnabled) {
				TreeNode info = ICorDebug.GetDebugInfoRoot(val.AppDomain, val.CorValue);
				this.childNodes = Utils.PrependNode(info, this.ChildNodes);
			}
			
			// Do last since it may expire the object
			if (val.Type.IsInteger) {
				fullText = FormatInteger(val.PrimitiveValue);
			} else if (val.Type.IsPointer) {
				fullText = String.Format("0x{0:X}", val.PointerAddress);
			} else if ((val.Type.FullName == typeof(string).FullName ||
			            val.Type.FullName == typeof(char).FullName) && !val.IsNull) {
				try {
					fullText = '"' + Escape(val.InvokeToString()) + '"';
				} catch (GetValueException e) {
					error = e;
					fullText = e.Message;
					return;
				}
			} else if ((val.Type.IsClass || val.Type.IsValueType) && !val.IsNull) {
				try {
					fullText = val.InvokeToString();
				} catch (GetValueException e) {
					error = e;
					fullText = e.Message;
					return;
				}
			} else {
				fullText = val.AsString();
			}
			
			this.Text = (fullText.Length > 256) ? fullText.Substring(0, 256) + "..." : fullText;
		}
		
		string Escape(string source)
		{
			return source.Replace("\n", "\\n")
				.Replace("\t", "\\t")
				.Replace("\r", "\\r")
				.Replace("\0", "\\0")
				.Replace("\b", "\\b")
				.Replace("\a", "\\a")
				.Replace("\f", "\\f")
				.Replace("\v", "\\v")
				.Replace("\"", "\\\"");
		}
		
		string FormatInteger(object i)
		{
			if (DebuggingOptions.Instance.ShowIntegersAs == ShowIntegersAs.Decimal)
				return i.ToString();
			
			string hex = null;
			for(int len = 1;; len *= 2) {
				hex = string.Format("{0:X" + len + "}", i);
				if (hex.Length == len)
					break;
			}
			
			if (DebuggingOptions.Instance.ShowIntegersAs == ShowIntegersAs.Hexadecimal) {
				return "0x" + hex;
			} else {
				if (ShowAsHex(i)) {
					return String.Format("{0} (0x{1})", i, hex);
				} else {
					return i.ToString();
				}
			}
		}
		
		bool ShowAsHex(object i)
		{
			ulong val;
			if (i is sbyte || i is short || i is int || i is long) {
				unchecked { val = (ulong)Convert.ToInt64(i); }
				if (val > (ulong)long.MaxValue)
					val = ~val + 1;
			} else {
				val = Convert.ToUInt64(i);
			}
			if (val >= 0x10000)
				return true;
			
			int ones = 0; // How many 1s there is
			int runs = 0; // How many runs of 1s there is
			int size = 0; // Size of the integer in bits
			while(val != 0) { // There is at least one 1
				while((val & 1) == 0) { // Skip 0s
					val = val >> 1;
					size++;
				}
				while((val & 1) == 1) { // Skip 1s
					val = val >> 1;
					size++;
					ones++;
				}
				runs++;
			}
			
			return size >= 7 && runs <= (size + 7) / 8;
		}
		
		public override bool SetText(string newText)
		{
			string fullName = FullName;
			
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
		
		public static IImage GetImageForThis(out string imageName)
		{
			imageName = "Icons.16x16.Parameter";
			return DebuggerResourceService.GetImage(imageName);
		}
		
		public static IImage GetImageForParameter(out string imageName)
		{
			imageName = "Icons.16x16.Parameter";
			return DebuggerResourceService.GetImage(imageName);
		}
		
		public static IImage GetImageForLocalVariable(out string imageName)
		{
			imageName = "Icons.16x16.Local";
			return DebuggerResourceService.GetImage(imageName);
		}
		
		public static IImage GetImageForArrayIndexer(out string imageName)
		{
			imageName = "Icons.16x16.Field";
			return DebuggerResourceService.GetImage(imageName);
		}
		
		public static IImage GetImageForMember(IDebugMemberInfo memberInfo, out string imageName)
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
			
			imageName = "Icons.16x16." + name;
			return DebuggerResourceService.GetImage(imageName);
		}
		
//		public ContextMenuStrip GetContextMenu()
//		{
//			if (this.Error != null) return GetErrorContextMenu();
//
//			ContextMenuStrip menu = new ContextMenuStrip();
//
//			ToolStripMenuItem copyItem;
//			copyItem = new ToolStripMenuItem();
//			copyItem.Text = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.CopyToClipboard");
//			copyItem.Checked = false;
//			copyItem.Click += delegate {
//				ClipboardWrapper.SetText(fullText);
//			};
		
//			ToolStripMenuItem hexView;
//			hexView = new ToolStripMenuItem();
//			hexView.Text = ResourceService.GetString("MainWindow.Windows.Debug.LocalVariables.ShowInHexadecimal");
//			hexView.Checked = DebuggingOptions.Instance.ShowValuesInHexadecimal;
//			hexView.Click += delegate {
//				// refresh all pads that use ValueNode for display
//				DebuggingOptions.Instance.ShowValuesInHexadecimal = !DebuggingOptions.Instance.ShowValuesInHexadecimal;
//				// always check if instance is null, might be null if pad is not opened
//				if (LocalVarPad.Instance != null)
//					LocalVarPad.Instance.RefreshPad();
//				if (WatchPad.Instance != null)
//					WatchPad.Instance.RefreshPad();
//			};
		
//			menu.Items.AddRange(new ToolStripItem[] {
//			                    	copyItem,
//			                    	//hexView
//			                    });
//
//			return menu;
//		}
		
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
		
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		
		private void NotifyPropertyChanged(string info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(info));
			}
		}
	}
}
