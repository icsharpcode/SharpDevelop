// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	sealed class DebuggerTextOutput : ITextOutput
	{
		readonly ITextOutput output;
		
		public readonly Dictionary<string, MethodDebugSymbols> DebugSymbols = new Dictionary<string, MethodDebugSymbols>();
		public readonly Dictionary<string, ICSharpCode.NRefactory.TextLocation> MemberLocations = new Dictionary<string, ICSharpCode.NRefactory.TextLocation>();
		
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
			if (definition is MemberReference) {
				MemberLocations[XmlDocKeyProvider.GetKey((MemberReference)definition)] = Location;
			}
			output.WriteDefinition(text, definition, isLocal);
		}
		
		public void WriteReference(string text, object reference, bool isLocal)
		{
			output.WriteReference(text, reference, isLocal);
		}
		
		public void AddDebugSymbols(MethodDebugSymbols methodDebugSymbols)
		{
			var id = XmlDocKeyProvider.GetKey(methodDebugSymbols.CecilMethod);
			methodDebugSymbols.SequencePoints = methodDebugSymbols.SequencePoints.OrderBy(s => s.ILOffset).ToList();
			this.DebugSymbols.Add(id, methodDebugSymbols);
			output.AddDebugSymbols(methodDebugSymbols);
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
