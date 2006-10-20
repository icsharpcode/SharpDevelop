// <file>
//     <copyright see="prj:///Doc/copyright.txt"/>
//     <license see="prj:///Doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Document;

using Hornung.ResourceToolkit.ResourceFileContent;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Resolves references to resources that are accessed using ICSharpCode.Core
	/// ("${res:...}").
	/// </summary>
	public class ICSharpCodeCoreResourceResolver : AbstractResourceResolver
	{
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ICSharpCodeCoreResourceResolver"/> class.
		/// </summary>
		public ICSharpCodeCoreResourceResolver() : base()
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
			// Any parseable source code file may contain references
			if (ICSharpCode.SharpDevelop.ParserService.GetParser(fileName) != null) {
				return true;
			}
			
			// Support additional files by extension
			switch(Path.GetExtension(fileName).ToLowerInvariant()) {
				case ".addin":
				case ".xfrm":
				case ".xml":
					return true;
				default:
					break;
			}
			
			return false;
		}
		
		static readonly string[] possiblePatterns = new string[] {
			"${res:"
		};
		
		/// <summary>
		/// Gets a list of patterns that can be searched for in the specified file
		/// to find possible resource references that are supported by this
		/// resolver.
		/// </summary>
		/// <param name="fileName">The name of the file to get a list of possible patterns for.</param>
		public override IEnumerable<string> GetPossiblePatternsForFile(string fileName)
		{
			if (this.SupportsFile(fileName)) {
				return possiblePatterns;
			}
			return new string[0];
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// The token that indicates a reference to an ICSharpCode.Core resource.
		/// </summary>
		public const string ResourceReferenceToken = @"${res:";
		
		/// <summary>
		/// Attempts to resolve a reference to a resource.
		/// </summary>
		/// <param name="fileName">The name of the file that contains the expression to be resolved.</param>
		/// <param name="document">The document that contains the expression to be resolved.</param>
		/// <param name="caretLine">The 0-based line in the file that contains the expression to be resolved.</param>
		/// <param name="caretColumn">The 0-based column position of the expression to be resolved.</param>
		/// <param name="caretOffset">The offset of the position of the expression to be resolved.</param>
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the specified position in the specified file, or <c>null</c> if that expression does not reference a (known) resource.</returns>
		protected override ResourceResolveResult Resolve(string fileName, IDocument document, int caretLine, int caretColumn, int caretOffset)
		{
			// Find $ character to the left of the caret.
			caretOffset += 1;
			char ch;
			do {
				ch = document.GetCharAt(--caretOffset);
			} while (!Char.IsWhiteSpace(ch) && ch != '$' && caretOffset > 0);
			
			if (caretOffset + 6 >= document.TextLength || document.GetText(caretOffset, 6) != ResourceReferenceToken) {
				return null;
			}
			caretOffset += 6;
			
			// Read resource key.
			StringBuilder key = new StringBuilder();
			while (caretOffset < document.TextLength && !Char.IsWhiteSpace(ch = document.GetCharAt(caretOffset++)) && ch != '}') {
				key.Append(ch);
			}
			if (ch != '}') {
				key = null;
				#if DEBUG
			} else {
				LoggingService.Debug("ResourceToolkit: ICSharpCodeCoreResourceResolver found resource key: "+key.ToString());
				#endif
			}
			
			string resourceFile = ResolveICSharpCodeCoreResourceFileName(key == null ? null : key.ToString(), fileName);
			
			if (resourceFile != null) {
				// TODO: Add information about callingClass, callingMember, returnType
				return new ResourceResolveResult(null, null, null, ResourceFileContentRegistry.GetResourceFileContent(resourceFile), key == null ? null : key.ToString());
			} else {
				LoggingService.Info("ResourceToolkit: ICSharpCodeCoreResourceResolver: Could not find the ICSharpCode.Core resource file name for the source file '"+fileName+"', key '"+(key == null ? "<null>" : key.ToString())+"'.");
			}
			
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Tries to find the name of the resource file which serves as source for
		/// the resources accessed by the ICSharpCode.Core.
		/// </summary>
		/// <param name="key">The resource key to look for. May be null.</param>
		/// <param name="sourceFileName">The name of the source code file that contains the reference to the resource.</param>
		static string ResolveICSharpCodeCoreResourceFileName(string key, string sourceFileName)
		{
			
			// As there is no easy way to find out the actual location of the resources
			// based on the source code, we just look in some standard directories.
			
			// Local file (SD addin or standalone application with standard directory structure)
			string localFile = GetICSharpCodeCoreLocalResourceFileName(sourceFileName);
			
			// Prefer local file, especially if the key is there.
			if (localFile != null) {
				if (key != null) {
					IResourceFileContent localContent = ResourceFileContentRegistry.GetResourceFileContent(localFile);
					if (localContent != null) {
						if (localContent.ContainsKey(key)) {
							return localFile;
						}
					}
				} else {
					return localFile;
				}
			}
			
			// Resource file of the host application
			string hostFile = GetICSharpCodeCoreHostResourceFileName(sourceFileName);
			if (key != null) {
				if (hostFile != null) {
					IResourceFileContent hostContent = ResourceFileContentRegistry.GetResourceFileContent(hostFile);
					if (hostContent != null) {
						if (hostContent.ContainsKey(key)) {
							return hostFile;
						}
					}
				}
			}
			
			// Use local file also if the key is not there
			// (allows adding of a new key)
			return localFile == null ? hostFile : localFile;
		}
		
		/// <summary>
		/// Tries to find an ICSharpCode.Core resource file in the given path
		/// according to the file names defined in the AddIn tree.
		/// </summary>
		static string FindICSharpCodeCoreResourceFile(string path)
		{
			string file;
			foreach (string fileName in AddInTree.BuildItems<string>("/AddIns/ResourceToolkit/ICSharpCodeCoreResourceResolver/ResourceFileNames", null, false)) {
				if ((file = FindResourceFileName(Path.Combine(path, fileName))) != null) {
					return file;
				}
			}
			return null;
		}
		
		/// <summary>
		/// Tries to find the local string resource file used for ICSharpCode.Core resource access.
		/// </summary>
		/// <param name="sourceFileName">The name of the source code file which to find the ICSharpCode.Core resource file for.</param>
		public static string GetICSharpCodeCoreLocalResourceFileName(string sourceFileName)
		{
			IProject project = ProjectFileDictionaryService.GetProjectForFile(sourceFileName);
			if (project == null || String.IsNullOrEmpty(project.Directory)) {
				return null;
			}
			
			string localFile = null;
			
			if (!NRefactoryAstCacheService.CacheEnabled || !cachedLocalResourceFiles.TryGetValue(project, out localFile)) {
				foreach (string relativePath in AddInTree.BuildItems<string>("/AddIns/ResourceToolkit/ICSharpCodeCoreResourceResolver/LocalResourcesLocations", null, false)) {
					if ((localFile = FindICSharpCodeCoreResourceFile(Path.GetFullPath(Path.Combine(project.Directory, relativePath)))) != null) {
						if (NRefactoryAstCacheService.CacheEnabled) {
							cachedLocalResourceFiles.Add(project, localFile);
						}
						break;
					}
				}
			}
			
			return localFile;
		}
		
		/// <summary>
		/// Tries to find the string resource file of the host application for ICSharpCode.Core resource access.
		/// </summary>
		/// <param name="sourceFileName">The name of the source code file which to find the ICSharpCode.Core resource file for.</param>
		public static string GetICSharpCodeCoreHostResourceFileName(string sourceFileName)
		{
			IProject project = ProjectFileDictionaryService.GetProjectForFile(sourceFileName);
			string hostFile = null;
			
			if (project == null ||
			    !NRefactoryAstCacheService.CacheEnabled || !cachedHostResourceFiles.TryGetValue(project, out hostFile)) {
				
				// Get SD directory using the reference to ICSharpCode.Core
				string coreAssemblyFullPath = GetICSharpCodeCoreFullPath(project);
				
				if (coreAssemblyFullPath == null) {
					// Look for the ICSharpCode.Core project using all available projects.
					if (ProjectService.OpenSolution != null) {
						foreach (IProject p in ProjectService.OpenSolution.Projects) {
							if ((coreAssemblyFullPath = GetICSharpCodeCoreFullPath(p)) != null) {
								break;
							}
						}
					}
				}
				
				if (coreAssemblyFullPath == null) {
					return null;
				}
				
				#if DEBUG
				LoggingService.Debug("ResourceToolkit: ICSharpCodeCoreResourceResolver coreAssemblyFullPath = "+coreAssemblyFullPath);
				#endif
				
				foreach (string relativePath in AddInTree.BuildItems<string>("/AddIns/ResourceToolkit/ICSharpCodeCoreResourceResolver/HostResourcesLocations", null, false)) {
					if ((hostFile = FindICSharpCodeCoreResourceFile(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(coreAssemblyFullPath), relativePath)))) != null) {
						if (NRefactoryAstCacheService.CacheEnabled && project != null) {
							cachedHostResourceFiles.Add(project, hostFile);
						}
						break;
					}
				}
				
			}
			
			return hostFile;
		}
		
		static string GetICSharpCodeCoreFullPath(IProject sourceProject)
		{
			if (sourceProject == null) {
				return null;
			}
			
			string coreAssemblyFullPath = null;
			
			if (sourceProject.Name.Equals("ICSharpCode.Core", StringComparison.InvariantCultureIgnoreCase)) {
				
				// This is the ICSharpCode.Core project itself.
				coreAssemblyFullPath = sourceProject.OutputAssemblyFullPath;
				
			} else {
				
				// Get the ICSharpCode.Core.dll path by using the project reference.
				foreach (ProjectItem item in sourceProject.Items) {
					ProjectReferenceProjectItem prpi = item as ProjectReferenceProjectItem;
					if (prpi != null) {
						if (prpi.ReferencedProject != null) {
							if (prpi.ReferencedProject.Name.Equals("ICSharpCode.Core", StringComparison.InvariantCultureIgnoreCase) && prpi.ReferencedProject.OutputAssemblyFullPath != null) {
								coreAssemblyFullPath = prpi.ReferencedProject.OutputAssemblyFullPath;
								break;
							}
						}
					}
					ReferenceProjectItem rpi = item as ReferenceProjectItem;
					if (rpi != null) {
						if (rpi.Name.Equals("ICSharpCode.Core", StringComparison.InvariantCultureIgnoreCase) && rpi.FileName != null) {
							coreAssemblyFullPath = rpi.FileName;
							break;
						}
					}
				}
				
			}
			
			return coreAssemblyFullPath;
		}
		
		#region ICSharpCode.Core resource file mapping cache
		
		static Dictionary<IProject, string> cachedLocalResourceFiles;
		static Dictionary<IProject, string> cachedHostResourceFiles;
		
		static ICSharpCodeCoreResourceResolver()
		{
			cachedLocalResourceFiles = new Dictionary<IProject, string>();
			cachedHostResourceFiles = new Dictionary<IProject, string>();
			NRefactoryAstCacheService.CacheEnabledChanged += NRefactoryCacheEnabledChanged;
		}
		
		static void NRefactoryCacheEnabledChanged(object sender, EventArgs e)
		{
			if (!NRefactoryAstCacheService.CacheEnabled) {
				// Clear cache when disabled.
				cachedLocalResourceFiles.Clear();
				cachedHostResourceFiles.Clear();
			}
		}
		
		#endregion
	}
}
