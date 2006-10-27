// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Text;

using Hornung.ResourceToolkit.Resolver;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

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
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Attempts to resolve a reference to a resource using all registered resolvers.
		/// </summary>
		/// <param name="editor">The text editor for which a resource resolution attempt should be performed.</param>
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the caret in the specified editor, or <c>null</c> if all registered resolvers return <c>null</c>.</returns>
		public static ResourceResolveResult Resolve(TextEditorControl editor)
		{
			ResourceResolveResult result;
			foreach (IResourceResolver resolver in Resolvers) {
				if ((result = resolver.Resolve(editor)) != null) {
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
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the caret in the specified editor, or <c>null</c> if all registered resolvers return <c>null</c>.</returns>
		public static ResourceResolveResult Resolve(string fileName, IDocument document, int caretLine, int caretColumn)
		{
			ResourceResolveResult result;
			foreach (IResourceResolver resolver in Resolvers) {
				if ((result = resolver.Resolve(fileName, document, caretLine, caretColumn)) != null) {
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
		
	}
}
