// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.Decompiler;

namespace ICSharpCode.ILSpyAddIn
{
	sealed class DebuggerTextOutput : ITextOutput
	{
		readonly ITextOutput output;
		
		public readonly List<MemberMapping> DebuggerMemberMappings = new List<MemberMapping>();
		
		public DebuggerTextOutput(ITextOutput output)
		{
			this.output = output;
		}
		
		public ICSharpCode.NRefactory.TextLocation Location {
			get { return output.Location; }
		}
		
		public void Indent()
		{
			output.Indent();
		}
		
		public void Unindent()
		{
			output.Unindent();
		}
		
		public void Write(char ch)
		{
			output.Write(ch);
		}
		
		public void Write(string text)
		{
			output.Write(text);
		}
		
		public void WriteLine()
		{
			output.WriteLine();
		}
		
		public void WriteDefinition(string text, object definition, bool isLocal)
		{
			output.WriteDefinition(text, definition, isLocal);
		}
		
		public void WriteReference(string text, object reference, bool isLocal)
		{
			output.WriteReference(text, reference, isLocal);
		}
		
		public void AddDebuggerMemberMapping(MemberMapping memberMapping)
		{
			DebuggerMemberMappings.Add(memberMapping);
			output.AddDebuggerMemberMapping(memberMapping);
		}
		
		public void MarkFoldStart(string collapsedText, bool defaultCollapsed)
		{
			output.MarkFoldStart(collapsedText, defaultCollapsed);
		}
		
		public void MarkFoldEnd()
		{
			output.MarkFoldEnd();
		}
	}
}
