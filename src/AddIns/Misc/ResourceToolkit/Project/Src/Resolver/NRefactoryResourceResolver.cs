// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Resolves resource references using NRefactory.
	/// </summary>
	public class NRefactoryResourceResolver : AbstractResourceResolver
	{
		
		/// <summary>
		/// The AddIn tree path where the NRefactory resource resolvers are registered.
		/// </summary>
		public const string NRefactoryResourceResolversAddInTreePath = "/AddIns/ResourceToolkit/NRefactoryResourceResolver/Resolvers";
		
		// ********************************************************************************************************************************
		
		static List<INRefactoryResourceResolver> resolvers;
		
		/// <summary>
		/// Gets a list of all registered NRefactory resource resolvers.
		/// </summary>
		public static IEnumerable<INRefactoryResourceResolver> Resolvers {
			get {
				if (resolvers == null) {
					resolvers = AddInTree.BuildItems<INRefactoryResourceResolver>(NRefactoryResourceResolversAddInTreePath, null, false);
				}
				return resolvers;
			}
		}
		
		public static void SetResourceResolversListUnitTestOnly(IEnumerable<INRefactoryResourceResolver> resolversToSet)
		{
			resolvers = new List<INRefactoryResourceResolver>(resolversToSet);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="NRefactoryResourceResolver"/> class.
		/// </summary>
		public NRefactoryResourceResolver() : base()
		{
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Determines whether this resolver supports resolving resources in the given file.
		/// </summary>
		/// <param name="fileName">The name of the file to examine.</param>
		/// <returns><c>true</c>, if this resolver supports resolving resources in the given file, <c>false</c> otherwise.</returns>
		public override bool SupportsFile(string fileName)
		{
			// Any source code file supported by NRefactory is supported
			return (GetFileLanguage(fileName) != null);
		}
		
		/// <summary>
		/// Gets a list of patterns that can be searched for in the specified file
		/// to find possible resource references that are supported by this
		/// resolver.
		/// </summary>
		/// <param name="fileName">The name of the file to get a list of possible patterns for.</param>
		public override IEnumerable<string> GetPossiblePatternsForFile(string fileName)
		{
			if (this.SupportsFile(fileName)) {
				List<string> patterns = new List<string>();
				foreach (INRefactoryResourceResolver resolver in Resolvers) {
					foreach (string pattern in resolver.GetPossiblePatternsForFile(fileName)) {
						if (!patterns.Contains(pattern)) {
							patterns.Add(pattern);
						}
					}
				}
				return patterns;
			}
			return new string[0];
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Attempts to resolve a reference to a resource.
		/// </summary>
		/// <param name="fileName">The name of the file that contains the expression to be resolved.</param>
		/// <param name="document">The document that contains the expression to be resolved.</param>
		/// <param name="caretLine">The 0-based line in the file that contains the expression to be resolved.</param>
		/// <param name="caretColumn">The 0-based column position of the expression to be resolved.</param>
		/// <param name="caretOffset">The offset of the position of the expression to be resolved.</param>
		/// <param name="charTyped">The character that has been typed at the caret position but is not yet in the buffer (this is used when invoked from code completion), or <c>null</c>.</param>
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the specified position in the specified file, or <c>null</c> if that expression does not reference a (known) resource.</returns>
		protected override ResourceResolveResult Resolve(string fileName, IDocument document, int caretLine, int caretColumn, int caretOffset, char? charTyped)
		{
			IExpressionFinder ef = ResourceResolverService.GetExpressionFinder(fileName);
			if (ef == null) {
				return null;
			}
			
			bool foundStringLiteral = false;
			
			while (true) {
				
				ExpressionResult result = ef.FindFullExpression(document.Text, caretOffset);
				
				if (result.Expression == null) {
					// Try to find an expression to the left, but only
					// in the same line.
					if (foundStringLiteral || --caretOffset < 0)
						return null;
					var line = document.GetLineForOffset(caretOffset);
					if (line.LineNumber - 1 != caretLine)
						return null;
					continue;
				}
				
				if (!result.Region.IsEmpty) {
					caretLine = result.Region.BeginLine - 1;
					caretColumn = result.Region.BeginColumn - 1;
				}
				
				PrimitiveExpression pe;
				Expression expr = NRefactoryAstCacheService.ParseExpression(fileName, result.Expression, caretLine + 1, caretColumn + 1);
				
				if (expr == null) {
					return null;
				} else if ((pe = expr as PrimitiveExpression) != null) {
					if (pe.Value is string) {
						
						if (foundStringLiteral) {
							return null;
						}
						
						// We are inside a string literal and need to find
						// the next outer expression to decide
						// whether it is a resource key.
						
						if (!result.Region.IsEmpty) {
							// Go back to the start of the string literal - 2.
							caretOffset = document.PositionToOffset(result.Region.BeginLine, result.Region.BeginColumn) - 2;
							if (caretOffset < 0) return null;
						} else {
							LoggingService.Debug("ResourceToolkit: NRefactoryResourceResolver: Found string literal, but result region is empty. Trying to infer position from text.");
							int newCaretOffset = document.GetText(0, Math.Min(document.TextLength, caretOffset + result.Expression.Length)).LastIndexOf(result.Expression);
							if (newCaretOffset == -1) {
								LoggingService.Warn("ResourceToolkit: NRefactoryResourceResolver: Could not find resolved expression in text.");
								--caretOffset;
								continue;
							} else {
								caretOffset = newCaretOffset;
							}
						}
						
						foundStringLiteral = true;
						continue;
						
					} else {
						return null;
					}
				}
				
				return TryResolve(result, expr, caretLine, caretColumn, fileName, document.Text, ef, charTyped);
				
			}
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Tries to resolve the resource reference using all available
		/// NRefactory resource resolvers.
		/// </summary>
		static ResourceResolveResult TryResolve(ExpressionResult result, Expression expr, int caretLine, int caretColumn, string fileName, string fileContent, IExpressionFinder expressionFinder, char? charTyped)
		{
			ResolveResult rr = NRefactoryAstCacheService.ResolveLowLevel(fileName, fileContent, caretLine+1, caretColumn+1, null, result.Expression, expr, result.Context);
			if (rr != null) {
				
				ResourceResolveResult rrr;
				foreach (INRefactoryResourceResolver resolver in Resolvers) {
					if ((rrr = resolver.Resolve(result, expr, rr, caretLine, caretColumn, fileName, fileContent, expressionFinder, charTyped)) != null) {
						return rrr;
					}
				}
				
			}
			
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Determines the file which contains the resources referenced by the specified manifest resource name.
		/// </summary>
		/// <param name="sourceFileName">The name of the source code file which the reference occurs in.</param>
		/// <param name="resourceName">The manifest resource name to find the resource file for.</param>
		/// <returns>A <see cref="ResourceSetReference"/> with the specified resource set name and the name of the file that contains the resources with the specified manifest resource name, or <c>null</c> if the file name cannot be determined.</returns>
		/// <exception cref="ArgumentNullException">The <paramref name="resourceName"/> parameter is <c>null</c>.</exception>
		public static ResourceSetReference GetResourceSetReference(string sourceFileName, string resourceName)
		{
			if (resourceName == null) {
				throw new ArgumentNullException("resourceName");
			}
			
			IProject p = ProjectFileDictionaryService.GetProjectForFile(sourceFileName);
			
			if (p != null) {
				
				string fileName;
				
				if ((fileName = TryGetResourceFileNameFromProjectDirect(resourceName, p)) != null) {
					return new ResourceSetReference(resourceName, fileName);
				}
				
				// SharpDevelop silently strips the (hard-coded) folder names
				// "src" and "source" when generating the default namespace name
				// for new files.
				// When MSBuild generates the manifest resource names for the
				// forms designer resources, it uses the type name of the
				// first class in the file. So we should find all files
				// that contain a type with the name in resourceName
				// and then look for dependent resource files or resource files
				// with the same name in the same directory as the source files.
				
				// Find all source files that contain a type with the same
				// name as the resource we are looking for.
				List<string> possibleSourceFiles = new List<string>();
				IProjectContent pc = ResourceResolverService.GetProjectContent(p);
				if (pc != null) {
					
					IClass resourceClass = pc.GetClass(resourceName, 0);
					
					if (resourceClass != null) {
						CompoundClass cc = resourceClass.GetCompoundClass() as CompoundClass;
						
						foreach (IClass c in (cc == null ? (IList<IClass>)new IClass[] { resourceClass } : cc.Parts)) {
							if (c.CompilationUnit != null && c.CompilationUnit.FileName != null) {
								
								#if DEBUG
								LoggingService.Debug("ResourceToolkit: NRefactoryResourceResolver found file '"+c.CompilationUnit.FileName+"' to contain the type '"+resourceName+"'");
								#endif
								
								possibleSourceFiles.Add(c.CompilationUnit.FileName);
								
							}
						}
						
					}
					
				}
				
				foreach (string possibleSourceFile in possibleSourceFiles) {
					string possibleSourceFileName = Path.GetFileName(possibleSourceFile);
					
					// Find resource files dependent on these source files.
					foreach (ProjectItem pi in p.Items) {
						FileProjectItem fpi = pi as FileProjectItem;
						if (fpi != null) {
							if (fpi.DependentUpon != null &&
							    (fpi.ItemType == ItemType.EmbeddedResource || fpi.ItemType == ItemType.Resource || fpi.ItemType == ItemType.None) &&
							    FileUtility.IsEqualFileName(fpi.DependentUpon, possibleSourceFileName)) {
								
								#if DEBUG
								LoggingService.Debug("ResourceToolkit: NRefactoryResourceResolver trying to use dependent file '"+fpi.FileName+"' as resource file");
								#endif
								
								if ((fileName = FindResourceFileName(fpi.FileName)) != null) {
									// Prefer culture-invariant resource file
									// over localized resource file
									IResourceFileContent rfc = ResourceFileContentRegistry.GetResourceFileContent(fileName);
									if (rfc.Culture.Equals(CultureInfo.InvariantCulture)) {
										return new ResourceSetReference(resourceName, fileName);
									}
								}
								
							}
						}
					}
					
					// Fall back to any found resource file
					// if no culture-invariant resource file was found
					if (fileName != null) {
						return new ResourceSetReference(resourceName, fileName);
					}
					
					// Find resource files with the same name as the source file
					// and in the same directory.
					if ((fileName = FindResourceFileName(possibleSourceFile)) != null) {
						return new ResourceSetReference(resourceName, fileName);
					}
					
				}
				
			} else {
				
				#if DEBUG
				LoggingService.Info("ResourceToolkit: NRefactoryResourceResolver.GetResourceSetReference could not determine the project for the source file '"+(sourceFileName ?? "<null>")+"'.");
				#endif
				
				if (sourceFileName != null) {
					
					// The project could not be determined.
					// Try a simple file search.
					
					string directory = Path.GetDirectoryName(sourceFileName);
					string resourcePart = resourceName;
					string fileName;
					
					while (true) {
						
						#if DEBUG
						LoggingService.Debug("ResourceToolkit: NRefactoryResourceResolver.GetResourceSetReference: looking for a resource file like '"+Path.Combine(directory, resourcePart)+"'");
						#endif
						
						if ((fileName = FindResourceFileName(Path.Combine(directory, resourcePart.Replace('.', Path.DirectorySeparatorChar)))) != null) {
							return new ResourceSetReference(resourceName, fileName);
						}
						if ((fileName = FindResourceFileName(Path.Combine(directory, resourcePart))) != null) {
							return new ResourceSetReference(resourceName, fileName);
						}
						
						if (resourcePart.Contains(".")) {
							resourcePart = resourcePart.Substring(resourcePart.IndexOf('.')+1);
						} else {
							break;
						}
						
					}
					
				}
				
			}
			
			#if DEBUG
			LoggingService.Info("ResourceToolkit: NRefactoryResourceResolver.GetResourceSetReference is unable to find a suitable resource file for '"+resourceName+"'");
			#endif
			
			return new ResourceSetReference(resourceName, null);
		}
		
		/// <summary>
		/// Tries to find a resource file name for the manifest resources specified by
		/// <paramref name="resourceName"/> according to the default MSBuild rules
		/// for embedding resources.
		/// The reference to these resources occurs within the project <paramref name="p"/>.
		/// </summary>
		/// <param name="resourceName">The name of the manifest resources to find the resource file for.</param>
		/// <param name="p">The project where the reference to these resources occurs.</param>
		/// <returns>The full path and name of the resource file that contains the specified resources, or <c>null</c> if no suitable resource file is found.</returns>
		static string TryGetResourceFileNameFromProjectDirect(string resourceName, IProject p)
		{
			if (!resourceName.StartsWith(p.RootNamespace + ".", StringComparison.OrdinalIgnoreCase)) {
				return null;
			}
			
			// Strip root namespace + "." from resourceName
			resourceName = resourceName.Substring(p.RootNamespace.Length+1);
			
			switch(p.Language) {
					
				case "VBNet":
					
					// SD2-1239
					// The VB MSBuild tasks do not use the folder names
					// in the manifest resource names.
					// We have to look in all folders of the project
					// for a file with the specified resource name.
					
					foreach (ProjectItem item in p.Items) {
						
						FileProjectItem fpi = item as FileProjectItem;
						if (fpi == null) continue;
						
						string virtualName = fpi.VirtualName;
						if (String.IsNullOrEmpty(virtualName)) continue;
						
						if (Path.GetFileNameWithoutExtension(virtualName).Equals(resourceName, StringComparison.OrdinalIgnoreCase) &&
						    File.Exists(fpi.FileName) &&
						    ResourceFileContentRegistry.GetResourceFileContentFactory(fpi.FileName) != null) {
							
							return fpi.FileName;
							
						}
						
					}
					
					break;
					
					
				case "C#":
				default:
					
					// The C# MSBuild tasks use the folder names in the
					// manifest resource names.
					// For example:
					// RootNamespace.Resources.SomeResources
					// is normally found in
					// <ProjectRootDir>\Resources\SomeResources.{resx|resources}
					
					foreach (ProjectItem item in p.Items) {
						
						FileProjectItem fpi = item as FileProjectItem;
						if (fpi == null) continue;
						
						string virtualName = FileUtility.NormalizePath(fpi.VirtualName);
						if (String.IsNullOrEmpty(virtualName)) continue;
						
						int lastDotIndex = virtualName.LastIndexOf('.');
						if (lastDotIndex == -1) continue;
						
						if (virtualName.Substring(0, lastDotIndex).Replace('\\', '.').Equals(resourceName, StringComparison.OrdinalIgnoreCase) &&
						    File.Exists(fpi.FileName) &&
						    ResourceFileContentRegistry.GetResourceFileContentFactory(fpi.FileName) != null) {
							
							return fpi.FileName;
							
						}
						
					}
					
					break;
					
			}
			
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Gets the NRefactory language for the specified file name.
		/// </summary>
		public static SupportedLanguage? GetFileLanguage(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			if (ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
				return SupportedLanguage.CSharp;
			if (ext.Equals(".vb", StringComparison.OrdinalIgnoreCase))
				return SupportedLanguage.VBNet;
			return null;
		}
		
		/// <summary>
		/// Gets the language properties for the project the specified member
		/// belongs to.
		/// Returns <c>null</c> if the language cannot be determined.
		/// </summary>
		public static LanguageProperties GetLanguagePropertiesForMember(IMember member)
		{
			if (member == null) {
				return null;
			}
			if (member.DeclaringType == null) {
				return null;
			}
			if (member.DeclaringType.CompilationUnit == null) {
				return null;
			}
			if (member.DeclaringType.CompilationUnit.ProjectContent == null) {
				return null;
			}
			return member.DeclaringType.CompilationUnit.ProjectContent.Language;
		}
		
		/// <summary>
		/// Gets the language properties for the specified file.
		/// </summary>
		/// <param name="fileName">The file to get the language properties for.</param>
		/// <returns>The language properties of the specified file, or <c>null</c> if the language cannot be determined.</returns>
		public static LanguageProperties GetLanguagePropertiesForFile(string fileName)
		{
			var p = ResourceResolverService.GetParser(fileName);
			if (p == null) {
				return null;
			}
			return p.Language;
		}
		
	}
}
