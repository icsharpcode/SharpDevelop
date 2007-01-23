// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using Hornung.ResourceToolkit.Resolver;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor.Document;
using SearchAndReplace;

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
			return FindReferences(new SpecificResourceReferenceFinder(resourceFileName, key), monitor);
		}
		
		/// <summary>
		/// Finds all references to resources (except the definition) using the specified
		/// <see cref="IResourceReferenceFinder"/> object.
		/// </summary>
		/// <param name="finder">The <see cref="IResourceReferenceFinder"/> to use to find resource references.</param>
		/// <param name="monitor">An object implementing <see cref="IProgressMonitor"/> to report the progress of the operation. Can be <c>null</c>.</param>
		/// <returns>A list of references to resources.</returns>
		public static List<Reference> FindReferences(IResourceReferenceFinder finder, IProgressMonitor monitor)
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
				
				ICollection<string> files = GetPossibleFiles();
				
				if (monitor != null) {
					monitor.BeginTask("${res:SharpDevelop.Refactoring.FindingReferences}", files.Count, true);
				}
				
				foreach (string fileName in files) {
					
					if (monitor != null && monitor.IsCancelled) {
						return null;
					}
					
					IDocument doc = null;
					try {
						// The following line throws an exception if the file does not exist.
						// But the file may be in an unsaved view content (which would be found by GetDocumentInformation),
						// so we cannot simply loop on !File.Exists(...).
						doc = FindReferencesAndRenameHelper.GetDocumentInformation(fileName).CreateDocument();
					} catch (FileNotFoundException) {
					}
					if (doc == null) {
						if (monitor != null) ++monitor.WorkDone;
						continue;
					}
					
					string fileContent = doc.TextContent;
					if (String.IsNullOrEmpty(fileContent)) {
						if (monitor != null) ++monitor.WorkDone;
						continue;
					}
					
					int pos = -1;
					while ((pos = finder.GetNextPossibleOffset(fileName, fileContent, pos)) >= 0) {
						
						Point docPos = doc.OffsetToPosition(pos);
						ResourceResolveResult rrr = ResourceResolverService.Resolve(fileName, doc, docPos.Y, docPos.X, null);
						
						if (rrr != null && rrr.ResourceFileContent != null && rrr.Key != null) {
							if (finder.IsReferenceToResource(rrr)) {
								
								// The actual location of the key string may be after 'pos' because
								// the resolvers may find an expression just before it.
								string keyString = rrr.Key;
								int keyPos = fileContent.IndexOf(keyString, pos, StringComparison.InvariantCultureIgnoreCase);
								
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
								
							}
						}
						
					}
					
					if (monitor != null) ++monitor.WorkDone;
				}
				
				LoggingService.Info("ResourceToolkit: FindReferences finished in "+(DateTime.UtcNow - startTime).TotalSeconds.ToString(System.Globalization.CultureInfo.CurrentCulture)+"s");
				
			} finally {
				NRefactoryAstCacheService.DisableCache();
				if (monitor != null) monitor.Done();
			}
			
			return references;
		}
		
		/// <summary>
		/// Finds all references to resources (except the definitions).
		/// </summary>
		/// <param name="monitor">An object implementing <see cref="IProgressMonitor"/> to report the progress of the operation. Can be <c>null</c>.</param>
		/// <returns>A list of references to resources.</returns>
		public static List<Reference> FindAllReferences(IProgressMonitor monitor)
		{
			return FindReferences(new AnyResourceReferenceFinder(), monitor);
		}
		
		/// <summary>
		/// Finds all references to missing resource keys.
		/// </summary>
		/// <param name="monitor">An object implementing <see cref="IProgressMonitor"/> to report the progress of the operation. Can be <c>null</c>.</param>
		/// <returns>A list of all references to missing resource keys.</returns>
		public static List<Reference> FindReferencesToMissingKeys(IProgressMonitor monitor)
		{
			List<Reference> references = FindAllReferences(monitor);
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
			List<Reference> references = FindAllReferences(monitor);
			if (references == null) {
				return null;
			}
			
			if (monitor != null) {
				monitor.BeginTask(null, 0, false);
			}
			
			List<ResourceItem> unused = new List<ResourceItem>();
			
			// Get a list of all referenced resource files.
			// Generate a dictonary of resource file names and the
			// corresponding referenced keys.
			Dictionary<string, List<string>> referencedKeys = new Dictionary<string, List<string>>();
			foreach (Reference reference in references) {
				ResourceResolveResult rrr = (ResourceResolveResult)reference.ResolveResult;
				if (rrr.ResourceFileContent != null) {
					string fileName = rrr.FileName;
					if (!referencedKeys.ContainsKey(fileName)) {
						referencedKeys.Add(fileName, new List<string>());
					}
					if (rrr.Key != null && !referencedKeys[fileName].Contains(rrr.Key)) {
						referencedKeys[fileName].Add(rrr.Key);
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
					if (!referencedKeys[fileName].Contains(entry.Key)) {
						unused.Add(new ResourceItem(fileName, entry.Key));
					}
				}
			}
			
			if (monitor != null) monitor.Done();
			
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
			
			if (monitor != null) {
				monitor.BeginTask(null, 0, false);
			}
			
			// rename references
			// FIXME: RenameReferences does not enforce escaping rules. May be a problem if someone uses double-quotes in the new resource key name.
			FindReferencesAndRenameHelper.RenameReferences(references, newKey);
			
			// rename definition (if present)
			if (rrr.ResourceFileContent.ContainsKey(rrr.Key)) {
				rrr.ResourceFileContent.RenameKey(rrr.Key, newKey);
			} else {
				if (monitor != null) monitor.ShowingDialog = true;
				MessageService.ShowWarning("${res:Hornung.ResourceToolkit.RenameKeyDefinitionNotFoundWarning}");
				if (monitor != null) monitor.ShowingDialog = false;
			}
			
			// rename definitions in localized resource files
			foreach (KeyValuePair<string, IResourceFileContent> entry in ResourceFileContentRegistry.GetLocalizedContents(rrr.FileName)) {
				if (entry.Value.ContainsKey(rrr.Key)) {
					entry.Value.RenameKey(rrr.Key, newKey);
				}
			}
			
			if (monitor != null) monitor.Done();
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Gets a list of names of files which can possibly contain resource references.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public static ICollection<string> GetPossibleFiles()
		{
			List<string> files = new List<string>();
			
			if (ProjectService.OpenSolution == null) {
				
				foreach (IViewContent vc in WorkbenchSingleton.Workbench.ViewContentCollection) {
					string name = vc.PrimaryFileName;
					if (IsPossibleFile(name)) {
						files.Add(name);
					}
				}
				
			} else {
				
				foreach (IProject p in ProjectService.OpenSolution.Projects) {
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
				
			}
			
			return files.AsReadOnly();
		}
		
		/// <summary>
		/// Determines whether the specified file could possibly contain resource references
		/// that can be detected by at least one registered resource resolver.
		/// </summary>
		public static bool IsPossibleFile(string name)
		{
			foreach (IResourceResolver resolver in ResourceResolverService.Resolvers) {
				if (resolver.SupportsFile(name)) {
					return true;
				}
			}
			return false;
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
					return fileContent.IndexOf(code, startOffset, StringComparison.InvariantCultureIgnoreCase);
				}
				
			}
			
			code = null;
			return -1;
		}
	}
}
