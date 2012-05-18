// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using RegistryContentPair = System.Collections.Generic.KeyValuePair<ICSharpCode.SharpDevelop.Dom.ProjectContentRegistry, ICSharpCode.SharpDevelop.Dom.IProjectContent>;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Portions of parser service that deal with loading external assemblies for code completion.
	/// </summary>
	public static class AssemblyParserService
	{
		static IList<ProjectContentRegistryDescriptor> registries;
		static ProjectContentRegistry defaultProjectContentRegistry = new ProjectContentRegistry();
		static string domPersistencePath;
		
		internal static void Initialize()
		{
			if (registries == null) {
				registries = AddInTree.BuildItems<ProjectContentRegistryDescriptor>("/Workspace/ProjectContentRegistry", null, false);
				if (!string.IsNullOrEmpty(domPersistencePath)) {
					Directory.CreateDirectory(domPersistencePath);
					defaultProjectContentRegistry.ActivatePersistence(domPersistencePath);
				}
			}
		}
		
		/// <summary>
		/// Gets/Sets the cache directory used for DOM persistence.
		/// </summary>
		public static string DomPersistencePath {
			get {
				return domPersistencePath;
			}
			set {
				if (registries != null)
					throw new InvalidOperationException("Cannot set DomPersistencePath after ParserService was initialized");
				domPersistencePath = value;
			}
		}
		
		public static ProjectContentRegistry DefaultProjectContentRegistry {
			get {
				return defaultProjectContentRegistry;
			}
		}
		
		public static ProjectContentRegistry GetRegistryForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem || item.Project == null) {
				return defaultProjectContentRegistry;
			}
			foreach (ProjectContentRegistryDescriptor registry in registries) {
				if (registry.UseRegistryForProject(item.Project)) {
					ProjectContentRegistry r = registry.Registry;
					if (r != null) {
						return r;
					} else {
						return defaultProjectContentRegistry; // fallback when registry class not found
					}
				}
			}
			return defaultProjectContentRegistry;
		}
		
		public static IProjectContent GetExistingProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				if (((ProjectReferenceProjectItem)item).ReferencedProject == null)
				{
					return null;
				}
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			return GetRegistryForReference(item).GetExistingProjectContent(item.FileName);
		}
		
		public static IProjectContent GetProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				if (((ProjectReferenceProjectItem)item).ReferencedProject == null)
				{
					return null;
				}
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			return GetRegistryForReference(item).GetProjectContentForReference(item.Include, item.FileName);
		}
		
		/// <summary>
		/// Refreshes the project content for the specified reference if required.
		/// This method does nothing if the reference is not an assembly reference, is not loaded or already is up-to-date.
		/// </summary>
		public static void RefreshProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				return;
			}
			ProjectContentRegistry registry = GetRegistryForReference(item);
			registry.RunLocked(
				delegate {
					IProjectContent rpc = GetExistingProjectContentForReference(item);
					if (rpc == null) {
						LoggingService.Debug("RefreshProjectContentForReference: not refreshing (rpc==null) " + item.FileName);
						return;
					}
					if (rpc.IsUpToDate) {
						LoggingService.Debug("RefreshProjectContentForReference: not refreshing (rpc.IsUpToDate) " + item.FileName);
						return;
					}
					LoggingService.Debug("RefreshProjectContentForReference " + item.FileName);
					
					HashSet<IProject> projectsToRefresh = new HashSet<IProject>();
					HashSet<IProjectContent> unloadedReferenceContents = new HashSet<IProjectContent>();
					UnloadReferencedContent(projectsToRefresh, unloadedReferenceContents, registry, rpc);
					
					foreach (IProject p in projectsToRefresh) {
						ParserService.Reparse(p, true, false);
					}
				});
		}
		
		static void UnloadReferencedContent(HashSet<IProject> projectsToRefresh, HashSet<IProjectContent> unloadedReferenceContents, ProjectContentRegistry referencedContentRegistry, IProjectContent referencedContent)
		{
			LoggingService.Debug("Unload referenced content " + referencedContent);
			
			List<RegistryContentPair> otherContentsToUnload = new List<RegistryContentPair>();
			foreach (ProjectContentRegistryDescriptor registry in registries) {
				if (registry.IsRegistryLoaded) {
					foreach (IProjectContent pc in registry.Registry.GetLoadedProjectContents()) {
						if (pc.ThreadSafeGetReferencedContents().Contains(referencedContent)) {
							if (unloadedReferenceContents.Add(pc)) {
								LoggingService.Debug("Mark dependent content for unloading " + pc);
								otherContentsToUnload.Add(new RegistryContentPair(registry.Registry, pc));
							}
						}
					}
				}
			}
			
			foreach (IProjectContent pc in ParserService.AllProjectContents) {
				IProject project = (IProject)pc.Project;
				if (projectsToRefresh.Contains(project))
					continue;
				lock (pc.ReferencedContents) {
					if (pc.ReferencedContents.Remove(referencedContent)) {
						LoggingService.Debug("UnloadReferencedContent: Mark project for reparsing " + project.Name);
						projectsToRefresh.Add(project);
					}
				}
			}
			
			foreach (RegistryContentPair pair in otherContentsToUnload) {
				UnloadReferencedContent(projectsToRefresh, unloadedReferenceContents, pair.Key, pair.Value);
			}
			
			referencedContentRegistry.UnloadProjectContent(referencedContent);
		}
	}
}
