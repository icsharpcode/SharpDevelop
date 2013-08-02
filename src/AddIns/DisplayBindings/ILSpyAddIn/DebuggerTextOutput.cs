// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.Decompiler;
using ICSharpCode.NRefactory.CSharp;
using Mono.Cecil;

namespace ICSharpCode.ILSpyAddIn
{
	public sealed class DebugInfoTokenWriterDecorator : DecoratingTokenWriter, ILocatable
	{
		readonly Stack<MethodDebugSymbols> symbolsStack = new Stack<MethodDebugSymbols>();
		readonly ILocatable locationProvider;
		
		public readonly Dictionary<string, MethodDebugSymbols> DebugSymbols = new Dictionary<string, MethodDebugSymbols>();
		public readonly Dictionary<string, ICSharpCode.NRefactory.TextLocation> MemberLocations = new Dictionary<string, ICSharpCode.NRefactory.TextLocation>();
		
		public DebugInfoTokenWriterDecorator(TokenWriter writer, ILocatable locationProvider)
			: base(writer)
		{
			if (locationProvider == null)
				throw new ArgumentNullException("locationProvider");
			this.locationProvider = locationProvider;
		}
		
		public override void StartNode(AstNode node)
		{
			base.StartNode(node);
			if (node.Annotation<MethodDebugSymbols>() != null) {
				symbolsStack.Push(node.Annotation<MethodDebugSymbols>());
			}
		}
		
		public override void EndNode(AstNode node)
		{
			base.EndNode(node);
			if (node is EntityDeclaration && node.Annotation<MemberReference>() != null) {
				MemberLocations[XmlDocKeyProvider.GetKey(node.Annotation<MemberReference>())] = node.StartLocation;
			}
			if (node.Annotation<MethodDebugSymbols>() != null) {
				var symbols = symbolsStack.Pop();
				symbols.SequencePoints = symbols.SequencePoints.OrderBy(s => s.ILOffset).ToList();
				symbols.StartLocation = node.StartLocation;
				DebugSymbols[XmlDocKeyProvider.GetKey(symbols.CecilMethod)] = symbols;
			}
		}
		
		public ICSharpCode.NRefactory.TextLocation Location {
			get { return locationProvider.Location; }
		}
	}
}
