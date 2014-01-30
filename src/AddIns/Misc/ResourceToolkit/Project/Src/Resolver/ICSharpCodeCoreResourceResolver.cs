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
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Project;

namespace Hornung.ResourceToolkit.Resolver
{
	/// <summary>
	/// Resolves references to resources that are accessed using ICSharpCode.Core
	/// ("${res: ... }").
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
			if (ResourceResolverService.GetParser(fileName) != null) {
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
		/// <param name="charTyped">The character that has been typed at the caret position but is not yet in the buffer (this is used when invoked from code completion), or <c>null</c>.</param>
		/// <returns>A <see cref="ResourceResolveResult"/> that describes which resource is referenced by the expression at the specified position in the specified file, or <c>null</c> if that expression does not reference a (known) resource.</returns>
		protected override ResourceResolveResult Resolve(string fileName, IDocument document, int caretLine, int caretColumn, int caretOffset, char? charTyped)
		{
			// If Resolve is invoked from code completion,
			// we are only interested in the ':' character of '${res:'.
			if (charTyped != null && charTyped != ':') {
				return null;
			}
			
			// Find $ character to the left of the caret.
			char ch = document.GetCharAt(caretOffset);
			if (ch == '}' && caretOffset > 0) {
				ch = document.GetCharAt(--caretOffset);
			}
			while (!Char.IsWhiteSpace(ch) && ch != '$' && ch != '}' && caretOffset > 0) {
				ch = document.GetCharAt(--caretOffset);
			}
			
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
			
			ResourceSetReference resource = ResolveICSharpCodeCoreResourceSet(key == null ? null : key.ToString(), fileName);
			
			#if DEBUG
			if (resource.FileName == null) {
				LoggingService.Info("ResourceToolkit: ICSharpCodeCoreResourceResolver: Could not find the ICSharpCode.Core resource file name for the source file '"+fileName+"', key '"+(key == null ? "<null>" : key.ToString())+"'.");
			}
			#endif
			
			// TODO: Add information about callingClass, callingMember, returnType
			return new ResourceResolveResult(null, null, null, resource, key == null ? null : key.ToString());
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Tries to find the resource set which serves as source for
		/// the resources accessed by the ICSharpCode.Core.
		/// </summary>
		/// <param name="key">The resource key to look for. May be null.</param>
		/// <param name="sourceFileName">The name of the source code file that contains the reference to the resource.</param>
		public static ResourceSetReference ResolveICSharpCodeCoreResourceSet(string key, string sourceFileName)
		{
			
			// As there is no easy way to find out the actual location of the resources
			// based on the source code, we just look in some standard directories.
			
			// Local set (SD addin or standalone application with standard directory structure)
			ResourceSetReference local = GetICSharpCodeCoreLocalResourceSet(sourceFileName);
			
			// Prefer local set, especially if the key is there.
			if (local.ResourceFileContent != null) {
				if (key != null) {
					if (local.ResourceFileContent.ContainsKey(key)) {
						return local;
					}
				} else {
					return local;
				}
			}
			
			// Resource set of the host application
			ResourceSetReference host = GetICSharpCodeCoreHostResourceSet(sourceFileName);
			if (key != null) {
				if (host.ResourceFileContent != null) {
					if (host.ResourceFileContent.ContainsKey(key)) {
						return host;
					}
				}
			}
			
			// Use local file also if the key is not there
			// (allows adding of a new key)
			return local.ResourceFileContent == null ? host : local;
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
		/// Gets a dummy resource set name used to represent the ICSharpCode.Core
		/// resource set of the local application.
		/// </summary>
		public const string ICSharpCodeCoreLocalResourceSetName = "[ICSharpCodeCoreLocalResourceSet]";
		
		/// <summary>
		/// Gets a dummy resource set name used to represent the ICSharpCode.Core
		/// resource set of the host application.
		/// </summary>
		public const string ICSharpCodeCoreHostResourceSetName = "[ICSharpCodeCoreHostResourceSet]";
		
		static readonly ResourceSetReference EmptyLocalResourceSetReference = new ResourceSetReference(ICSharpCodeCoreLocalResourceSetName, null);
		static readonly ResourceSetReference EmptyHostResourceSetReference = new ResourceSetReference(ICSharpCodeCoreHostResourceSetName, null);
		
		/// <summary>
		/// Tries to find the local string resource set used for ICSharpCode.Core resource access.
		/// </summary>
		/// <param name="sourceFileName">The name of the source code file which to find the ICSharpCode.Core resource set for.</param>
		/// <returns>A <see cref="ResourceSetReference"/> that describes the referenced resource set. The contained file name may be <c>null</c> if the file cannot be determined.</returns>
		public static ResourceSetReference GetICSharpCodeCoreLocalResourceSet(string sourceFileName)
		{
			IProject project = ProjectFileDictionaryService.GetProjectForFile(sourceFileName);
			if (project == null || String.IsNullOrEmpty(project.Directory)) {
				return EmptyLocalResourceSetReference;
			}
			
			string localFile;
			ResourceSetReference local = null;
			
			if (!NRefactoryAstCacheService.CacheEnabled || !cachedLocalResourceSets.TryGetValue(project, out local)) {
				foreach (string relativePath in AddInTree.BuildItems<string>("/AddIns/ResourceToolkit/ICSharpCodeCoreResourceResolver/LocalResourcesLocations", null, false)) {
					if ((localFile = FindICSharpCodeCoreResourceFile(Path.GetFullPath(Path.Combine(project.Directory, relativePath)))) != null) {
						local = new ResourceSetReference(ICSharpCodeCoreLocalResourceSetName, localFile);
						if (NRefactoryAstCacheService.CacheEnabled) {
							cachedLocalResourceSets.Add(project, local);
						}
						break;
					}
				}
			}
			
			return local ?? EmptyLocalResourceSetReference;
		}
		
		/// <summary>
		/// Tries to find the string resource set of the host application for ICSharpCode.Core resource access.
		/// </summary>
		/// <param name="sourceFileName">The name of the source code file which to find the ICSharpCode.Core resource set for.</param>
		/// <returns>A <see cref="ResourceSetReference"/> that describes the referenced resource set. The contained file name may be <c>null</c> if the file cannot be determined.</returns>
		public static ResourceSetReference GetICSharpCodeCoreHostResourceSet(string sourceFileName)
		{
			IProject project = ProjectFileDictionaryService.GetProjectForFile(sourceFileName);
			ResourceSetReference host = null;
			string hostFile;
			
			if (project == null ||
			    !NRefactoryAstCacheService.CacheEnabled || !cachedHostResourceSets.TryGetValue(project, out host)) {
				
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
					return EmptyHostResourceSetReference;
				}
				
				#if DEBUG
				LoggingService.Debug("ResourceToolkit: ICSharpCodeCoreResourceResolver coreAssemblyFullPath = "+coreAssemblyFullPath);
				#endif
				
				foreach (string relativePath in AddInTree.BuildItems<string>("/AddIns/ResourceToolkit/ICSharpCodeCoreResourceResolver/HostResourcesLocations", null, false)) {
					if ((hostFile = FindICSharpCodeCoreResourceFile(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(coreAssemblyFullPath), relativePath)))) != null) {
						host = new ResourceSetReference(ICSharpCodeCoreHostResourceSetName, hostFile);
						if (NRefactoryAstCacheService.CacheEnabled && project != null) {
							cachedHostResourceSets.Add(project, host);
						}
						break;
					}
				}
				
			}
			
			return host ?? EmptyHostResourceSetReference;
		}
		
		static string GetICSharpCodeCoreFullPath(IProject sourceProject)
		{
			if (sourceProject == null) {
				return null;
			}
			
			string coreAssemblyFullPath = null;
			
			if (sourceProject.Name.Equals("ICSharpCode.Core", StringComparison.OrdinalIgnoreCase)) {
				
				// This is the ICSharpCode.Core project itself.
				coreAssemblyFullPath = sourceProject.OutputAssemblyFullPath;
				
			} else {
				
				// Get the ICSharpCode.Core.dll path by using the project reference.
				foreach (ProjectItem item in sourceProject.Items) {
					ProjectReferenceProjectItem prpi = item as ProjectReferenceProjectItem;
					if (prpi != null) {
						if (prpi.ReferencedProject != null) {
							if (prpi.ReferencedProject.Name.Equals("ICSharpCode.Core", StringComparison.OrdinalIgnoreCase) && prpi.ReferencedProject.OutputAssemblyFullPath != null) {
								coreAssemblyFullPath = prpi.ReferencedProject.OutputAssemblyFullPath;
								break;
							}
						}
					}
					ReferenceProjectItem rpi = item as ReferenceProjectItem;
					if (rpi != null) {
						if (rpi.Name.Equals("ICSharpCode.Core", StringComparison.OrdinalIgnoreCase) && rpi.FileName != null) {
							coreAssemblyFullPath = rpi.FileName;
							break;
						}
					}
				}
				
			}
			
			return coreAssemblyFullPath;
		}
		
		#region ICSharpCode.Core resource set mapping cache
		
		static Dictionary<IProject, ResourceSetReference> cachedLocalResourceSets;
		static Dictionary<IProject, ResourceSetReference> cachedHostResourceSets;
		
		static ICSharpCodeCoreResourceResolver()
		{
			cachedLocalResourceSets = new Dictionary<IProject, ResourceSetReference>();
			cachedHostResourceSets = new Dictionary<IProject, ResourceSetReference>();
			NRefactoryAstCacheService.CacheEnabledChanged += NRefactoryCacheEnabledChanged;
		}
		
		static void NRefactoryCacheEnabledChanged(object sender, EventArgs e)
		{
			if (!NRefactoryAstCacheService.CacheEnabled) {
				// Clear cache when disabled.
				cachedLocalResourceSets.Clear();
				cachedHostResourceSets.Clear();
			}
		}
		
		#endregion
	}
}
