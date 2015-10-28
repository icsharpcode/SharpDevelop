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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Analysis;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using CSharpBinding.Parser;

namespace CSharpBinding
{
	class CSharpSemanticHighlighterVisitor : SemanticHighlightingVisitor<HighlightingColor>, IDisposable
	{
		readonly CSharpSemanticHighlighter highlighter;
		readonly IDocument document;

		#region Constructor + Dispose
		public CSharpSemanticHighlighterVisitor(CSharpSemanticHighlighter highlighter, IDocument document)
		{
			if (highlighter == null)
				throw new ArgumentNullException("highlighter");
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
			this.highlighter = highlighter;
			
			var highlighting = HighlightingManager.Instance.GetDefinition("C#");
			//this.defaultTextColor = ???;
			this.referenceTypeColor = highlighting.GetNamedColor("ReferenceTypes");
			this.valueTypeColor = highlighting.GetNamedColor("ValueTypes");
			this.interfaceTypeColor = highlighting.GetNamedColor("InterfaceTypes");
			this.enumerationTypeColor = highlighting.GetNamedColor("EnumTypes");
			this.typeParameterTypeColor = highlighting.GetNamedColor("TypeParameters");
			this.delegateTypeColor = highlighting.GetNamedColor("DelegateType");
			this.methodDeclarationColor = this.methodCallColor = highlighting.GetNamedColor("MethodCall");
			//this.eventDeclarationColor = this.eventAccessColor = defaultTextColor;
			//this.propertyDeclarationColor = this.propertyAccessColor = defaultTextColor;
			this.fieldDeclarationColor = this.fieldAccessColor = highlighting.GetNamedColor("FieldAccess");
			//this.variableDeclarationColor = this.variableAccessColor = defaultTextColor;
			//this.parameterDeclarationColor = this.parameterAccessColor = defaultTextColor;
			this.valueKeywordColor = highlighting.GetNamedColor("NullOrValueKeywords");
			//this.externAliasKeywordColor = ...;
			this.parameterModifierColor = highlighting.GetNamedColor("ParameterModifiers");
			this.inactiveCodeColor = highlighting.GetNamedColor("InactiveCode");
			this.syntaxErrorColor = highlighting.GetNamedColor("SemanticError");
		}

		public void Dispose()
		{
			resolver = null;
		}
		#endregion
		
		#region Colorize
		protected override void Colorize(TextLocation start, TextLocation end, HighlightingColor color)
		{
			highlighter.Colorize(start, end, color);
		}
		#endregion
		
		#if DEBUG
		public override void VisitSyntaxTree(ICSharpCode.NRefactory.CSharp.SyntaxTree syntaxTree)
		{
			base.VisitSyntaxTree(syntaxTree);
			
		}
		#endif
		
		internal void UpdateLineInformation(int lineNumber)
		{
			regionStart = new TextLocation(lineNumber, 1);
			regionEnd = new TextLocation(lineNumber, 1 + document.GetLineByNumber(lineNumber).Length);
		}

		internal CSharpAstResolver Resolver {
			get {
				return resolver;
			}
			set {
				this.resolver = value;
			}
		}
	}
}


