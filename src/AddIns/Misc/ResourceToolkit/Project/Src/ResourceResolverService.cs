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
using System.Text;

using Hornung.ResourceToolkit.Resolver;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace Hornung.ResourceToolkit
{
	/// <summary>
	/// Provides facilities to find and resolve expressions referencing resources.
	/// </summary>
	public static class ResourceResolverService
	{
		
		/// <summary>
		/// The AddIn tree path where the resource resolvers are registered.
		/// </summary>
		public const string ResourceResolversAddInTreePath = "/AddIns/ResourceToolkit/Resolvers";
		
		// ********************************************************************************************************************************
		
		static List<IResourceResolver> resolvers;
		
		/// <summary>
		/// Gets a list of all registered resource resolvers.
		/// </summary>
		public static IEnumerable<IResourceResolver> Resolvers {
			get {
				if (resolvers == null) {
					resolvers = AddInTree.BuildItems<IResourceResolver>(ResourceResolversAddInTreePath, null, false);
				}
				return resolvers;
			}
		}
		
		public static void SetResourceResolversListUnitTestOnly(IEnumerable<IResourceResolver> resolversToSet)
		{
			resolvers = new List<IResourceResolver>(resolversToSet);
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Attempts to resolve a reference to a resource using all registered resolvers.
		/// </summary>
		/// <param name="editor">The text editor for which a resource resolution attempt should be performed.</param>
		/// <param name="charTyped">The character that has been typed at the caret position but is not yet in the buffer (this is used when invoked from code completion), or <c>null</c>.</param>
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the caret in the specified editor, or <c>null</c> if all registered resolvers return <c>null</c>.</returns>
		public static ResourceResolveResult Resolve(ITextEditor editor, char? charTyped)
		{
			ResourceResolveResult result;
			foreach (IResourceResolver resolver in Resolvers) {
				if ((result = resolver.Resolve(editor, charTyped)) != null) {
					return result;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Attempts to resolve a reference to a resource using all registered resolvers.
		/// </summary>
		/// <param name="fileName">The name of the file that contains the expression to be resolved.</param>
		/// <param name="document">The document that contains the expression to be resolved.</param>
		/// <param name="caretLine">The 0-based line in the file that contains the expression to be resolved.</param>
		/// <param name="caretColumn">The 0-based column position of the expression to be resolved.</param>
		/// <param name="charTyped">The character that has been typed at the caret position but is not yet in the buffer (this is used when invoked from code completion), or <c>null</c>.</param>
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the caret in the specified editor, or <c>null</c> if all registered resolvers return <c>null</c>.</returns>
		public static ResourceResolveResult Resolve(string fileName, IDocument document, int caretLine, int caretColumn, char? charTyped)
		{
			ResourceResolveResult result;
			foreach (IResourceResolver resolver in Resolvers) {
				if ((result = resolver.Resolve(fileName, document, caretLine, caretColumn, charTyped)) != null) {
					return result;
				}
			}
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Builds the formatted description string for the specified resource.
		/// </summary>
		public static string FormatResourceDescription(IResourceFileContent content, string key)
		{
			StringBuilder sb = new StringBuilder();
			
			IMultiResourceFileContent mrfc;
			if (key != null && (mrfc = (content as IMultiResourceFileContent)) != null) {
				string file = mrfc.GetFileNameForKey(key);
				if (file == null) {
					file = content.FileName;
				}
				sb.AppendFormat(StringParser.Parse("${res:Hornung.ResourceToolkit.ToolTips.PlaceMessage}"), file);
			} else {
				sb.AppendFormat(StringParser.Parse("${res:Hornung.ResourceToolkit.ToolTips.PlaceMessage}"), content.FileName);
			}
			
			sb.AppendLine();
			sb.Append(StringParser.Parse("${res:Hornung.ResourceToolkit.KeyLabel}"));
			sb.Append(' ');
			
			if (key != null) {
				
				sb.AppendLine(key);
				sb.AppendLine();
				sb.AppendLine(StringParser.Parse("${res:Hornung.ResourceToolkit.ValueLabel}"));
				
				object value;
				if (content.TryGetValue(key, out value)) {
					if (value is string) {
						sb.Append(value);
					} else {
						sb.AppendFormat(StringParser.Parse("${res:Hornung.ResourceToolkit.ToolTips.TypeMessage}"), value.GetType().ToString());
						sb.Append(' ');
						sb.Append(value.ToString());
					}
				} else {
					sb.Append(StringParser.Parse("${res:Hornung.ResourceToolkit.ToolTips.KeyNotFound}"));
				}
				
			} else {
				sb.Append(StringParser.Parse("${res:Hornung.ResourceToolkit.ToolTips.UnknownKey}"));
			}
			
			return sb.ToString();
		}
		
		// ********************************************************************************************************************************
		
		// The following helper methods are needed to support running
		// in the unit testing mode where the addin tree is not available.
		
		static Dictionary<string, IParser> presetParsersUnitTestOnly;
		
		public static void SetParsersUnitTestOnly(Dictionary<string, IParser> parsers)
		{
			presetParsersUnitTestOnly = parsers;
		}
		
		public static IParser GetParser(string fileName)
		{
			IParser p;
			if (presetParsersUnitTestOnly == null) {
				p = ParserService.CreateParser(fileName);
			} else {
				presetParsersUnitTestOnly.TryGetValue(System.IO.Path.GetExtension(fileName), out p);
			}
			return p;
		}
		
		public static IExpressionFinder GetExpressionFinder(string fileName)
		{
			IParser p = GetParser(fileName);
			if (p == null) return null;
			return p.CreateExpressionFinder(fileName);
		}
		
		public static IResolver CreateResolver(string fileName)
		{
			IParser p = GetParser(fileName);
			if (p == null) return null;
			return p.CreateResolver();
		}
		
		static Dictionary<string, string> fileContents;
		
		public static void SetFileContentUnitTestOnly(string fileName, string fileContent)
		{
			if (fileContents == null) {
				fileContents = new Dictionary<string, string>();
			}
			fileContents[fileName] = fileContent;
		}
		
		public static string GetParsableFileContent(string fileName)
		{
			if (fileContents == null) {
				return ParserService.GetParseableFileContent(fileName).Text;
			} else {
				return fileContents[fileName];
			}
		}
		
		static Dictionary<IProject, IProjectContent> projectContents;
		
		public static void SetProjectContentUnitTestOnly(IProject project, IProjectContent projectContent)
		{
			if (projectContents == null) {
				projectContents = new Dictionary<IProject, IProjectContent>();
			}
			projectContents[project] = projectContent;
		}
		
		public static IProjectContent GetProjectContent(IProject project)
		{
			if (projectContents == null) {
				return ParserService.GetProjectContent(project);
			} else {
				return projectContents[project];
			}
		}
	}
}
