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
using System.IO;
using System.Linq;

using Hornung.ResourceToolkit.Resolver;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace Hornung.ResourceToolkit.Refactoring
{
	/// <summary>
	/// Provides facilities for refactoring resources.
	/// </summary>
	public static class ResourceRefactoringService
	{
		
		/// <summary>
		/// Finds all references to the specified resource (except the definition).
		/// </summary>
		/// <param name="resourceFileName">The name of the resource file that contains the resource key to find.</param>
		/// <param name="key">The resource key to find.</param>
		/// <param name="monitor">An object implementing <see cref="IProgressMonitor"/> to report the progress of the operation. Can be <c>null</c>.</param>
		/// <returns>A list of references to this resource.</returns>
		public static List<Reference> FindReferences(string resourceFileName, string key, IProgressMonitor monitor)
		{
			return FindReferences(new SpecificResourceReferenceFinder(resourceFileName, key), monitor, SearchScope.WholeSolution);
		}
		
		/// <summary>
		/// Finds all references to resources (except the definition) using the specified
		/// <see cref="IResourceReferenceFinder"/> object.
		/// </summary>
		/// <param name="finder">The <see cref="IResourceReferenceFinder"/> to use to find resource references.</param>
		/// <param name="monitor">An object implementing <see cref="IProgressMonitor"/> to report the progress of the operation. Can be <c>null</c>.</param>
		/// <param name="scope">The scope which should be searched.</param>
		/// <returns>A list of references to resources.</returns>
		public static List<Reference> FindReferences(IResourceReferenceFinder finder, IProgressMonitor monitor, SearchScope scope)
		{
			if (finder == null) {
				throw new ArgumentNullException("finder");
			}
			
			if (ParserService.LoadSolutionProjectsThreadRunning) {
				if (monitor != null) monitor.ShowingDialog = true;
				MessageService.ShowMessage("${res:SharpDevelop.Refactoring.LoadSolutionProjectsThreadRunning}");
				if (monitor != null) monitor.ShowingDialog = false;
				return null;
			}
			
			DateTime startTime = DateTime.UtcNow;
			
			List<Reference> references = new List<Reference>();
			
			try {
				
				NRefactoryAstCacheService.EnableCache();
				
				ICollection<string> files = GetPossibleFiles(scope);
				
				if (monitor != null) {
					monitor.TaskName = StringParser.Parse("${res:SharpDevelop.Refactoring.FindingReferences}");
				}
				double workDone = 0;
				foreach (string fileName in files) {
					if (monitor != null)
						monitor.Progress = workDone / files.Count;
					workDone += 1;
					if (monitor != null && monitor.CancellationToken.IsCancellationRequested) {
						return null;
					}
					
					IDocument doc = null;
					try {
						// The following line throws an exception if the file does not exist.
						// But the file may be in an unsaved view content (which would be found by GetDocumentInformation),
						// so we cannot simply loop on !File.Exists(...).
						doc = FindReferencesAndRenameHelper.GetDocumentInformation(fileName).Document;
					} catch (FileNotFoundException) {
					}
					if (doc == null) {
						continue;
					}
					
					string fileContent = doc.Text;
					if (String.IsNullOrEmpty(fileContent)) {
						continue;
					}
					
					int pos = -1;
					while ((pos = finder.GetNextPossibleOffset(fileName, fileContent, pos)) >= 0) {
						
						Location docPos = doc.OffsetToPosition(pos);
						ResourceResolveResult rrr = ResourceResolverService.Resolve(fileName, doc, docPos.Y - 1, docPos.X - 1, null);
						
						if (rrr != null && rrr.ResourceFileContent != null) {
							if (finder.IsReferenceToResource(rrr)) {
								
								if (rrr.Key != null) {
									
									// The actual location of the key string may be after 'pos' because
									// the resolvers may find an expression just before it.
									string keyString = rrr.Key;
									int keyPos = fileContent.IndexOf(keyString, pos, StringComparison.OrdinalIgnoreCase);
									
									if (keyPos < pos) {
										// The key may be escaped in some way in the document.
										// Try using the code generator to find this out.
										keyPos = FindStringLiteral(fileName, fileContent, rrr.Key, pos, out keyString);
									}
									
									if (keyPos < pos) {
										if (monitor != null) monitor.ShowingDialog = true;
										MessageService.ShowWarning("ResourceToolkit: The key '"+rrr.Key+"' could not be located at the resolved position in the file '"+fileName+"'.");
										if (monitor != null) monitor.ShowingDialog = false;
									} else {
										references.Add(new Reference(fileName, keyPos, keyString.Length, keyString, rrr));
									}
									
								} else {
									references.Add(new Reference(fileName, pos, 0, null, rrr));
								}
								
							}
						}
						
					}
				}
				
				LoggingService.Info("ResourceToolkit: FindReferences finished in "+(DateTime.UtcNow - startTime).TotalSeconds.ToString(System.Globalization.CultureInfo.CurrentCulture)+"s");
				
			} finally {
				NRefactoryAstCacheService.DisableCache();
			}
			
			return references;
		}
		
		/// <summary>
		/// Finds all references to resources (except the definitions).
		/// </summary>
		/// <param name="monitor">An object implementing <see cref="IProgressMonitor"/> to report the progress of the operation. Can be <c>null</c>.</param>
		/// <param name="scope">The scope which should be searched.</param>
		/// <returns>A list of references to resources.</returns>
		public static List<Reference> FindAllReferences(IProgressMonitor monitor, SearchScope scope)
		{
			return FindReferences(new AnyResourceReferenceFinder(), monitor, scope);
		}
		
		/// <summary>
		/// Finds all references to missing resource keys.
		/// </summary>
		/// <param name="monitor">An object implementing <see cref="IProgressMonitor"/> to report the progress of the operation. Can be <c>null</c>.</param>
		/// <param name="scope">The scope which should be searched.</param>
		/// <returns>A list of all references to missing resource keys.</returns>
		public static List<Reference> FindReferencesToMissingKeys(IProgressMonitor monitor, SearchScope scope)
		{
			List<Reference> references = FindAllReferences(monitor, scope);
			if (references == null) {
				return null;
			}
			return references.FindAll(IsReferenceToMissingKey);
		}
		
		/// <summary>
		/// Determines whether the specified reference is a resource reference
		/// to a missing key.
		/// </summary>
		/// <param name="reference">The reference to examine.</param>
		/// <returns><c>true</c>, if the specified reference is a resource reference to a missing key, otherwise <c>false</c>.</returns>
		public static bool IsReferenceToMissingKey(Reference reference)
		{
			ResourceResolveResult rrr = reference.ResolveResult as ResourceResolveResult;
			if (rrr == null || rrr.Key == null) {
				return false;
			}
			if (rrr.ResourceFileContent == null) {
				return true;
			}
			return !rrr.ResourceFileContent.ContainsKey(rrr.Key);
		}
		
		/// <summary>
		/// Finds all unused resource keys in all resource files that are referenced
		/// in code at least once in the whole solution.
		/// </summary>
		/// <param name="monitor">An object implementing <see cref="IProgressMonitor"/> to report the progress of the operation. Can be <c>null</c>.</param>
		/// <returns>A collection of <see cref="ResourceItem"/> classes that represent the unused resource keys.</returns>
		public static ICollection<ResourceItem> FindUnusedKeys(IProgressMonitor monitor)
		{
			List<Reference> references = FindAllReferences(monitor, SearchScope.WholeSolution);
			if (references == null) {
				return null;
			}
			
			List<ResourceItem> unused = new List<ResourceItem>();
			
			// Get a list of all referenced resource files.
			// Generate a dictonary of resource file names and the
			// corresponding referenced keys.
			Dictionary<string, List<string>> referencedKeys = new Dictionary<string, List<string>>();
			Dictionary<string, List<string>> referencedPrefixes = new Dictionary<string, List<string>>();
			foreach (Reference reference in references) {
				ResourceResolveResult rrr = (ResourceResolveResult)reference.ResolveResult;
				if (rrr.ResourceFileContent != null) {
					string fileName = rrr.FileName;
					if (!referencedKeys.ContainsKey(fileName)) {
						referencedKeys.Add(fileName, new List<string>());
						referencedPrefixes.Add(fileName, new List<string>());
					}
					if (rrr.Key != null && !referencedKeys[fileName].Contains(rrr.Key)) {
						referencedKeys[fileName].Add(rrr.Key);
					} else {
						ResourcePrefixResolveResult rprr = rrr as ResourcePrefixResolveResult;
						if (rprr != null && rprr.Prefix != null && !referencedPrefixes[fileName].Contains(rprr.Prefix)) {
							referencedPrefixes[fileName].Add(rprr.Prefix);
						}
					}
				} else {
					if (monitor != null) monitor.ShowingDialog = true;
					MessageService.ShowWarning("Found a resource reference that could not be resolved."+Environment.NewLine+(reference.FileName ?? "<null>")+":"+reference.Offset+Environment.NewLine+"Expression: "+(reference.Expression ?? "<null>"));
					if (monitor != null) monitor.ShowingDialog = false;
				}
			}
			
			// Find keys that are not referenced anywhere.
			foreach (string fileName in referencedKeys.Keys) {
				#if DEBUG
				LoggingService.Debug("ResourceToolkit: FindUnusedKeys: Referenced resource file '"+fileName+"'");
				#endif
				foreach (KeyValuePair<string, object> entry in ResourceFileContentRegistry.GetResourceFileContent(fileName).Data) {
					if (!referencedKeys[fileName].Contains(entry.Key) &&
					    !referencedPrefixes[fileName].Any(prefix => entry.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))) {
						unused.Add(new ResourceItem(fileName, entry.Key));
					}
				}
			}
			
			return unused.AsReadOnly();
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Renames all references to a resource including the definition.
		/// Asks the user for a new name and shows a progress dialog during
		/// the operation.
		/// </summary>
		/// <param name="rrr">The resource to be renamed.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "ICSharpCode.Core.MessageService.ShowInputBox(System.String,System.String,System.String)")]
		public static void Rename(ResourceResolveResult rrr)
		{
			string newKey = MessageService.ShowInputBox("${res:SharpDevelop.Refactoring.Rename}", "${res:Hornung.ResourceToolkit.RenameResourceText}", rrr.Key);
			if (!String.IsNullOrEmpty(newKey) && !newKey.Equals(rrr.Key)) {
				using(AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog("${res:SharpDevelop.Refactoring.Rename}")) {
					Rename(rrr, newKey, monitor);
				}
			}
		}
		
		/// <summary>
		/// Renames all references to a resource including the definition.
		/// </summary>
		/// <param name="rrr">The resource to be renamed.</param>
		/// <param name="newKey">The new name of the resource key.</param>
		/// <param name="monitor">An object implementing <see cref="IProgressMonitor"/> to report the progress of the operation. Can be <c>null</c>.</param>
		public static void Rename(ResourceResolveResult rrr, string newKey, IProgressMonitor monitor)
		{
			// Prevent duplicate key names
			if (rrr.ResourceFileContent.ContainsKey(newKey)) {
				if (monitor != null) monitor.ShowingDialog = true;
				MessageService.ShowWarning("${res:Hornung.ResourceToolkit.EditStringResourceDialog.DuplicateKey}");
				if (monitor != null) monitor.ShowingDialog = false;
				return;
			}
			
			List<Reference> references = FindReferences(rrr.FileName, rrr.Key, monitor);
			if (references == null) {
				return;
			}
			
			try {
				// rename definition (if present)
				if (rrr.ResourceFileContent.ContainsKey(rrr.Key)) {
					rrr.ResourceFileContent.RenameKey(rrr.Key, newKey);
				} else {
					if (monitor != null) monitor.ShowingDialog = true;
					MessageService.ShowWarning("${res:Hornung.ResourceToolkit.RenameKeyDefinitionNotFoundWarning}");
					if (monitor != null) monitor.ShowingDialog = false;
				}
			} catch (Exception ex) {
				if (monitor != null) monitor.ShowingDialog = true;
				MessageService.ShowWarningFormatted("${res:Hornung.ResourceToolkit.ErrorProcessingResourceFile}" + Environment.NewLine + ex.Message, rrr.ResourceFileContent.FileName);
				if (monitor != null) monitor.ShowingDialog = false;
				// Do not rename the references when renaming the definition failed.
				return;
			}
			
			// rename references
			// FIXME: RenameReferences does not enforce escaping rules. May be a problem if someone uses double-quotes in the new resource key name.
			FindReferencesAndRenameHelper.RenameReferences(references, newKey);
			
			// rename definitions in localized resource files
			foreach (KeyValuePair<string, IResourceFileContent> entry in ResourceFileContentRegistry.GetLocalizedContents(rrr.FileName)) {
				try {
					if (entry.Value.ContainsKey(rrr.Key)) {
						entry.Value.RenameKey(rrr.Key, newKey);
					}
				} catch (Exception ex) {
					if (monitor != null) monitor.ShowingDialog = true;
					MessageService.ShowWarningFormatted("${res:Hornung.ResourceToolkit.ErrorProcessingResourceFile}" + Environment.NewLine + ex.Message, entry.Value.FileName);
					if (monitor != null) monitor.ShowingDialog = false;
				}
			}
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Gets a list of names of files which can possibly contain resource references.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static ICollection<string> GetPossibleFiles(SearchScope scope)
		{
			List<string> files = new List<string>();
			
			switch(scope) {
				case SearchScope.WholeSolution:
					Solution s = ProjectService.OpenSolution;
					if (s == null) {
						throw new InvalidOperationException("Cannot search in whole solution when no solution is open.");
					}
					AddFilesFromSolution(files, s);
					break;
					
				case SearchScope.CurrentProject:
					IProject p = ProjectService.CurrentProject;
					if (p == null) {
						throw new InvalidOperationException("Cannot search in current project when no project is active.");
					}
					AddFilesFromProject(files, p);
					break;
					
				case SearchScope.CurrentFile:
					IViewContent vc = WorkbenchSingleton.Workbench.ActiveViewContent;
					if (vc == null) {
						throw new InvalidOperationException("Cannot search in current file when no file is open.");
					}
					AddFilesFromViewContent(files, vc);
					break;
					
				case SearchScope.OpenFiles:
					foreach (IViewContent v in WorkbenchSingleton.Workbench.ViewContentCollection) {
						AddFilesFromViewContent(files, v);
					}
					break;
					
				default:
					throw new ArgumentOutOfRangeException("scope", "The scope parameter is not set to one of the SearchScope values.");
			}
			
			return files.AsReadOnly();
		}
		
		static void AddFilesFromSolution(IList<string> files, Solution s)
		{
			foreach (IProject p in s.Projects) {
				AddFilesFromProject(files, p);
			}
		}
		
		static void AddFilesFromProject(IList<string> files, IProject p)
		{
			foreach (ProjectItem pi in p.Items) {
				if (pi is FileProjectItem) {
					string name = pi.FileName;
					if (IsPossibleFile(name)) {
						files.Add(name);
						// Add the file to the project dictionary here.
						// This saves the lookup time when the corresponding project
						// is needed later.
						ProjectFileDictionaryService.AddFile(name, p);
					}
				}
			}
		}
		
		static void AddFilesFromViewContent(IList<string> files, IViewContent vc)
		{
			files.AddRange(vc.Files
			               .Select(f => f.FileName.ToString())
			               .Where(name => name != null && IsPossibleFile(name))
			              );
		}
		
		/// <summary>
		/// Determines whether the specified file could possibly contain resource references
		/// that can be detected by at least one registered resource resolver.
		/// </summary>
		public static bool IsPossibleFile(string name)
		{
			return ResourceResolverService.Resolvers.Any(resolver => resolver.SupportsFile(name));
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Finds a string literal in a source code file with respect to escaping rules
		/// of the language.
		/// </summary>
		/// <param name="fileName">The name of the file to search in.</param>
		/// <param name="fileContent">The text content of the file.</param>
		/// <param name="literal">The string literal to find.</param>
		/// <param name="startOffset">The position to start searching at.</param>
		/// <param name="code">Receives the unquoted program code that represents the specified string literal in the language this file is written in.</param>
		/// <returns>The next index where the specified string literal appears, or -1 if there is no match or the language cannot be determined.</returns>
		public static int FindStringLiteral(string fileName, string fileContent, string literal, int startOffset, out string code)
		{
			ICSharpCode.SharpDevelop.Dom.LanguageProperties lp = NRefactoryResourceResolver.GetLanguagePropertiesForFile(fileName);
			
			if (lp != null && lp.CodeGenerator != null) {
				
				code = lp.CodeGenerator.GenerateCode(new PrimitiveExpression(literal, literal), String.Empty);
				
				if (!String.IsNullOrEmpty(code)) {
					// Unquote the string if possible.
					if (code.StartsWith("\"") || code.StartsWith("'")) {
						code = code.Substring(1);
					}
					if (code.EndsWith("\"") || code.EndsWith("'")) {
						code = code.Remove(code.Length-1);
					}
					return fileContent.IndexOf(code, startOffset, StringComparison.OrdinalIgnoreCase);
				}
				
			}
			
			code = null;
			return -1;
		}
	}
	
	/// <summary>
	/// Determines a search scope for finding references to resources.
	/// </summary>
	public enum SearchScope {
		/// <summary>Search the whole solution.</summary>
		WholeSolution,
		/// <summary>Search the current project.</summary>
		CurrentProject,
		/// <summary>Search the current file (in the active view content).</summary>
		CurrentFile,
		/// <summary>Search all currently open files (all open view contents).</summary>
		OpenFiles
	}
}
