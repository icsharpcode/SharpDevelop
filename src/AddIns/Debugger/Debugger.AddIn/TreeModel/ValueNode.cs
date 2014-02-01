// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using Debugger.AddIn.Visualizers;
using Debugger.MetaData;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Debugging;
using ICSharpCode.SharpDevelop.Services;

//using Debugger.AddIn.Visualizers;
//using Debugger.AddIn.Visualizers.Utils;

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
		internal GetValueException error;
		
		public string FullText {
			get { return this.Value; }
		}
		
		public ValueNode(IImage image, string name, Func<Value> getValue, Action<Value> setValue = null)
			: base(image, name, string.Empty, string.Empty, null)
		{
			if (getValue == null)
				throw new ArgumentNullException("getValue");
			
			this.getValue = getValue;
			this.setValue = setValue;
			this.ContextMenuAddInTreeEntry = "/AddIns/Debugger/Tooltips/ContextMenu/ValueNode";
			
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
				LoggingService.InfoFormatted("Evaluated node '{0}' in {1} ms (result cached)", this.Name, watch.ElapsedMilliseconds);
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
		
		/// <summary> Get the value of the node and update the UI text fields. </summary>
		/// <remarks> This should be only called once so the Value is not cached. </remarks>
		void GetValueAndUpdateUI()
		{
			try {
				Stopwatch watch = new Stopwatch();
				watch.Start();
				
				// Do not keep permanent reference
				Value val = this.getValue();
				
				if (val == null) {
					Value = string.Empty;
					Type  = string.Empty;
					GetChildren = null;
					VisualizerCommands = null;
					return;
				}
				
				// Note that the child collections are lazy-evaluated
				if (val.IsNull) {
					this.GetChildren = null;
				} else if (val.Type.IsPrimitiveType() || val.Type.IsKnownType(KnownTypeCode.String)) { // Must be before IsClass
					this.GetChildren = null;
				} else if (val.Type.Kind == TypeKind.Array) { // Must be before IsClass
					var dimBase = val.ArrayBaseIndicies;		// Eval now
					var dimSize = val.ArrayDimensions;		// Eval now
					if (val.ArrayLength > 0) {
						this.GetChildren = () => GetArrayChildren(dimBase, dimSize);
					}
				} else if (val.Type.Kind == TypeKind.Class || val.Type.Kind == TypeKind.Struct) {
					if (val.Type.IsKnownType(typeof(List<>))) {
						if ((int)val.GetFieldValue("_size").PrimitiveValue > 0)
							this.GetChildren = () => GetIListChildren(this.GetValue);
					} else {
						this.GetChildren = () => GetObjectChildren(val.Type);
					}
				} else if (val.Type.Kind == TypeKind.Pointer) {
					if (val.Dereference() != null) {
						this.GetChildren = () => new[] { new ValueNode(ClassBrowserIconService.LocalVariable, "*" + this.Name, () => GetValue().Dereference()) };
					}
				}
				
				// Do last since it may expire the object
				if (val.IsNull) {
					fullValue = "null";
				} else if (val.Type.IsInteger()) {
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
				} else if (val.Type.Kind == TypeKind.Pointer) {
					fullValue = String.Format("0x{0:X}", val.PointerAddress);
				} else if (val.Type.IsKnownType(KnownTypeCode.String)) {
					fullValue = '"' + val.InvokeToString(WindowsDebugger.EvalThread).Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\0", "\\0").Replace("\b", "\\b").Replace("\a", "\\a").Replace("\f", "\\f").Replace("\v", "\\v").Replace("\"", "\\\"") + '"';
				} else if (val.Type.IsKnownType(KnownTypeCode.Char)) {
					fullValue = "'" + val.InvokeToString(WindowsDebugger.EvalThread).Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("\0", "\\0").Replace("\b", "\\b").Replace("\a", "\\a").Replace("\f", "\\f").Replace("\v", "\\v").Replace("\"", "\\\"") + "'";
				} else if ((val.Type.Kind == TypeKind.Class || val.Type.Kind == TypeKind.Struct)) {
					fullValue = val.FormatByDebuggerDisplayAttribute(WindowsDebugger.EvalThread);
					if (fullValue == null)
						fullValue = val.InvokeToString(WindowsDebugger.EvalThread);
				} else if (val.Type.Kind == TypeKind.Enum) {
					var primitiveValue = val.PrimitiveValue;
					var builder = new TypeSystemAstBuilder();
					builder.AlwaysUseShortTypeNames = true;
					AstNode node = builder.ConvertConstantValue(val.Type, primitiveValue);
					fullValue = node + "=" + primitiveValue;
				} else {
					fullValue = val.AsString();
				}
				
				this.error = null;
				this.Value = (fullValue.Length > 256) ? fullValue.Substring(0, 256) + "..." : fullValue;
				this.Type = val.Type.Name;
				
				if (!val.IsNull) {
					this.VisualizerCommands = VisualizerDescriptors.GetAllDescriptors()
						.Where(descriptor => descriptor.IsVisualizerAvailable(val.Type))
						.Select(descriptor => descriptor.CreateVisualizerCommand(this.Name, this.GetValue))
						.ToList();
				}
				
				LoggingService.InfoFormatted("Evaluated node '{0}' in {1} ms", this.Name, watch.ElapsedMilliseconds);
				
			} catch (GetValueException e) {
				error = e;
				this.Value = e.Message;
				this.Type  = string.Empty;
				this.GetChildren = null;
				this.VisualizerCommands = null;
			} finally {
				if (error == null)
					ContextMenuAddInTreeEntry = "/AddIns/Debugger/Tooltips/ContextMenu/ValueNode";
				else
					ContextMenuAddInTreeEntry = "/AddIns/Debugger/Tooltips/ContextMenu/ErrorNode";
			}
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
			var localVars = stackFrame.GetLocalVariables(stackFrame.IP).ToList();
			foreach(var par in stackFrame.MethodInfo.Parameters.Select((p, i) => new { Param = p, Index = i})) {
				var parCopy = par;
				// do not display parameters that have been copied to captured variables twice. (see SD-1912)
				// display only the value of the captured instance (the value of the parameter still has the original value)
				var localVar = localVars.FirstOrDefault(v => string.Equals(v.Name, parCopy.Param.Name, StringComparison.Ordinal));
				if (localVar == null)
					yield return new ValueNode(ClassBrowserIconService.Parameter, par.Param.Name,
					                           () => stackFrame.GetArgumentValue(par.Index));
				else {
					yield return new ValueNode(ClassBrowserIconService.Parameter, localVar.Name,
					                           () => localVar.GetValue(stackFrame));
					localVars.Remove(localVar);
				}
			}
			foreach(LocalVariable locVar in localVars) {
				var locVarCopy = locVar;
				yield return new ValueNode(ClassBrowserIconService.LocalVariable, locVar.Name,
				                           () => locVarCopy.GetValue(stackFrame));
			}
		}
		
		IEnumerable<TreeNode> GetObjectChildren(IType shownType)
		{
			IEnumerable<IMember> fields = shownType.GetFields(f => !f.IsConst, GetMemberOptions.IgnoreInheritedMembers);
			IEnumerable<IMember> properties = shownType.GetProperties(p => p.CanGet && p.Parameters.Count == 0, GetMemberOptions.IgnoreInheritedMembers);
			IEnumerable<IMember> fieldsAndProperties = fields.Concat(properties).ToList();
			
			IEnumerable<IMember> publicStatic      = fieldsAndProperties.Where(m =>  m.IsPublic &&  m.IsStatic);
			IEnumerable<IMember> publicInstance    = fieldsAndProperties.Where(m =>  m.IsPublic && !m.IsStatic);
			IEnumerable<IMember> nonPublicStatic   = fieldsAndProperties.Where(m => !m.IsPublic &&  m.IsStatic);
			IEnumerable<IMember> nonPublicInstance = fieldsAndProperties.Where(m => !m.IsPublic && !m.IsStatic);
			
			IType baseType = shownType.DirectBaseTypes.FirstOrDefault(t => t.Kind != TypeKind.Interface);
			if (baseType != null) {
				yield return new TreeNode(
					ClassBrowserIconService.Class,
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}"),
					baseType.Name,
					string.Empty,
					baseType.FullName == "System.Object" ? (Func<IEnumerable<TreeNode>>) null : () => GetObjectChildren(baseType)
				);
			}
			
			if (nonPublicInstance.Any()) {
				yield return new TreeNode(
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicMembers}"),
					() => GetMembers(nonPublicInstance)
				);
			}
			
			if (publicStatic.Any() || nonPublicStatic.Any()) {
				yield return new TreeNode(
					StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}"),
					() => {
						var children = GetMembers(publicStatic).ToList();
						if (nonPublicStatic.Any()) {
							children.Insert(0, new TreeNode(
								StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.NonPublicStaticMembers}"),
								() => GetMembers(nonPublicStatic)
							));
						}
						return children;
					}
				);
			}
			
			// IList
			if (shownType.GetAllBaseTypeDefinitions().Any(t => t.IsKnownType(KnownTypeCode.IList))) {
				yield return new TreeNode(
					"IList",
					() => GetIListChildren(GetValue)
				);
			}
			
			// IEnumberable<T> (pottentially several of them)
			var ienumerableTypes = shownType.GetAllBaseTypes().OfType<ParameterizedType>().Where(p => p.IsKnownType(KnownTypeCode.IEnumerableOfT));
			foreach(var ienumerableType in ienumerableTypes) {
				var ienumerableTypeCopy = ienumerableType;
				yield return new TreeNode(
					null,
					ienumerableType.Name,
					ienumerableType.ReflectionName,
					string.Empty,
					() => {
						// Note that this will bind to the current content forever and it will not reeveluate
						Value list = CreateListFromIEnumerable(ienumerableTypeCopy, GetValue()).GetPermanentReferenceOfHeapValue();
						return GetIListChildren(() => list);
					}
				);
			}
			
			foreach(TreeNode node in GetMembers(publicInstance)) {
				yield return node;
			}
		}
		
		IEnumerable<TreeNode> GetMembers(IEnumerable<IMember> members)
		{
			foreach(var memberInfo in members.OrderBy(m => m.Name)) {
				var memberInfoCopy = memberInfo;
				var icon = ClassBrowserIconService.GetIcon(memberInfo);
				yield return new ValueNode(icon, memberInfo.Name, () => GetValue().GetMemberValue(WindowsDebugger.EvalThread, memberInfoCopy));
			}
		}
		
		/// <remarks> 'getValue' really should return cached value, because we do Eval to create indices. </remarks>
		static IEnumerable<TreeNode> GetIListChildren(Func<Value> getValue)
		{
			IProperty itemProp;
			int count = 0;
			try {
				Value list = getValue();
				IType iListType = list.Type.GetAllBaseTypeDefinitions().FirstOrDefault(t => t.FullName == typeof(IList).FullName);
				itemProp = iListType.GetProperties(p => p.Name == "Item").Single();
				// Do not get string representation since it can be printed in hex
				count = (int)list.GetPropertyValue(WindowsDebugger.EvalThread, iListType.GetProperties(p => p.Name == "Count").Single()).PrimitiveValue;
			} catch (GetValueException e) {
				return new [] { new TreeNode(null, "(error)", e.Message, string.Empty, null) };
			}
			if (count == 0) {
				return new [] { new TreeNode("(empty)", null) };
			} else {
				return Enumerable.Range(0, count).Select(i => new ValueNode(ClassBrowserIconService.Field, "[" + i + "]", () => getValue().GetPropertyValue(WindowsDebugger.EvalThread, itemProp, Eval.CreateValue(WindowsDebugger.EvalThread, i))));
			}
		}
		
		/// <summary> Evaluates 'new List&lt;T&gt;(iEnumerableValue)' in the debuggee. </summary>
		public static Value CreateListFromIEnumerable(ParameterizedType ienumerableType, Value iEnumerableValue)
		{
			var ilistDef = ienumerableType.Compilation.FindType(typeof(List<>)).GetDefinition();
			var ilistType = new ParameterizedType(ilistDef, ienumerableType.TypeArguments);
			var ctors = ilistType.GetConstructors(m => m.Parameters.Count == 1);
			var ctor = ctors.Single(m => m.Parameters[0].Type.IsKnownType(KnownTypeCode.IEnumerableOfT));
			return Eval.NewObject(WindowsDebugger.EvalThread, ctor, new Value[] { iEnumerableValue });
		}
		
		IEnumerable<TreeNode> GetArrayChildren(uint[] dimBase, uint[] dimSize)
		{
			const int MaxElementCount = 100;
			
			int rank = dimSize.Length;
			uint totalSize = dimSize.Aggregate((uint)1, (acc, s) => acc * s);
			
			if (totalSize == 0)
			{
				yield return new TreeNode("(empty)", null);
				yield break;
			}
			
			// The array is small - just add all elements as children
			StringBuilder sb = new StringBuilder();
			if (totalSize <= MaxElementCount) {
				uint[] indices = (uint[])dimBase.Clone();
				while(indices[0] < dimBase[0] + dimSize[0]) {
					// Make element name
					sb.Clear();
					sb.Append('[');
					bool isFirst = true;
					foreach(int index in indices) {
						if (!isFirst) sb.Append(", ");
						sb.Append(index);
						isFirst = false;
					}
					sb.Append(']');
					
					// The getValue delegate might be called later or several times
					uint[] indicesCopy = (uint[])indices.Clone();
					yield return new ValueNode(ClassBrowserIconService.Field, sb.ToString(), () => GetValue().GetArrayElement(indicesCopy));
					
					// Get next combination
					indices[rank - 1]++;
					for (int i = rank - 1; i > 0; i--) {
						if (indices[i] >= dimBase[i] + dimSize[i]) {
							indices[i] = dimBase[i];
							indices[i - 1]++;
						}
					}
				}
				yield break;
			}
			
			// Split the array into smaller subsets
			int splitIndex = Array.FindIndex(dimSize, s => (s > 1));
			uint groupSize = 1;
			while (dimSize[splitIndex] > groupSize * MaxElementCount) {
				groupSize *= MaxElementCount;
			}
			for(uint i = dimBase[splitIndex]; i < dimBase[splitIndex] + dimSize[splitIndex]; i += groupSize) {
				// Get the base&size for the subset
				uint[] newDimBase = (uint[])dimBase.Clone();
				uint[] newDimSize = (uint[])dimSize.Clone();
				newDimBase[splitIndex] = i;
				newDimSize[splitIndex] = Math.Min(groupSize, dimBase[splitIndex] + dimSize[splitIndex] - i);
				
				// Make the subset name
				sb.Clear();
				sb.Append('[');
				bool isFirst = true;
				for (int j = 0; j < rank; j++) {
					if (!isFirst) sb.Append(", ");
					if (j < splitIndex) {
						sb.Append(newDimBase[j]);
					} else if (j == splitIndex) {
						sb.Append(i);
						if (newDimSize[splitIndex] > 1) {
							sb.Append("..");
							sb.Append(i + newDimSize[splitIndex] - 1);
						}
					} else if (j > splitIndex) {
						sb.Append('*');
					}
					isFirst = false;
				}
				sb.Append(']');
				
				yield return new TreeNode(sb.ToString(), () => GetArrayChildren(newDimBase, newDimSize));
			}
		}
	}
}
