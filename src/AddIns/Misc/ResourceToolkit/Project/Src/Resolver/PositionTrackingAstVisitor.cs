// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Provides contextual position information while iterating through
	/// the AST and the ability to resolve expressions in-place.
	/// </summary>
	public abstract class PositionTrackingAstVisitor : NodeTrackingAstVisitor
	{
		
		private Stack<INode> parentNodes;
		
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
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Resolves an expression in the current node's context.
		/// </summary>
		/// <param name="expression">The expression to be resolved.</param>
		/// <param name="memberInThisFile">Any member declared in the source file in question. Used to get the language, file name and file content.</param>
		public ResolveResult Resolve(Expression expression, IMember memberInThisFile)
		{
			if (!this.PositionAvailable) {
				LoggingService.Debug("ResourceToolkit: PositionTrackingAstVisitor: Resolve failed due to position information being unavailable. Expression: "+expression.ToString());
				return null;
			}
			
			// In order to resolve expression, we need the original code.
			// HACK: To get the code belonging to this expression, we pass it through the code generator.
			// (Is there a better way?)
			
			string code = null;
			LanguageProperties lp = NRefactoryResourceResolver.GetLanguagePropertiesForMember(memberInThisFile);
			if (lp != null && lp.CodeGenerator != null) {
				code = lp.CodeGenerator.GenerateCode(expression, String.Empty);
			}
			
			if (!String.IsNullOrEmpty(code)) {
				
				// Now resolve the expression in the current context.
				#if DEBUG
				LoggingService.Debug("ResourceToolkit: PositionTrackingAstVisitor: Resolving an expression, code (re-generated): '"+code+"'");
				#endif
				ResolveResult rr = ParserService.Resolve(new ExpressionResult(code, ExpressionContext.Default), this.CurrentNodeStartLocation.Y-1, this.CurrentNodeStartLocation.X, memberInThisFile.DeclaringType.CompilationUnit.FileName, ParserService.GetParseableFileContent(memberInThisFile.DeclaringType.CompilationUnit.FileName));
				
				#if DEBUG
				if (rr != null) {
					LoggingService.Debug("ResourceToolkit: PositionTrackingAstVisitor: The expression resolved to: "+rr.ToString());
				} else {
					LoggingService.Debug("ResourceToolkit: PositionTrackingAstVisitor: The expression could not be resolved.");
				}
				#endif
				
				return rr;
				
			} else {
				LoggingService.Debug("ResourceToolkit: PositionTrackingAstVisitor could not re-generate code for the expression: "+expression.ToString());
			}
			
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PositionTrackingAstVisitor"/> class.
		/// </summary>
		protected PositionTrackingAstVisitor() : base()
		{
			this.parentNodes = new Stack<INode>();
		}
	}
}
