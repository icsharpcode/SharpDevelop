// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
	public sealed class ParseProjectContent : DefaultProjectContent
	{
		public ParseProjectContent(IProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.Language = project.LanguageProperties;
			this.initializing = true;
		}
		
		readonly IProject project;
		
		public override object Project {
			get {
				return project;
			}
		}
		
		public string ProjectName {
			get { return project.Name; }
		}
		
		public override string AssemblyName {
			get { return project.AssemblyName; }
		}
		
		bool initializing;
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, project.Name);
		}
		
		internal void Initialize1(IProgressMonitor progressMonitor)
		{
			ICollection<ProjectItem> items = project.Items;
			ProjectService.ProjectItemAdded   += OnProjectItemAdded;
			ProjectService.ProjectItemRemoved += OnProjectItemRemoved;
			UpdateDefaultImports(items);
			// TODO: Translate me
//			progressMonitor.TaskName = "Resolving references for " + project.Name + "...";
			AbstractProject abstractProject = project as AbstractProject;
			ReferencedContents.Clear();
			if (abstractProject != null) {
				foreach (var reference in abstractProject.ResolveAssemblyReferences(progressMonitor.CancellationToken)) {
					if (!initializing) return; // abort initialization
					AddReference(reference, false, progressMonitor.CancellationToken);
				}
			} else {
				project.ResolveAssemblyReferences();
				AddReferencedContent(AssemblyParserService.DefaultProjectContentRegistry.Mscorlib);
				foreach (ProjectItem item in items) {
					if (!initializing) return; // abort initialization
					progressMonitor.CancellationToken.ThrowIfCancellationRequested();
					if (ItemType.ReferenceItemTypes.Contains(item.ItemType)) {
						ReferenceProjectItem reference = item as ReferenceProjectItem;
						if (reference != null) {
							AddReference(reference, false, progressMonitor.CancellationToken);
						}
					}
				}
			}
			UpdateReferenceInterDependencies();
			OnReferencedContentsChanged(EventArgs.Empty);
		}
		
		internal void ReInitialize1(IProgressMonitor progressMonitor)
		{
			// prevent adding event handler twice
			ProjectService.ProjectItemAdded   -= OnProjectItemAdded;
			ProjectService.ProjectItemRemoved -= OnProjectItemRemoved;
			initializing = true;
			try {
				Initialize1(progressMonitor);
			} finally {
				initializing = false;
			}
		}
		
		void UpdateReferenceInterDependencies()
		{
			// Use ToArray because the collection could be modified inside the loop
			IProjectContent[] referencedContents;
			lock (this.ReferencedContents) {
				referencedContents = new IProjectContent[this.ReferencedContents.Count];
				this.ReferencedContents.CopyTo(referencedContents, 0);
			}
			foreach (IProjectContent referencedContent in referencedContents) {
				if (referencedContent is ReflectionProjectContent) {
					((ReflectionProjectContent)referencedContent).InitializeReferences(referencedContents);
				}
			}
		}
		
		void AddReference(ReferenceProjectItem reference, bool updateInterDependencies, CancellationToken cancellationToken)
		{
			try {
				cancellationToken.ThrowIfCancellationRequested();
				AddReferencedContent(AssemblyParserService.GetProjectContentForReference(reference));
				if (updateInterDependencies) {
					UpdateReferenceInterDependencies();
				}
				OnReferencedContentsChanged(EventArgs.Empty);
				
				// Refresh the reference if required.
				// If the user removes the reference and then re-adds it, there might be other references
				// in the project depending on it, so we do the refresh after the old reference was added.
				AssemblyParserService.RefreshProjectContentForReference(reference);
			} catch (OperationCanceledException) {
				throw;
			} catch (ObjectDisposedException e) {
				// ObjectDisposedException can happen if project gets disposed while LoadSolutionProjectsThread is running.
				// We will ignore the ObjectDisposedException and throw OperationCanceledException instead.
				cancellationToken.ThrowIfCancellationRequested();
				MessageService.ShowException(e);
			} catch (Exception e) {
				MessageService.ShowException(e);
			}
		}
		
		// ensure that com references are built serially because we cannot invoke multiple instances of MSBuild
		static Queue<System.Windows.Forms.MethodInvoker> callAfterAddComReference = new Queue<System.Windows.Forms.MethodInvoker>();
		static bool buildingComReference;
		
		void OnProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			
			ReferenceProjectItem reference = e.ProjectItem as ReferenceProjectItem;
			if (reference != null) {
				if (reference.ItemType == ItemType.COMReference) {
					System.Windows.Forms.MethodInvoker action = delegate {
						// Compile project to ensure interop library is generated
						project.Save(); // project is not yet saved when ItemAdded fires, so save it here
						string message = StringParser.Parse("\n${res:MainWindow.CompilerMessages.CreatingCOMInteropAssembly}\n");
						TaskService.BuildMessageViewCategory.AppendText(message);
						BuildCallback afterBuildCallback = delegate {
							lock (callAfterAddComReference) {
								if (callAfterAddComReference.Count > 0) {
									// run next enqueued action
									callAfterAddComReference.Dequeue()();
								} else {
									buildingComReference = false;
									ParserService.Reparse(project, true, false);
								}
							}
						};
						BuildEngine.BuildInGui(project, new BuildOptions(BuildTarget.ResolveComReferences, afterBuildCallback));
					};
					
					// enqueue actions when adding multiple COM references so that multiple builds of the same project
					// are not started parallely
					lock (callAfterAddComReference) {
						if (buildingComReference) {
							callAfterAddComReference.Enqueue(action);
						} else {
							buildingComReference = true;
							action();
						}
					}
				} else {
					ParserService.Reparse(project, true, false);
				}
			}
			if (e.ProjectItem.ItemType == ItemType.Import) {
				UpdateDefaultImports(project.Items);
			} else if (e.ProjectItem.ItemType == ItemType.Compile) {
				if (System.IO.File.Exists(e.ProjectItem.FileName)) {
					ParserService.BeginParse(e.ProjectItem.FileName);
				}
			}
		}
		
		void OnProjectItemRemoved(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			
			ReferenceProjectItem reference = e.ProjectItem as ReferenceProjectItem;
			if (reference != null) {
				try {
					IProjectContent referencedContent = AssemblyParserService.GetExistingProjectContentForReference(reference);
					if (referencedContent != null) {
						lock (ReferencedContents) {
							ReferencedContents.Remove(referencedContent);
						}
						OnReferencedContentsChanged(EventArgs.Empty);
					}
				} catch (Exception ex) {
					MessageService.ShowException(ex);
				}
			}
			
			if (e.ProjectItem.ItemType == ItemType.Import) {
				UpdateDefaultImports(project.Items);
			} else if (e.ProjectItem.ItemType == ItemType.Compile) {
				ParserService.ClearParseInformation(e.ProjectItem.FileName);
			}
		}
		
		int languageDefaultImportCount = -1;
		
		void UpdateDefaultImports(ICollection<ProjectItem> items)
		{
			if (languageDefaultImportCount < 0) {
				languageDefaultImportCount = (DefaultImports != null) ? DefaultImports.Usings.Count : 0;
			}
			if (languageDefaultImportCount == 0) {
				DefaultImports = null;
			} else {
				while (DefaultImports.Usings.Count > languageDefaultImportCount) {
					DefaultImports.Usings.RemoveAt(languageDefaultImportCount);
				}
			}
			foreach (ProjectItem item in items) {
				if (item.ItemType == ItemType.Import) {
					if (DefaultImports == null) {
						DefaultImports = new DefaultUsing(this);
					}
					DefaultImports.Usings.Add(item.Include);
				}
			}
		}
		
		internal int GetInitializationWorkAmount()
		{
			return project.Items.Count;
		}
		
		internal void ReInitialize2(IProgressMonitor progressMonitor)
		{
			if (initializing) return;
			initializing = true;
			Initialize2(progressMonitor);
		}
		
		internal void Initialize2(IProgressMonitor progressMonitor)
		{
			if (!initializing) return;
			try {
				IProjectContent[] referencedContents;
				lock (this.ReferencedContents) {
					referencedContents = new IProjectContent[this.ReferencedContents.Count];
					this.ReferencedContents.CopyTo(referencedContents, 0);
				}
				
				foreach (IProjectContent referencedContent in referencedContents) {
					if (referencedContent is ReflectionProjectContent) {
						((ReflectionProjectContent)referencedContent).InitializeReferences(referencedContents);
					}
				}
				
				ParseableFileContentFinder finder = new ParseableFileContentFinder();
				var fileContents = (
					from p in project.Items.AsParallel().WithCancellation(progressMonitor.CancellationToken)
					where !ItemType.NonFileItemTypes.Contains(p.ItemType) && !String.IsNullOrEmpty(p.FileName)
					select FileName.Create(p.FileName)
				).ToList();
				
				object progressLock = new object();
				double fileCountInverse = 1.0 / fileContents.Count;
				Parallel.ForEach(
					fileContents,
					new ParallelOptions {
						MaxDegreeOfParallelism = Environment.ProcessorCount * 2,
						CancellationToken = progressMonitor.CancellationToken
					},
					fileName => {
						// Don't read files we don't have a parser for.
						// This avoids loading huge files (e.g. sdps) when we have no intention of parsing them.
						if (ParserService.GetParser(fileName) != null) {
							ITextBuffer content = finder.Create(fileName);
							if (content != null)
								ParserService.ParseFile(this, fileName, content);
						}
						lock (progressLock) {
							progressMonitor.Progress += fileCountInverse;
						}
					}
				);
			} finally {
				initializing = false;
				progressMonitor.Progress = 1;
			}
		}
		
		public override void Dispose()
		{
			ProjectService.ProjectItemAdded   -= OnProjectItemAdded;
			ProjectService.ProjectItemRemoved -= OnProjectItemRemoved;
			initializing = false;
			base.Dispose();
		}
	}
}
