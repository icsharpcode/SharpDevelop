// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Project;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Parses files using NRefactory and caches the AST on demand.
	/// Provides an event to monitor the cache status and
	/// the <see cref="ResolveLowLevel"/> method to resolve expressions
	/// faster by making use of the cache.
	/// </summary>
	public static class NRefactoryAstCacheService
	{
		static bool cacheEnabled;
		static Dictionary<string, CompilationUnit> cachedAstInfo = new Dictionary<string, CompilationUnit>();
		static Dictionary<IMember, INode> cachedMemberMappings = new Dictionary<IMember, INode>(new MemberEqualityComparer());
		
		/// <summary>
		/// Gets a flag that indicates whether the AST cache is currently enabled.
		/// </summary>
		public static bool CacheEnabled {
			get {
				return cacheEnabled;
			}
		}
		
		/// <summary>
		/// Occurs when the cache is enabled or disabled.
		/// </summary>
		public static event EventHandler CacheEnabledChanged;
		
		/// <summary>
		/// Raises the CacheEnabledChanged event.
		/// </summary>
		private static void OnCacheEnabledChanged(EventArgs e)
		{
			if (CacheEnabledChanged != null) {
				CacheEnabledChanged(null, e);
			}
		}
		
		/// <summary>
		/// Enables the AST cache.
		/// </summary>
		/// <exception cref="InvalidOperationException">The AST cache is already enabled.</exception>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.InvalidOperationException.#ctor(System.String)")]
		public static void EnableCache()
		{
			if (CacheEnabled) {
				throw new InvalidOperationException("The AST cache is already enabled.");
			}
			cacheEnabled = true;
			LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService cache enabled");
			OnCacheEnabledChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Disables the AST cache.
		/// </summary>
		public static void DisableCache()
		{
			if (CacheEnabled) {
				cacheEnabled = false;
				cachedAstInfo.Clear();
				cachedMemberMappings.Clear();
				LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService cache disabled and cleared");
				OnCacheEnabledChanged(EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// Gets the complete NRefactory AST for the specified file.
		/// </summary>
		/// <param name="language">The language of the file.</param>
		/// <param name="fileName">The file to get the AST for.</param>
		/// <param name="fileContent">The content of the file to get the AST for.</param>
		/// <returns>A <see cref="CompilationUnit"/> that contains the AST for the specified file, or <c>null</c> if the file cannot be parsed.</returns>
		/// <remarks>Between calls to <see cref="EnableCache"/> and <see cref="DisableCache"/> the file is parsed only once. On subsequent accesses the AST is retrieved from the cache.</remarks>
		public static CompilationUnit GetFullAst(SupportedLanguage language, string fileName, string fileContent)
		{
			CompilationUnit cu;
			if (!CacheEnabled || !cachedAstInfo.TryGetValue(fileName, out cu)) {
				cu = Parse(language, fileName, fileContent);
				if (cu != null && CacheEnabled) {
					cachedAstInfo.Add(fileName, cu);
				}
			}
			return cu;
		}
		
		static CompilationUnit Parse(SupportedLanguage language, string fileName, string fileContent)
		{
			using(ICSharpCode.NRefactory.IParser parser = ParserFactory.CreateParser(language, new StringReader(fileContent))) {
				if (parser != null) {
					#if DEBUG
					LoggingService.Debug("ResourceToolkit: NRefactoryAstCacheService: Parsing file '"+fileName+"'");
					#endif
					parser.ParseMethodBodies = true;
					parser.Parse();
					return parser.CompilationUnit;
				}
			}
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Parses an expression with NRefactory.
		/// </summary>
		/// <param name="fileName">The file name of the source code file that contains the expression.</param>
		/// <param name="expression">The expression to parse.</param>
		/// <param name="caretLine">The 1-based line number of the expression.</param>
		/// <param name="caretColumn">The 1-based column number of the expression.</param>
		/// <returns>The parsed expression or <c>null</c> if the expression cannot be parsed or the language of the source code file is not supported.</returns>
		public static Expression ParseExpression(string fileName, string expression, int caretLine, int caretColumn)
		{
			SupportedLanguage? l = NRefactoryResourceResolver.GetFileLanguage(fileName);
			if (l == null) {
				return null;
			}
			
			using (ICSharpCode.NRefactory.IParser p = ICSharpCode.NRefactory.ParserFactory.CreateParser(l.Value, new System.IO.StringReader(expression))) {
				Expression expr = p.ParseExpression();
				if (expr != null) {
					expr.AcceptVisitor(new SetAllNodePointsAstVisitor(new Location(caretColumn, caretLine), new Location(caretColumn + 1, caretLine)), null);
				}
				return expr;
			}
		}
		
		/// <summary>
		/// An AST visitor that sets the start and end location of all visited nodes
		/// to the values passed to the constructor.
		/// </summary>
		sealed class SetAllNodePointsAstVisitor : ICSharpCode.NRefactory.Visitors.NodeTrackingAstVisitor
		{
			readonly Location start, end;
			
			public SetAllNodePointsAstVisitor(Location start, Location end)
			{
				this.start = start;
				this.end = end;
			}
			
			protected override void BeginVisit(INode node)
			{
				node.StartLocation = start;
				node.EndLocation = end;
			}
		}
		
		/// <summary>
		/// Resolves an expression using low-level NRefactoryResolver methods and making
		/// use of the cache if possible.
		/// </summary>
		/// <param name="fileName">The file name of the source code file that contains the expression to be resolved.</param>
		/// <param name="fileContent">The content of the source code file that contains the expression to be resolved.</param>
		/// <param name="caretLine">The 1-based line number of the expression.</param>
		/// <param name="caretColumn">The 1-based column number of the expression.</param>
		/// <param name="compilationUnit">The CompilationUnit that contains the NRefactory AST for this file. May be <c>null</c> (then the CompilationUnit is retrieved from the cache or the file is parsed).</param>
		/// <param name="expression">The expression to be resolved.</param>
		/// <param name="context">The ExpressionContext of the expression.</param>
		/// <returns>A ResolveResult or <c>null</c> if the expression cannot be resolved.</returns>
		public static ResolveResult ResolveLowLevel(string fileName, string fileContent, int caretLine, int caretColumn, CompilationUnit compilationUnit, string expression, ExpressionContext context)
		{
			Expression expr = ParseExpression(fileName, expression, caretLine, caretColumn);
			if (expr == null) return null;
			return ResolveLowLevel(fileName, fileContent, caretLine, caretColumn, compilationUnit, expression, expr, context);
		}
		
		/// <summary>
		/// Resolves an expression using low-level NRefactoryResolver methods and making
		/// use of the cache if possible.
		/// </summary>
		/// <param name="fileName">The file name of the source code file that contains the expression to be resolved.</param>
		/// <param name="fileContent">The content of the source code file that contains the expression to be resolved.</param>
		/// <param name="caretLine">The 1-based line number of the expression.</param>
		/// <param name="caretColumn">The 1-based column number of the expression.</param>
		/// <param name="compilationUnit">The CompilationUnit that contains the NRefactory AST for this file. May be <c>null</c> (then the CompilationUnit is retrieved from the cache or the file is parsed).</param>
		/// <param name="expressionString">The expression to be resolved as a string. If this parameter is <c>null</c>, the expression string is generated by using the code generator.</param>
		/// <param name="expression">The parsed expression to be resolved.</param>
		/// <param name="context">The ExpressionContext of the expression.</param>
		/// <returns>A ResolveResult or <c>null</c> if the expression cannot be resolved.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "4#")]
		public static ResolveResult ResolveLowLevel(string fileName, string fileContent, int caretLine, int caretColumn, CompilationUnit compilationUnit, string expressionString, Expression expression, ExpressionContext context)
		{
			if (fileName == null) {
				throw new ArgumentNullException("fileName");
			}
			if (fileContent == null) {
				throw new ArgumentNullException("fileName");
			}
			if (expression == null) {
				throw new ArgumentNullException("expression");
			}
			
			IProject p = ProjectFileDictionaryService.GetProjectForFile(fileName);
			if (p == null) {
				LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService: ResolveLowLevel failed. Project is null for file '"+fileName+"'");
				return null;
			}
			
			IProjectContent pc = ResourceResolverService.GetProjectContent(p);
			if (pc == null) {
				LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService: ResolveLowLevel failed. ProjectContent is null for project '"+p.ToString()+"'");
				return null;
			}
			
			NRefactoryResolver resolver = ResourceResolverService.CreateResolver(fileName) as NRefactoryResolver;
			if (resolver == null) {
				resolver = new NRefactoryResolver(LanguageProperties.CSharp);
			}
			
			if (compilationUnit == null) {
				compilationUnit = GetFullAst(resolver.Language, fileName, fileContent);
			}
			if (compilationUnit == null) {
				LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService: ResolveLowLevel failed due to the compilation unit being unavailable.");
				return null;
			}
			
			if (!resolver.Initialize(ParserService.GetParseInformation(fileName), caretLine, caretColumn)) {
				LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService: ResolveLowLevel failed. NRefactoryResolver.Initialize returned false.");
				return null;
			}
			
			if (resolver.CallingClass != null) {
				ResolveResult rr;
				if (expressionString == null) {
					// HACK: Re-generate the code for the expression from the expression object by using the code generator.
					// This is necessary when invoking from inside an AST visitor where the
					// code belonging to this expression is unavailable.
					expressionString = resolver.LanguageProperties.CodeGenerator.GenerateCode(expression, String.Empty);
				}
				if ((rr = CtrlSpaceResolveHelper.GetResultFromDeclarationLine(resolver.CallingClass, resolver.CallingMember as IMethodOrProperty, caretLine, caretColumn, new ExpressionResult(expressionString))) != null) {
					return rr;
				}
			}
			
			if (resolver.CallingMember != null) {
				
				// Cache member->node mappings to improve performance
				// (if cache is enabled)
				INode memberNode;
				if (!CacheEnabled || !cachedMemberMappings.TryGetValue(resolver.CallingMember, out memberNode)) {
					MemberFindAstVisitor visitor = new MemberFindAstVisitor(resolver.CallingMember);
					compilationUnit.AcceptVisitor(visitor, null);
					memberNode = visitor.MemberNode;
					if (CacheEnabled && memberNode != null) {
						cachedMemberMappings.Add(resolver.CallingMember, memberNode);
					}
				}
				
				if (memberNode == null) {
					LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService: Could not find member in AST: "+resolver.CallingMember.ToString());
				} else {
					resolver.RunLookupTableVisitor(memberNode);
				}
				
			}
			
			return resolver.ResolveInternal(expression, context);
		}
		
		/// <summary>
		/// Resolves the next outer expression of the specified expression
		/// using low-level NRefactoryResolver methods and making
		/// use of the cache if possible.
		/// </summary>
		/// <param name="expressionResult">The ExpressionResult that contains the expression to be resolved. The contained expression will be set to the next outer expression or <c>null</c> if there is no such expression.</param>
		/// <param name="caretLine">The 0-based line number of the expression.</param>
		/// <param name="caretColumn">The 0-based column number of the expression.</param>
		/// <param name="fileName">The file name of the source code file that contains the expression to be resolved.</param>
		/// <param name="fileContent">The content of the source code file that contains the expression to be resolved.</param>
		/// <param name="expressionFinder">The ExpressionFinder for this source code file.</param>
		/// <returns>A ResolveResult or <c>null</c> if the outer expression cannot be resolved or if the specified expression is the outermost expression.</returns>
		public static ResolveResult ResolveNextOuterExpression(ref ExpressionResult expressionResult, int caretLine, int caretColumn, string fileName, string fileContent, IExpressionFinder expressionFinder)
		{
			if (!String.IsNullOrEmpty(expressionResult.Expression = expressionFinder.RemoveLastPart(expressionResult.Expression))) {
				Expression nextExpression;
				if ((nextExpression = ParseExpression(fileName, expressionResult.Expression, caretLine + 1, caretColumn + 1)) != null) {
					return ResolveLowLevel(fileName, fileContent, caretLine + 1, caretColumn + 1, null, expressionResult.Expression, nextExpression, expressionResult.Context);
				}
			}
			return null;
		}
	}
}
