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
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Provides contextual position information while iterating through
	/// the AST and the ability to resolve expressions in-place.
	/// </summary>
	public abstract class PositionTrackingAstVisitor : ICSharpCode.NRefactory.Visitors.NodeTrackingAstVisitor
	{
		readonly Stack<INode> parentNodes = new Stack<INode>();
		
		readonly string fileName;
		readonly string fileContent;
		
		protected override void BeginVisit(INode node)
		{
			base.BeginVisit(node);
			// Only push nodes on the stack which have valid position information.
			if (node != null &&
			    node.StartLocation.X >= 1 && node.StartLocation.Y >= 1 &&
			    node.EndLocation.X >= 1 && node.EndLocation.Y >= 1) {
				this.parentNodes.Push(node);
			}
		}
		
		protected override void EndVisit(INode node)
		{
			base.EndVisit(node);
			// Only remove those nodes which have actually been pushed before.
			if (this.parentNodes.Count > 0 && INode.ReferenceEquals(this.parentNodes.Peek(), node)) {
				this.parentNodes.Pop();
			}
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Gets a flag that indicates whether the current node is located
		/// inside a block which position information is available for.
		/// </summary>
		protected bool PositionAvailable {
			get {
				return this.parentNodes.Count > 0;
			}
		}
		
		/// <summary>
		/// Gets the start location of the current innermost node with valid position information
		/// as 1-based coordinates in the parsed document.
		/// X = column number, Y = line number.
		/// </summary>
		protected Location CurrentNodeStartLocation {
			get {
				return this.parentNodes.Peek().StartLocation;
			}
		}
		
		/// <summary>
		/// Gets the end location of the current innermost node with valid position information
		/// as 1-based coordinates in the parsed document.
		/// X = column number, Y = line number.
		/// </summary>
		protected Location CurrentNodeEndLocation {
			get {
				return this.parentNodes.Peek().EndLocation;
			}
		}
		
		protected string FileName {
			get { return fileName; }
		}
		
		protected string FileContent {
			get { return fileContent; }
		}
		
		// ********************************************************************************************************************************
		
		private CompilationUnit compilationUnit;
		
		public override object TrackedVisitCompilationUnit(CompilationUnit compilationUnit, object data)
		{
			this.compilationUnit = compilationUnit;
			return base.TrackedVisitCompilationUnit(compilationUnit, data);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Resolves an expression in the current node's context.
		/// </summary>
		/// <param name="expression">The expression to be resolved.</param>
		public ResolveResult Resolve(Expression expression)
		{
			return this.Resolve(expression, ExpressionContext.Default);
		}
		
		/// <summary>
		/// Resolves an expression in the current node's context.
		/// </summary>
		/// <param name="expression">The expression to be resolved.</param>
		/// <param name="context">The ExpressionContext.</param>
		public ResolveResult Resolve(Expression expression, ExpressionContext context)
		{
			if (!this.PositionAvailable) {
				LoggingService.Info("ResourceToolkit: PositionTrackingAstVisitor: Resolve failed due to position information being unavailable. Expression: "+expression.ToString());
				return null;
			}
			
			#if DEBUG
			LoggingService.Debug("ResourceToolkit: PositionTrackingAstVisitor: Using this parent node for resolve: "+this.parentNodes.Peek().ToString());
			#endif
			
			return NRefactoryAstCacheService.ResolveLowLevel(this.fileName, this.fileContent, this.CurrentNodeStartLocation.Y, this.CurrentNodeStartLocation.X+1, this.compilationUnit, null, expression, context);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PositionTrackingAstVisitor"/> class.
		/// </summary>
		protected PositionTrackingAstVisitor(string fileName, string fileContent)
			: base()
		{
			if (fileName == null) {
				throw new ArgumentNullException("fileName");
			}
			if (fileContent == null) {
				throw new ArgumentNullException("fileContent");
			}
			
			this.fileName = fileName;
			this.fileContent = fileContent;
		}
	}
}
