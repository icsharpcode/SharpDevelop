// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Produces MethodInsightItem instances for showing the insight window on method calls.
	/// </summary>
	public class MethodInsightProvider
	{
		/// <summary>
		/// Gets/Sets the offset where lookup is done.
		/// The default value is -1, which instructs the insight provider to use the caret position.
		/// </summary>
		public int LookupOffset { get; set; }
		
		public MethodInsightProvider()
		{
			this.LookupOffset = -1;
		}
		
		public IInsightItem[] ProvideInsight(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(editor.FileName);
			if (expressionFinder == null) {
				return null;
			}
			
			IDocument document = editor.Document;
			int useOffset = (this.LookupOffset < 0) ? editor.Caret.Offset : this.LookupOffset;
			ExpressionResult expressionResult = expressionFinder.FindExpression(document.Text, useOffset);
			
			if (expressionResult.Expression == null) // expression is null when cursor is in string/comment
				return null;
			
			if (LoggingService.IsDebugEnabled) {
				if (expressionResult.Context == ExpressionContext.Default)
					LoggingService.DebugFormatted("ShowInsight for >>{0}<<", expressionResult.Expression);
				else
					LoggingService.DebugFormatted("ShowInsight for >>{0}<<, context={1}", expressionResult.Expression, expressionResult.Context);
			}
			
			var position = document.OffsetToPosition(useOffset);
			ResolveResult rr = ParserService.Resolve(expressionResult, position.Line, position.Column, editor.FileName, document.Text);
			return ProvideInsight(expressionResult, rr);
		}
		
		public virtual IInsightItem[] ProvideInsight(ExpressionResult expressionResult, ResolveResult result)
		{
			if (result == null)
				return null;
			
			bool constructorInsight = false;
			if (expressionResult.Context == ExpressionContext.Attribute) {
				constructorInsight = true;
			} else if (expressionResult.Context.IsObjectCreation) {
				constructorInsight = true;
				expressionResult.Context = ExpressionContext.Type;
			} else if (expressionResult.Context == CSharpExpressionContext.BaseConstructorCall) {
				constructorInsight = true;
			}
			
			LanguageProperties language;
			if (result.CallingClass != null)
				language = result.CallingClass.CompilationUnit.Language;
			else
				language = ParserService.CurrentProjectContent.Language;
			
			TypeResolveResult trr = result as TypeResolveResult;
			if (trr == null && language.AllowObjectConstructionOutsideContext) {
				if (result is MixedResolveResult)
					trr = (result as MixedResolveResult).TypeResult;
			}
			if (trr != null && !constructorInsight) {
				if (language.AllowObjectConstructionOutsideContext)
					constructorInsight = true;
			}
			
			List<IMethod> methods = new List<IMethod>();
			if (constructorInsight) {
				if (trr != null || expressionResult.Context == CSharpExpressionContext.BaseConstructorCall) {
					if (result.ResolvedType != null) {
						methods.AddRange(GetConstructors(result.ResolvedType));
					}
				}
			} else {
				MethodGroupResolveResult mgrr = result as MethodGroupResolveResult;
				if (mgrr == null)
					return null;
				bool classIsInInheritanceTree = false;
				if (result.CallingClass != null)
					classIsInInheritanceTree = result.CallingClass.IsTypeInInheritanceTree(mgrr.ContainingType.GetUnderlyingClass());
				
				foreach (IMethod method in mgrr.ContainingType.GetMethods()) {
					if (language.NameComparer.Equals(method.Name, mgrr.Name)) {
						if (method.IsAccessible(result.CallingClass, classIsInInheritanceTree)) {
							methods.Add(method);
						}
					}
				}
				if (methods.Count == 0 && result.CallingClass != null && language.SupportsExtensionMethods) {
					List<IMethodOrProperty> list = new List<IMethodOrProperty>();
					ResolveResult.AddExtensions(language, list.Add, result.CallingClass, mgrr.ContainingType);
					foreach (IMethodOrProperty mp in list) {
						if (language.NameComparer.Equals(mp.Name, mgrr.Name) && mp is IMethod) {
							DefaultMethod m = (DefaultMethod)mp.CreateSpecializedMember();
							// for the insight window, remove first parameter and mark the
							// method as normal - this is required to show the list of
							// parameters the method expects.
							m.IsExtensionMethod = false;
							m.Modifiers ^= ModifierEnum.Static;
							m.Parameters.RemoveAt(0);
							methods.Add(m);
						}
					}
				}
			}
			return methods.Select(m => new MethodInsightItem(m)).ToArray();
		}
		
		IEnumerable<IMethod> GetConstructors(IReturnType rt)
		{
			// no need to add default constructor here:
			// default constructors should already be present in rt.GetMethods()
			if (rt == null)
				return Enumerable.Empty<IMethod>();
			else
				return rt.GetMethods().Where(m => m.IsConstructor && !m.IsStatic);
		}
	}
}
