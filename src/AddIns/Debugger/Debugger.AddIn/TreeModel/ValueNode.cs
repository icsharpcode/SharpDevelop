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
//using Debugger.AddIn.Visualizers;
//using Debugger.AddIn.Visualizers.Utils;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Gui.Pads;
using ICSharpCode.SharpDevelop.Services;

namespace Debugger.AddIn.TreeModel
{
	/// <summary>
	/// Tree node which represents debuggee's <see cref="Value"/>.
	/// The node stores a lambda which can be used to reobtain the value
	/// at any time (possibly even after some stepping).
	/// </summary>
	/// <remarks>
	/// The general rule is that getting a value or getting children will
	/// either succeed or result in <see cref="GetValueException"/>.
	/// </remarks>
	public class ValueNode: TreeNode
	{
		Func<Value> getValue;
		Action<Value> setValue;
		
		Value cachedValue;
		Debugger.Process cachedValueProcess;
		long cachedValueDebuggeeState;
		
		string fullValue;
		GetValueException error;
		
		public string FullText {
			get { return this.Value; }
		}
		
		public ValueNode(string imageName, string name, Func<Value> getValue, Action<Value> setValue = null)
			: base(imageName, name, string.Empty, string.Empty, null)
		{
			if (getValue == null)
				throw new ArgumentNullException("getValue");
			
			this.getValue = getValue;
			this.setValue = setValue;
			
			GetValueAndUpdateUI();
		}
		
		/// <summary>
		/// Get the value of the node and cache it as long-lived reference.
		/// We assume that the user will need this value a lot.
		/// </summary>
		public Value GetValue()
		{
			// The value still survives across debuggee state, but we want a fresh one for the UI
			if (cachedValue == null || cachedValueProcess.DebuggeeState != cachedValueDebuggeeState)
			{
				Stopwatch watch = new Stopwatch();
				watch.Start();
				cachedValue = this.getValue().GetPermanentReference(WindowsDebugger.EvalThread);
				cachedValueProcess = cachedValue.Process;
				cachedValueDebuggeeState = cachedValue.Process.DebuggeeState;
				LoggingService.InfoFormatted("Evaluated node '{0}' in {1} ms (result cached for future use)", this.Name, watch.ElapsedMilliseconds);
			}
			return cachedValue;
		}
		
		public void SetValue(Value value)
		{
			if (setValue == null)
				throw new DebuggerException("Setting of value is not supported for this node");
			
			try
			{
				this.setValue(value);
			}
			catch(GetValueException e)
			{
				MessageService.ShowMessage(e.Message, "${res:MainWindow.Windows.Debug.LocalVariables.CannotSetValue.Title}");
			}
		}
		
		/// <summary>
		/// Get the value of the node and update the UI text fields.
		/// </summary>
		void GetValueAndUpdateUI()
		{			
			try {
				Stopwatch watch = new Stopwatch();
				watch.Start();
				
				// Do not keep permanent reference
				Value val = this.getValue();
				
				// Note that the child collections are lazy-evaluated
				if (val.IsNull) {
					this.GetChildren = null;
				} else if (val.Type.IsPrimitive || val.Type.FullName == typeof(string).FullName) { // Must be before IsClass
					this.GetChildren = null;
				} else if (val.Type.IsArray) { // Must be before IsClass
					var dims = val.ArrayDimensions;  // Eval now
					if (dims.TotalElementCount > 0) {
						this.GetChildren = () => GetArrayChildren(dims, dims);
					}
				} else if (val.Type.IsClass || val.Type.IsValueType) {
					if (val.Type.FullNameWithoutGenericArguments == typeof(List<>).FullName) {
						if ((int)val.GetFieldValue("_size").PrimitiveValue > 0)
							this.GetChildren = () => GetIListChildren(this.GetValue);
					} else {
						this.GetChildren = () => GetObjectChildren(val.Type);
					}
				} else if (val.Type.IsPointer) {
					if (val.Dereference() != null) {
						this.GetChildren = () => new[] { new ValueNode("Icons.16x16.Local", "*" + this.Name, () => GetValue().Dereference()) };
					}
				}
				
				// Do last since it may expire the object
				if (val.IsNull) {
					fullValue = "null";
				} else if (val.Type.IsInteger) {
					var i = val.PrimitiveValue;
					if (DebuggingOptions.Instance.ShowIntegersAs == ShowIntegersAs.Decimal) {
						fullValue = i.ToString();
					} else {
						string hex = string.Format("0x{0:X4}", i);
						if (hex.Length > 6 ) hex = string.Format("0x{0:X8}", i);
						if (hex.Length > 10) hex = string.Format("0x{0:X16}", i);
						if (DebuggingOptions.Instance.ShowIntegersAs == ShowIntegersAs.Hexadecimal) {
							fullValue = hex;
						} else {
							fullValue = string.Format("{0} ({1})", i, hex);
						}
					}
				} else if (val.Type.IsPointer) {
					fullValue = String.Format("0x{0:X}", val.PointerAddress);
				} else if (val.Type.FullName == typeof(string).FullName) {
					fullValue = '"' + val.InvokeToString(WindowsDebugger.EvalThread).Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\0", "\\0").Replace("\b", "\\b").Replace("\a", "\\a").Replace("\f", "\\f").Replace("\v", "\\v").Replace("\"", "\\\"") + '"';
				} else if (val.Type.FullName == typeof(char).FullName) {
					fullValue = "'" + val.InvokeToString(WindowsDebugger.EvalThread).Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\0", "\\0").Replace("\b", "\\b").Replace("\a", "\\a").Replace("\f", "\\f").Replace("\v", "\\v").Replace("\"", "\\\"") + "'";
				} else if ((val.Type.IsClass || val.Type.IsValueType)) {
					fullValue = val.InvokeToString(WindowsDebugger.EvalThread);
				} else {
					fullValue = val.AsString();
				}
				
				this.error = null;
				this.Value = (fullValue.Length > 256) ? fullValue.Substring(0, 256) + "..." : fullValue;
				this.Type = val.Type.Name;
				
				if (!val.IsNull) {
#warning					this.VisualizerCommands = VisualizerDescriptors.GetAllDescriptors()
//						.Where(descriptor => descriptor.IsVisualizerAvailable(val.Type))
//						.Select(descriptor => descriptor.CreateVisualizerCommand(this.Name, this.GetValue))
//						.ToList();
				}
				
				LoggingService.InfoFormatted("Evaluated node '{0}' in {1} ms", this.Name, watch.ElapsedMilliseconds);
			
			} catch (GetValueException e) {
				error = e;
				this.Value = e.Message;
				this.Type  = string.Empty;
				this.GetChildren = null;
#warning				this.VisualizerCommands = null;
				return;
			}
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
		
		ContextMenuStrip GetErrorContextMenu()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			
			ToolStripMenuItem showError = new ToolStripMenuItem();
			showError.Text = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.ShowFullError}");
			showError.Click += delegate { MessageService.ShowException(error, null); };
			menu.Items.Add(showError);
			
			return menu;
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
		
		/// <summary>
		/// The root of any node evaluation is valid stack frame.
		/// </summary>
		static StackFrame GetCurrentStackFrame()
		{
			if (WindowsDebugger.CurrentProcess == null)
				throw new GetValueException("Debugger is not running");
			if (WindowsDebugger.CurrentProcess.IsRunning)
				throw new GetValueException("Process is not paused");
			if (WindowsDebugger.CurrentStackFrame == null)
				throw new GetValueException("No stack frame selected");
			
			return WindowsDebugger.CurrentStackFrame;
		}
		
		public static IEnumerable<TreeNode> GetLocalVariables()
		{
			var stackFrame = GetCurrentStackFrame();
			foreach(DebugParameterInfo par in stackFrame.MethodInfo.GetParameters()) {
				var parCopy = par;
				yield return new ValueNode("Icons.16x16.Parameter", par.Name, () => parCopy.GetValue(GetCurrentStackFrame()));
			}
			if (stackFrame.HasSymbols) {
				foreach(DebugLocalVariableInfo locVar in stackFrame.MethodInfo.GetLocalVariables(stackFrame.IP)) {
					var locVarCopy = locVar;
					yield return new ValueNode("Icons.16x16.Local", locVar.Name, () => locVarCopy.GetValue(GetCurrentStackFrame()));
				}
			} else {
				WindowsDebugger debugger = (WindowsDebugger)DebuggerService.CurrentDebugger;
				if (debugger.debuggerDecompilerService != null) {
					int typeToken = stackFrame.MethodInfo.DeclaringType.MetadataToken;
					int methodToken = stackFrame.MethodInfo.MetadataToken;
					foreach (var localVar in debugger.debuggerDecompilerService.GetLocalVariables(typeToken, methodToken)) {
						int index = ((int[])debugger.debuggerDecompilerService.GetLocalVariableIndex(typeToken, methodToken, localVar))[0];
						yield return new ValueNode("Icons.16x16.Local", localVar, () => {
						    var newStackFrame = GetCurrentStackFrame();
							if (newStackFrame.MethodInfo != stackFrame.MethodInfo)
								throw new GetValueException("Expected stack frame: " + stackFrame.MethodInfo.ToString());
							
							return newStackFrame.GetLocalVariableValue((uint)index);
						});
					}
				}
			}
		}
		
		IEnumerable<TreeNode> GetObjectChildren(DebugType shownType)
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
					baseType.FullName == "System.Object" ? (Func<IEnumerable<TreeNode>>) null : () => GetObjectChildren(baseType)
				);
			}
			
			if (nonPublicInstance.Length > 0) {
				yield return new TreeNode(
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}"),
					() => GetMembers(nonPublicInstance)
				);
			}
			
			if (publicStatic.Length > 0 || nonPublicStatic.Length > 0) {
				yield return new TreeNode(
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}"),
					() => {
						var children = GetMembers(publicStatic).ToList();
						if (nonPublicStatic.Length > 0) {
							children.Insert(0, new TreeNode(
								StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicStaticMembers}"),
								() => GetMembers(nonPublicStatic)
							));
						}
						return children;
					}
				);
			}
			
			if (shownType.GetInterface(typeof(IList).FullName) != null) {
				yield return new TreeNode(
					"IList",
					() => GetIListChildren(GetValue)
				);
			} else {
				DebugType iEnumerableType, itemType;
				#warning reimplement this!
//				if (shownType.ResolveIEnumerableImplementation(out iEnumerableType, out itemType)) {
//					yield return new TreeNode(
//						null,
//						"IEnumerable",
//						"Expanding will enumerate the IEnumerable",
//						string.Empty,
//						() => GetIListChildren(() => DebuggerHelpers.CreateListFromIEnumerable(GetValue()))
//					);
//				}
			}
			
			foreach(TreeNode node in GetMembers(publicInstance)) {
				yield return node;
			}
		}
		
		IEnumerable<TreeNode> GetMembers(MemberInfo[] members)
		{
			foreach(MemberInfo memberInfo in members.OrderBy(m => m.Name)) {
				var memberInfoCopy = memberInfo;
				string imageName = GetImageForMember((IDebugMemberInfo)memberInfo);
				yield return new ValueNode(imageName, memberInfo.Name, () => GetValue().GetMemberValue(WindowsDebugger.EvalThread, memberInfoCopy));
			}
		}
		
		static IEnumerable<TreeNode> GetIListChildren(Func<Value> getValue)
		{
			Value list;
			PropertyInfo itemProp;
			int count = 0;
			try {
				// TODO: We want new list on reeval
				// We need the list to survive generation of index via Eval
				list = getValue().GetPermanentReference(WindowsDebugger.EvalThread);
				DebugType iListType = (DebugType)list.Type.GetInterface(typeof(IList).FullName);
				itemProp = iListType.GetProperty("Item");
				// Do not get string representation since it can be printed in hex
				count = (int)list.GetPropertyValue(WindowsDebugger.EvalThread, iListType.GetProperty("Count")).PrimitiveValue;
			} catch (GetValueException e) {
				return new [] { new TreeNode(null, "(error)", e.Message, string.Empty, null) };
			}
			if (count == 0) {
				return new [] { new TreeNode("(empty)", null) };
			} else {
				return Enumerable.Range(0, count).Select(i => new ValueNode("Icons.16x16.Field", "[" + i + "]", () => list.GetPropertyValue(WindowsDebugger.EvalThread, itemProp, Eval.CreateValue(WindowsDebugger.EvalThread, i))));
			}
		}
		
		TreeNode GetArraySubsetNode(ArrayDimensions bounds, ArrayDimensions originalBounds)
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
			
			return new TreeNode(name.ToString(), () => GetArrayChildren(bounds, originalBounds));
		}
		
		IEnumerable<TreeNode> GetArrayChildren(ArrayDimensions bounds, ArrayDimensions originalBounds)
		{
			const int MaxElementCount = 1000;
			
			if (bounds.TotalElementCount == 0)
			{
				yield return new TreeNode("(empty)", null);
				yield break;
			}
			
			// The whole array is small - just add all elements as childs
			if (bounds.TotalElementCount <= MaxElementCount * 2) {
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
					yield return new ValueNode("Icons.16x16.Field", sb.ToString(), () => GetValue().GetArrayElement(indicesCopy));
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
				yield return GetArraySubsetNode(new ArrayDimensions(newDims), originalBounds);
			}
			yield break;
		}
	}
}
