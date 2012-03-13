// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;

namespace Debugger.MetaData
{
	public class DebugLocalVariableInfo: System.Reflection.LocalVariableInfo
	{
		ValueGetter getter;
		int localIndex;
		DebugType localType;
		
		public DebugMethodInfo Member { get; private set; }
		
		/// <inheritdoc/>
		public override int LocalIndex {
			get { return localIndex; }
		}
		
		/// <inheritdoc/>
		public override Type LocalType {
			get { return localType; }
		}
		
		/// <inheritdoc/>
		public override bool IsPinned {
			get { throw new NotSupportedException(); }
		}
		
		public string Name { get; internal set; }
		/// <summary> IL offset of the start of the variable scope (inclusive) </summary>
		public int StartOffset { get; private set; }
		/// <summary> IL offset of the end of the variable scope (exclusive) </summary>
		public int EndOffset { get; private set; }
		public bool IsThis { get; internal set; }
		public bool IsCaptured { get; internal set; }
		
		public DebugLocalVariableInfo(DebugMethodInfo member, string name, int localIndex, int startOffset, int endOffset, DebugType localType, ValueGetter getter)
		{
			this.Member = member;
			this.Name = name;
			this.localIndex = localIndex;
			this.StartOffset = startOffset;
			this.EndOffset = endOffset;
			this.localType = localType;
			this.getter = getter;
		}
		
		public Value GetValue(StackFrame context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			if (context.MethodInfo != this.Member)
				throw new GetValueException("Expected stack frame: " + this.Member.ToString());
			
			return getter(context);
		}
		
		/// <inheritdoc/>
		public override string ToString()
		{
			string msg = this.LocalType + " " + this.Name;
			if (IsCaptured)
				msg += " (captured)";
			return msg;
		}
	}
}
