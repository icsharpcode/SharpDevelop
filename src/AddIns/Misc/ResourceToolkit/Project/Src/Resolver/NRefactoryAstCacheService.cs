// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

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
			cacheEnabled = false;
			cachedAstInfo.Clear();
			cachedMemberMappings.Clear();
			LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService cache disabled and cleared");
			OnCacheEnabledChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Gets the complete NRefactory AST for the specified file.
		/// </summary>
		/// <param name="language">The language of the file.</param>
		/// <param name="fileName">The file to get the AST for.</param>
		/// <returns>A <see cref="CompilationUnit"/> that contains the AST for the specified file, or <c>null</c> if the file cannot be parsed.</returns>
		/// <remarks>Between calls to <see cref="EnableCache"/> and <see cref="DisableCache"/> the file is parsed only once. On subsequent accesses the AST is retrieved from the cache.</remarks>
		public static CompilationUnit GetFullAst(SupportedLanguage language, string fileName)
		{
			CompilationUnit cu;
			if (!CacheEnabled || !cachedAstInfo.TryGetValue(fileName, out cu)) {
				cu = Parse(language, fileName);
				if (cu != null && CacheEnabled) {
					cachedAstInfo.Add(fileName, cu);
				}
			}
			return cu;
		}
		
		static CompilationUnit Parse(SupportedLanguage language, string fileName)
		{
			using(ICSharpCode.NRefactory.IParser parser = ParserFactory.CreateParser(language, new StringReader(ParserService.GetParseableFileContent(fileName)))) {
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
		/// Resolves an expression using low-level NRefactoryResolver methods and making
		/// use of the cache if possible.
		/// </summary>
		/// <param name="fileName">The file name of the source code file that contains the expression to be resolved.</param>
		/// <param name="caretLine">The 1-based line number of the expression.</param>
		/// <param name="caretColumn">The 1-based column number of the expression.</param>
		/// <param name="compilationUnit">The CompilationUnit that contains the NRefactory AST for this file. May be <c>null</c> (then the CompilationUnit is retrieved from the cache or the file is parsed).</param>
		/// <param name="expression">The expression to be resolved.</param>
		/// <param name="context">The ExpressionContext of the expression.</param>
		/// <returns>A ResolveResult or <c>null</c> if the expression cannot be resolved.</returns>
		public static ResolveResult ResolveLowLevel(string fileName, int caretLine, int caretColumn, CompilationUnit compilationUnit, string expression, ExpressionContext context)
		{
			using (ICSharpCode.NRefactory.IParser p = ICSharpCode.NRefactory.ParserFactory.CreateParser(NRefactoryResourceResolver.GetFileLanguage(fileName).Value, new System.IO.StringReader(expression))) {
				Expression expr = p.ParseExpression();
				if (expr == null) {
					return null;
				}
				return ResolveLowLevel(fileName, caretLine, caretColumn, compilationUnit, expression, expr, context);
			}
		}
		
		/// <summary>
		/// Resolves an expression using low-level NRefactoryResolver methods and making
		/// use of the cache if possible.
		/// </summary>
		/// <param name="fileName">The file name of the source code file that contains the expression to be resolved.</param>
		/// <param name="caretLine">The 1-based line number of the expression.</param>
		/// <param name="caretColumn">The 1-based column number of the expression.</param>
		/// <param name="compilationUnit">The CompilationUnit that contains the NRefactory AST for this file. May be <c>null</c> (then the CompilationUnit is retrieved from the cache or the file is parsed).</param>
		/// <param name="expressionString">The expression to be resolved as a string. If this parameter is <c>null</c>, the expression string is generated by using the code generator.</param>
		/// <param name="expression">The parsed expression to be resolved.</param>
		/// <param name="context">The ExpressionContext of the expression.</param>
		/// <returns>A ResolveResult or <c>null</c> if the expression cannot be resolved.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "4#")]
		public static ResolveResult ResolveLowLevel(string fileName, int caretLine, int caretColumn, CompilationUnit compilationUnit, string expressionString, Expression expression, ExpressionContext context)
		{
			if (fileName == null) {
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
			
			IProjectContent pc = ParserService.GetProjectContent(p);
			if (pc == null) {
				LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService: ResolveLowLevel failed. ProjectContent is null for project '"+p.ToString()+"'");
				return null;
			}
			
			NRefactoryResolver resolver = new NRefactoryResolver(pc);
			
			if (compilationUnit == null) {
				compilationUnit = GetFullAst(resolver.Language, fileName);
			}
			if (compilationUnit == null) {
				LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService: ResolveLowLevel failed due to the compilation unit being unavailable.");
				return null;
			}
			
			if (!resolver.Initialize(fileName, caretLine, caretColumn)) {
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
				if ((rr = CtrlSpaceResolveHelper.GetResultFromDeclarationLine(resolver.CallingClass, resolver.CallingMember as IMethodOrProperty, caretLine, caretColumn, expressionString)) != null) {
					return rr;
				}
			}
			
			if (resolver.CallingMember != null) {
				
				// Cache member->node mappings to improves performance
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
	}
}
