// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.SharpDevelop;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Parses files using NRefactory and caches the AST on demand.
	/// </summary>
	public static class NRefactoryAstCacheService
	{
		static bool cacheEnabled = false;
		static Dictionary<string, CompilationUnit> cachedAstInfo = new Dictionary<string, CompilationUnit>();
		
		/// <summary>
		/// Gets a flag that indicates whether the AST cache is currently enabled.
		/// </summary>
		public static bool CacheEnabled {
			get {
				return cacheEnabled;
			}
		}
		
		/// <summary>
		/// Enables the AST cache.
		/// </summary>
		/// <exception cref="InvalidOperationException">The AST cache is already enabled.</exception>
		public static void EnableCache()
		{
			if (CacheEnabled) {
				throw new InvalidOperationException("The AST cache is already enabled.");
			}
			cacheEnabled = true;
			LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService cache enabled");
		}
		
		/// <summary>
		/// Disables the AST cache.
		/// </summary>
		public static void DisableCache()
		{
			cacheEnabled = false;
			cachedAstInfo.Clear();
			LoggingService.Info("ResourceToolkit: NRefactoryAstCacheService cache disabled and cleared");
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
	}
}
