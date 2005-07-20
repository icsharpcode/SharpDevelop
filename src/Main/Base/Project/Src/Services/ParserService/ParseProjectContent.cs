using System;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;

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
		
		public IProject Project {
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
						AddReference(item as ReferenceProjectItem);
						break;
				}
			}
		}
		
		delegate void AddReferenceDelegate(ReferenceProjectItem reference);
		
		void AddReference(ReferenceProjectItem reference)
		{
			try {
				IProjectContent referencedContent = ProjectContentRegistry.GetProjectContentForReference(reference);
				if (referencedContent != null) {
					ReferencedContents.Add(referencedContent);
				}
				if (referencedContent is ReflectionProjectContent) {
					((ReflectionProjectContent)referencedContent).InitializeReferences();
				}
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
		}
		
		void OnProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			ReferenceProjectItem reference = e.ProjectItem as ReferenceProjectItem;
			if (reference != null) {
				new AddReferenceDelegate(AddReference).BeginInvoke(reference, null, null);
			}
			if (e.ProjectItem.ItemType == ItemType.Import) {
				UpdateDefaultImports(project.Items.ToArray());
			}
		}
		
		void OnProjectItemRemoved(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			if (e.ProjectItem.ItemType == ItemType.Import) {
				UpdateDefaultImports(project.Items.ToArray());
			}
		}
		
		void UpdateDefaultImports(ProjectItem[] items)
		{
			DefaultImports.Clear();
			DefaultUsing u = null;
			foreach (ProjectItem item in items) {
				if (item.ItemType == ItemType.Import) {
					if (u == null) {
						u = new DefaultUsing(this);
						DefaultImports.Add(u);
					}
					u.Usings.Add(item.Include);
				}
			}
		}
		
		internal int GetInitializationWorkAmount()
		{
			return project.Items.Count;
		}
		
		internal void Initialize2()
		{
			if (!initializing) return;
			int progressStart = StatusBarService.ProgressMonitor.WorkDone;
			ParseableFileContentEnumerator enumerator = new ParseableFileContentEnumerator(project);
			try {
				StatusBarService.ProgressMonitor.TaskName = "Parsing " + project.Name + "...";
				
				foreach (IProjectContent referencedContent in ReferencedContents) {
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
