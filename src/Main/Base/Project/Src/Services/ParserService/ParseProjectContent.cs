// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using System.Threading;

namespace ICSharpCode.SharpDevelop
{
	public sealed class ParseProjectContent : DefaultProjectContent
	{
		internal static ParseProjectContent CreateUninitalized(IProject project)
		{
			ParseProjectContent newProjectContent = new ParseProjectContent();
			newProjectContent.project = project;
			newProjectContent.Language = project.LanguageProperties;
			newProjectContent.initializing = true;
			IProjectContent mscorlib = AssemblyParserService.GetRegistryForReference(new ReferenceProjectItem(project, "mscorlib")).Mscorlib;
			newProjectContent.AddReferencedContent(mscorlib);
			return newProjectContent;
		}
		
		IProject project;
		
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
		
		internal void Initialize1(CancellationToken token)
		{
			ICollection<ProjectItem> items = project.Items;
			ProjectService.ProjectItemAdded   += OnProjectItemAdded;
			ProjectService.ProjectItemRemoved += OnProjectItemRemoved;
			UpdateDefaultImports(items);
			// TODO: Translate me
//			progressMonitor.TaskName = "Resolving references for " + project.Name + "...";
			project.ResolveAssemblyReferences();
			foreach (ProjectItem item in items) {
				if (!initializing) return; // abort initialization
				token.ThrowIfCancellationRequested();
				if (ItemType.ReferenceItemTypes.Contains(item.ItemType)) {
					ReferenceProjectItem reference = item as ReferenceProjectItem;
					if (reference != null) {
						// TODO: Translate me
//						progressMonitor.TaskName = "Loading " + reference.ShortName + "...";
						AddReference(reference, false);
					}
				}
			}
			UpdateReferenceInterDependencies();
			OnReferencedContentsChanged(EventArgs.Empty);
		}
		
		internal void ReInitialize1(CancellationToken token)
		{
			lock (ReferencedContents) {
				ReferencedContents.Clear();
				AddReferencedContent(AssemblyParserService.GetRegistryForReference(new ReferenceProjectItem(project, "mscorlib")).Mscorlib);
			}
			// prevent adding event handler twice
			ProjectService.ProjectItemAdded   -= OnProjectItemAdded;
			ProjectService.ProjectItemRemoved -= OnProjectItemRemoved;
			initializing = true;
			try {
				Initialize1(token);
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
					((ReflectionProjectContent)referencedContent).InitializeReferences();
				}
			}
		}
		
		void AddReference(ReferenceProjectItem reference, bool updateInterDependencies)
		{
			try {
				AddReferencedContent(AssemblyParserService.GetProjectContentForReference(reference));
				if (updateInterDependencies) {
					UpdateReferenceInterDependencies();
				}
				OnReferencedContentsChanged(EventArgs.Empty);
				
				// Refresh the reference if required.
				// If the user removes the reference and then re-adds it, there might be other references
				// in the project depending on it, so we do the refresh after the old reference was added.
				AssemblyParserService.RefreshProjectContentForReference(reference);
			} catch (Exception e) {
				MessageService.ShowError(e);
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
						TaskService.BuildMessageViewCategory.AppendText("\n${res:MainWindow.CompilerMessages.CreatingCOMInteropAssembly}\n");
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
					MessageService.ShowError(ex);
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
		
		internal void ReInitialize2(CancellationToken token)
		{
			if (initializing) return;
			initializing = true;
			Initialize2(token);
		}
		
		internal void Initialize2(CancellationToken token)
		{
			if (!initializing) return;
			//int progressStart = progressMonitor.WorkDone;
			ParseableFileContentEnumerator enumerator = new ParseableFileContentEnumerator(project);
			try {
				IProjectContent[] referencedContents;
				lock (this.ReferencedContents) {
					referencedContents = new IProjectContent[this.ReferencedContents.Count];
					this.ReferencedContents.CopyTo(referencedContents, 0);
				}
				
				foreach (IProjectContent referencedContent in referencedContents) {
					if (referencedContent is ReflectionProjectContent) {
						((ReflectionProjectContent)referencedContent).InitializeReferences();
					}
				}
				
				while (enumerator.MoveNext()) {
					int i = enumerator.Index;
					if ((i % 4) == 2) {
						//progressMonitor.WorkDone = progressStart + i;
						token.ThrowIfCancellationRequested();
					}
					
					ParserService.ParseFile(this, enumerator.CurrentFileName, new StringTextBuffer(enumerator.CurrentFileContent));
					
					if (!initializing) return;
				}
			} finally {
				initializing = false;
				//progressMonitor.WorkDone = progressStart + enumerator.ItemCount;
				enumerator.Dispose();
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
