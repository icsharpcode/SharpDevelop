// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
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
		
		public override IProject Project {
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
			ProjectService.EndBuild += OnEndBuild;
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
			WorkbenchSingleton.SafeThreadAsyncCall(this, "OnReferencedContentsChanged", EventArgs.Empty);
		}
		
		void UpdateReferenceInterDependencies()
		{
			// Use ToArray because the collection could be modified inside the loop
			IProjectContent[] referencedContents;
			lock (this.referencedContents) {
				referencedContents = this.referencedContents.ToArray();
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
				IProjectContent referencedContent = ProjectContentRegistry.GetProjectContentForReference(reference);
				if (referencedContent != null) {
					lock (this.referencedContents) {
						this.referencedContents.Add(referencedContent);
					}
				}
				if (updateInterDependencies) {
					UpdateReferenceInterDependencies();
				}
				WorkbenchSingleton.SafeThreadAsyncCall(this, "OnReferencedContentsChanged", EventArgs.Empty);
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		
		// waitcallback for AddReference
		void AddReference(object state)
		{
			AddReference((ReferenceProjectItem)state, true);
		}
		
		void OnProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			
			ReferenceProjectItem reference = e.ProjectItem as ReferenceProjectItem;
			if (reference != null) {
				System.Threading.ThreadPool.QueueUserWorkItem(AddReference, reference);
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
					IProjectContent referencedContent = ProjectContentRegistry.GetExistingProjectContentForReference(reference);
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
				lock (this.referencedContents) {
					referencedContents = this.referencedContents.ToArray();
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
			ProjectService.EndBuild -= OnEndBuild;
			initializing = false;
			base.Dispose();
		}
		
		void OnEndBuild(object source, EventArgs e)
		{
			AddComReferences();
		}
		
		void AddComReferences()
		{
			if (project != null) {
				foreach (ProjectItem item in project.Items) {
					if (item.ItemType == ItemType.COMReference) {
						System.Threading.ThreadPool.QueueUserWorkItem(AddReference, item as ReferenceProjectItem);
					}
				}
			}
		}
	}
}
