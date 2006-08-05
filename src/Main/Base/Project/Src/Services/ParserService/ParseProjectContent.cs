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
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	public class ParseProjectContent : DefaultProjectContent
	{
		internal static ParseProjectContent CreateUninitalized(IProject project)
		{
			ParseProjectContent newProjectContent = new ParseProjectContent();
			newProjectContent.project = project;
			newProjectContent.Language = project.LanguageProperties;
			newProjectContent.ReferencedContents.Add(ProjectContentRegistry.Mscorlib);
			newProjectContent.initializing = true;
			return newProjectContent;
		}
		
		public static ParseProjectContent Create(IProject project)
		{
			ParseProjectContent newProjectContent = CreateUninitalized(project);
			newProjectContent.Initialize1();
			newProjectContent.Initialize2();
			return newProjectContent;
		}
		
		IProject project;
		
		public override IDomProject Project {
			get {
				return project;
			}
		}
		
		bool initializing;
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, project.Name);
		}
		
		internal void Initialize1()
		{
			ProjectItem[] items = project.Items.ToArray();
			ProjectService.ProjectItemAdded   += OnProjectItemAdded;
			ProjectService.ProjectItemRemoved += OnProjectItemRemoved;
			UpdateDefaultImports(items);
			foreach (ProjectItem item in items) {
				if (!initializing) return; // abort initialization
				switch (item.ItemType) {
					case ItemType.Reference:
					case ItemType.ProjectReference:
					case ItemType.COMReference:
						AddReference(item as ReferenceProjectItem, false);
						break;
				}
			}
			UpdateReferenceInterDependencies();
			WorkbenchSingleton.SafeThreadAsyncCall(new Action<EventArgs>(OnReferencedContentsChanged),
			                                       EventArgs.Empty);
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
				IProjectContent referencedContent = ParserService.GetProjectContentForReference(reference);
				if (referencedContent != null) {
					lock (this.ReferencedContents) {
						this.ReferencedContents.Add(referencedContent);
					}
				}
				if (updateInterDependencies) {
					UpdateReferenceInterDependencies();
				}
				WorkbenchSingleton.SafeThreadAsyncCall(new Action<EventArgs>(OnReferencedContentsChanged),
				                                       EventArgs.Empty);
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		
		// waitcallback for AddReference
		void AddReference(object state)
		{
			AddReference((ReferenceProjectItem)state, true);
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
						MSBuildEngineCallback callback = delegate {
							System.Threading.ThreadPool.QueueUserWorkItem(AddReference, reference);
							lock (callAfterAddComReference) {
								if (callAfterAddComReference.Count > 0) {
									callAfterAddComReference.Dequeue()();
								} else {
									buildingComReference = false;
								}
							}
						};
						if (project is MSBuildProject) {
							((MSBuildProject)project).RunMSBuild("ResolveComReferences", callback, null);
						} else {
							project.Build(callback, null);
						}
					};
					lock (callAfterAddComReference) {
						if (buildingComReference) {
							callAfterAddComReference.Enqueue(action);
						} else {
							buildingComReference = true;
							action();
						}
					}
				} else {
					System.Threading.ThreadPool.QueueUserWorkItem(AddReference, reference);
				}
			}
			switch (e.ProjectItem.ItemType) {
				case ItemType.Import:
					UpdateDefaultImports(project.Items.ToArray());
					break;
				case ItemType.Compile:
					ParserService.EnqueueForParsing(e.ProjectItem.FileName);
					break;
			}
		}
		
		void OnProjectItemRemoved(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			
			ReferenceProjectItem reference = e.ProjectItem as ReferenceProjectItem;
			if (reference != null) {
				try {
					IProjectContent referencedContent = ParserService.GetExistingProjectContentForReference(reference);
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
			
			switch (e.ProjectItem.ItemType) {
				case ItemType.Import:
					UpdateDefaultImports(project.Items.ToArray());
					break;
				case ItemType.Compile:
					ParserService.ClearParseInformation(e.ProjectItem.FileName);
					break;
			}
		}
		
		int languageDefaultImportCount = -1;
		
		void UpdateDefaultImports(ProjectItem[] items)
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
		
		internal void ReInitialize2()
		{
			if (initializing) return;
			initializing = true;
			Initialize2();
		}
		
		internal void Initialize2()
		{
			if (!initializing) return;
			int progressStart = StatusBarService.ProgressMonitor.WorkDone;
			ParseableFileContentEnumerator enumerator = new ParseableFileContentEnumerator(project);
			try {
				StatusBarService.ProgressMonitor.TaskName = "${res:ICSharpCode.SharpDevelop.Internal.ParserService.Parsing} " + project.Name + "...";
				
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
					if ((i % 5) == 2)
						StatusBarService.ProgressMonitor.WorkDone = progressStart + i;
					
					ParserService.ParseFile(this, enumerator.CurrentFileName, enumerator.CurrentFileContent, true, false);
					
					if (!initializing) return;
				}
			} finally {
				initializing = false;
				StatusBarService.ProgressMonitor.WorkDone = progressStart + enumerator.ItemCount;
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
