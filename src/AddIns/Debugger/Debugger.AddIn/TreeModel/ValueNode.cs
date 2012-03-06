// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using Debugger.AddIn.Visualizers;
using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// Node in the tree which can be defined by a debugger expression.
	/// The expression will be lazily evaluated when needed.
	/// </summary>
	public class ExpressionNode: TreeNode, INotifyPropertyChanged
	{
		bool evaluated;
		
		Func<Value> valueGetter;
		Value permanentValue;
		bool canSetText;
		GetValueException error;
		string fullText;
		
		public bool Evaluated {
			get { return evaluated; }
			set { evaluated = value; }
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
		
		public override string Type {
			get {
				if (!evaluated) EvaluateExpression();
				return base.Type;
			}
		}
		
		public override Func<IEnumerable<TreeNode>> GetChildren {
			get {
				if (!evaluated) EvaluateExpression();
				return base.GetChildren;
			}
			protected set {
				base.GetChildren = value;
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
					yield return descriptor.CreateVisualizerCommand(this.Name, () => this.Evaluate());
				}
			}
		}

		public ExpressionNode(string imageName, string name, Func<Value> valueGetter)
			: base(imageName, name, string.Empty, string.Empty, null)
		{
			this.valueGetter = valueGetter;
		}
		
		/// <summary>
		/// Get the value of the node and cache it as long-lived reference.
		/// We assume that the user will need this value a lot.
		/// </summary>
		public Value Evaluate()
		{
			if (permanentValue == null)
			{
				Stopwatch watch = new Stopwatch();
				watch.Start();
				permanentValue = valueGetter().GetPermanentReference();
				LoggingService.InfoFormatted("Evaluated node '{0}' in {1} ms (result cached for future use)", this.Name, watch.ElapsedMilliseconds);
			}
			return permanentValue;
		}
		
		/// <summary>
		/// Get the value of the node and update the UI text fields.
		/// </summary>
		void EvaluateExpression()
		{
			evaluated = true;
			
			Stopwatch watch = new Stopwatch();
			watch.Start();
			
			Value val;
			try {
				// Do not keep permanent reference
				val = valueGetter();
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
				if (val.ArrayLength > 0) {
					var dims = val.ArrayDimensions;  // Eval now
					this.GetChildren = () => Utils.GetChildNodesOfArray(this, dims, dims);
				}
			} else if (val.Type.IsClass || val.Type.IsValueType) {
				if (val.Type.FullNameWithoutGenericArguments == typeof(List<>).FullName) {
					if ((int)val.GetMemberValue("_size").PrimitiveValue > 0)
						this.GetChildren = () => Utils.GetItemsOfIList(() => this.Evaluate());
				} else {
					this.GetChildren = () => Utils.GetChildNodesOfObject(this, val.Type);
				}
			} else if (val.Type.IsPointer) {
				Value deRef = val.Dereference();
				if (deRef != null) {
					this.GetChildren = () => new ExpressionNode [] { new ExpressionNode(this.ImageName, "*" + this.Name, () => this.Evaluate().Dereference()) };
				}
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
			
			LoggingService.InfoFormatted("Evaluated node '{0}' in {1} ms", this.Name, watch.ElapsedMilliseconds);
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
			Value val = null;
			try {
				val = this.Evaluate();
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
		
		public static string GetImageForMember(IDebugMemberInfo memberInfo)
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
			
			return "Icons.16x16." + name;
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
	
	public partial class Utils
	{
		public static IEnumerable<TreeNode> GetLocalVariableNodes(StackFrame stackFrame)
		{
			foreach(DebugParameterInfo par in stackFrame.MethodInfo.GetParameters()) {
				var parCopy = par;
				yield return new ExpressionNode("Icons.16x16.Parameter", par.Name, () => parCopy.GetValue(stackFrame));
			}
			if (stackFrame.HasSymbols) {
				foreach(DebugLocalVariableInfo locVar in stackFrame.MethodInfo.GetLocalVariables(stackFrame.IP)) {
					var locVarCopy = locVar;
					yield return new ExpressionNode("Icons.16x16.Local", locVar.Name, () => locVarCopy.GetValue(stackFrame));
				}
			} else {
				WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
				if (debugger.debuggerDecompilerService != null) {
					int typeToken = stackFrame.MethodInfo.DeclaringType.MetadataToken;
					int methodToken = stackFrame.MethodInfo.MetadataToken;
					foreach (var localVar in debugger.debuggerDecompilerService.GetLocalVariables(typeToken, methodToken)) {
						int index = ((int[])debugger.debuggerDecompilerService.GetLocalVariableIndex(typeToken, methodToken, localVar))[0];
						yield return new ExpressionNode("Icons.16x16.Local", localVar, () => stackFrame.GetLocalVariableValue((uint)index));
					}
				}
			}
			if (stackFrame.Thread.CurrentException != null) {
				yield return new ExpressionNode(null, "$exception", () => stackFrame.Thread.CurrentException.Value);
			}
		}
	}
	
	public partial class Utils
	{
		public static IEnumerable<TreeNode> GetChildNodesOfObject(ExpressionNode expr, DebugType shownType)
		{
			MemberInfo[] publicStatic      = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.Public    | BindingFlags.Static   | BindingFlags.DeclaredOnly);
			MemberInfo[] publicInstance    = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.Public    | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			MemberInfo[] nonPublicStatic   = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.NonPublic | BindingFlags.Static   | BindingFlags.DeclaredOnly);
			MemberInfo[] nonPublicInstance = shownType.GetFieldsAndNonIndexedProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			
			DebugType baseType = (DebugType)shownType.BaseType;
			if (baseType != null) {
				yield return new TreeNode(
					"Icons.16x16.Class",
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}"),
					baseType.Name,
					baseType.FullName,
					baseType.FullName == "System.Object" ? (Func<IEnumerable<TreeNode>>) null : () => Utils.GetChildNodesOfObject(expr, baseType)
				);
			}
			
			if (nonPublicInstance.Length > 0) {
				yield return new TreeNode(
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}"),
					() => GetMembersOfObject(expr, nonPublicInstance)
				);
			}
			
			if (publicStatic.Length > 0 || nonPublicStatic.Length > 0) {
				yield return new TreeNode(
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}"),
					() => {
						var children = GetMembersOfObject(expr, publicStatic).ToList();
						if (nonPublicStatic.Length > 0) {
							children.Insert(0, new TreeNode(
								StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicStaticMembers}"),
								() => GetMembersOfObject(expr, nonPublicStatic)
							));
						}
						return children;
					}
				);
			}
			
			if (shownType.GetInterface(typeof(IList).FullName) != null) {
				yield return new TreeNode(
					"IList",
					() => GetItemsOfIList(() => expr.Evaluate())
				);
			} else {
				DebugType listType, iEnumerableType, itemType;
				if (shownType.ResolveIEnumerableImplementation(out iEnumerableType, out itemType)) {
					yield return new TreeNode(
						null,
						"IEnumerable",
						"Expanding will enumerate the IEnumerable",
						string.Empty,
						() => GetItemsOfIList(() => DebuggerHelpers.CreateListFromIEnumeralbe(expr.Evaluate(), itemType, out listType))
					);
				}
			}
			
			foreach(TreeNode node in GetMembersOfObject(expr, publicInstance)) {
				yield return node;
			}
		}
		
		public static IEnumerable<TreeNode> GetMembersOfObject(ExpressionNode expr, MemberInfo[] members)
		{
			foreach(MemberInfo memberInfo in members.OrderBy(m => m.Name)) {
				var memberInfoCopy = memberInfo;
				string imageName = ExpressionNode.GetImageForMember((IDebugMemberInfo)memberInfo);
				yield return new ExpressionNode(imageName, memberInfo.Name, () => expr.Evaluate().GetMemberValue(memberInfoCopy));
			}
		}
		
		public static IEnumerable<TreeNode> GetItemsOfIList(Func<Value> getValue)
		{
			Value list = null;
			DebugType iListType = null;
			int count = 0;
			GetValueException error = null;
			try {
				// We use lambda for the value just so that we can get it in this try-catch block
				list = getValue().GetPermanentReference();
				iListType = (DebugType)list.Type.GetInterface(typeof(IList).FullName);
				// Do not get string representation since it can be printed in hex
				count = (int)list.GetPropertyValue(iListType.GetProperty("Count")).PrimitiveValue;
			} catch (GetValueException e) {
				// Cannot yield a value in the body of a catch clause (CS1631)
				error = e;
			}
			if (error != null) {
				yield return new TreeNode(null, "(error)", error.Message, string.Empty, null);
			} else if (count == 0) {
				yield return new TreeNode("(empty)", null);
			} else {
				PropertyInfo pi = iListType.GetProperty("Item");
				for(int i = 0; i < count; i++) {
					int iCopy = i;
					yield return new ExpressionNode("Icons.16x16.Field", "[" + i + "]", () => list.GetPropertyValue(pi, Eval.CreateValue(list.AppDomain, iCopy)));
				}
			}
		}
	}
	
	public partial class Utils
	{
		const int MaxElementCount = 100;
		
		public static TreeNode GetArrayRangeNode(ExpressionNode expr, ArrayDimensions bounds, ArrayDimensions originalBounds)
		{
			StringBuilder name = new StringBuilder();
			bool isFirst = true;
			name.Append("[");
			for(int i = 0; i < bounds.Count; i++) {
				if (!isFirst) name.Append(", ");
				isFirst = false;
				ArrayDimension dim = bounds[i];
				ArrayDimension originalDim = originalBounds[i];
				
				if (dim.Count == 0) {
					name.Append("-");
				} else if (dim.Count == 1) {
					name.Append(dim.LowerBound.ToString());
				} else if (dim.Equals(originalDim)) {
					name.Append("*");
				} else {
					name.Append(dim.LowerBound);
					name.Append("..");
					name.Append(dim.UpperBound);
				}
			}
			name.Append("]");
			
			return new TreeNode(name.ToString(), () => GetChildNodesOfArray(expr, bounds, originalBounds));
		}
		
		public static IEnumerable<TreeNode> GetChildNodesOfArray(ExpressionNode arrayTarget, ArrayDimensions bounds, ArrayDimensions originalBounds)
		{
			if (bounds.TotalElementCount == 0)
			{
				yield return new TreeNode("(empty)", null);
				yield break;
			}
			
			// The whole array is small - just add all elements as childs
			if (bounds.TotalElementCount <= MaxElementCount) {
				foreach(int[] indices in bounds.Indices) {
					StringBuilder sb = new StringBuilder(indices.Length * 4);
					sb.Append("[");
					bool isFirst = true;
					foreach(int index in indices) {
						if (!isFirst) sb.Append(", ");
						sb.Append(index.ToString());
						isFirst = false;
					}
					sb.Append("]");
					int[] indicesCopy = indices;
					yield return new ExpressionNode("Icons.16x16.Field", sb.ToString(), () => arrayTarget.Evaluate().GetArrayElement(indicesCopy));
				}
				yield break;
			}
			
			// Find a dimension of size at least 2
			int splitDimensionIndex = bounds.Count - 1;
			for(int i = 0; i < bounds.Count; i++) {
				if (bounds[i].Count > 1) {
					splitDimensionIndex = i;
					break;
				}
			}
			ArrayDimension splitDim = bounds[splitDimensionIndex];
			
			// Split the dimension
			int elementsPerSegment = 1;
			while (splitDim.Count > elementsPerSegment * MaxElementCount) {
				elementsPerSegment *= MaxElementCount;
			}
			for(int i = splitDim.LowerBound; i <= splitDim.UpperBound; i += elementsPerSegment) {
				List<ArrayDimension> newDims = new List<ArrayDimension>(bounds);
				newDims[splitDimensionIndex] = new ArrayDimension(i, Math.Min(i + elementsPerSegment - 1, splitDim.UpperBound));
				yield return GetArrayRangeNode(arrayTarget, new ArrayDimensions(newDims), originalBounds);
			}
			yield break;
		}
	}
}
