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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	class SolutionFolder : ISolutionFolder
	{
		readonly Solution parentSolution;
		Guid idGuid;
		
		public SolutionFolder(Solution parentSolution, Guid idGuid)
		{
			this.parentSolution = parentSolution;
			this.idGuid = idGuid;
			this.items = new SolutionItemsCollection(this);
		}
		
		protected SolutionFolder()
		{
			this.parentSolution = (Solution)this;
			this.items = new SolutionItemsCollection(this);
		}
		
		#region Items Collection
		sealed class SolutionItemsCollection : SimpleModelCollection<ISolutionItem>
		{
			readonly SolutionFolder folder;
			
			public SolutionItemsCollection(SolutionFolder folder)
			{
				this.folder = folder;
			}
			
			protected override void ValidateItem(ISolutionItem item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				if (item.ParentSolution != folder.parentSolution)
					throw new ArgumentException("The item belongs to a different solution than the folder");
				if (item.ParentFolder != null)
					throw new ArgumentException("The item already has a parent folder");
			}
			
			public override IDisposable BatchUpdate()
			{
				// This method exists to allow moving a project from one folder to another without
				// raising the ISolution.Projects.CollectionChanged event.
				// The batch update within OnCollectionChanged() is not sufficient as it is per-folder,
				// while we need a batch across multiple folders.
				// To allow users to create such a batch update across folders without explicitly exposing solution.ReportBatch()
				// in the API; we call ReportBatch() here.
				// API consumers should open batches on both the source and target folder,
				
				
				// Note that the base.BatchUpdate must be disposed first:
				// doing so will call OnCollectionChanged() and update ISolution.Projects;
				// and only then the solution batch may be disposed.
				return new CompositeDisposable(base.BatchUpdate(), folder.parentSolution.ReportBatch());
			}
			
			protected override void OnAdd(ISolutionItem item)
			{
				base.OnAdd(item);
				item.ParentFolder = folder;
			}
			
			protected override void OnRemove(ISolutionItem item)
			{
				base.OnRemove(item);
				item.ParentFolder = null;
			}
			
			protected override void OnCollectionChanged(IReadOnlyCollection<ISolutionItem> removedItems, IReadOnlyCollection<ISolutionItem> addedItems)
			{
				if (folder.parentSolution.IsAncestorOf(folder)) {
					using (folder.parentSolution.ReportBatch()) {
						foreach (ISolutionItem item in removedItems) {
							folder.parentSolution.ReportRemovedItem(item);
						}
						foreach (ISolutionItem item in addedItems) {
							folder.parentSolution.ReportAddedItem(item);
						}
					}
				}
				base.OnCollectionChanged(removedItems, addedItems);
				folder.parentSolution.IsDirty = true;
			}
		}
		
		readonly SolutionItemsCollection items;
		
		[Browsable(false)]
		public IMutableModelCollection<ISolutionItem> Items {
			get { return items; }
		}
		#endregion
		
		[ReadOnly(true)]
		public virtual string Name { get; set; }
		
		[Browsable(false)]
		public ISolutionFolder ParentFolder { get; set; }
		
		[Browsable(false)]
		public ISolution ParentSolution {
			get { return parentSolution; }
		}
		
		[Browsable(false)]
		public Guid IdGuid {
			get { return idGuid; }
			set { idGuid = value; }
		}
		
		[Browsable(false)]
		public Guid TypeGuid {
			get { return ProjectTypeGuids.SolutionFolder; }
		}
		
		public bool IsAncestorOf(ISolutionItem item)
		{
			for (ISolutionItem ancestor = item; ancestor != null; ancestor = ancestor.ParentFolder) {
				if (ancestor == this)
					return true;
			}
			return false;
		}
		
		public IProject AddExistingProject(FileName fileName)
		{
			if (parentSolution.Projects.Any(p => p.FileName == fileName))
				throw new ProjectLoadException("Project " + fileName + " is already part of this solution.");
			ProjectLoadInformation loadInfo = new ProjectLoadInformation(parentSolution, fileName, fileName.GetFileNameWithoutExtension());
			IProject project = SD.ProjectService.LoadProject(loadInfo);
			if (parentSolution.GetItemByGuid(project.IdGuid) != null) {
				SD.Log.Warn("Added project has duplicate GUID; a new GUID will be generated.");
				project.IdGuid = Guid.NewGuid();
			}
			this.Items.Add(project);
			if (parentSolution.IsLoaded) {
				project.ProjectLoaded();
				ProjectBrowserPad.RefreshViewAsync();
			}
			return project;
		}
		
		public ISolutionFileItem AddFile(FileName fileName)
		{
			var newItem = new SolutionFileItem(parentSolution);
			newItem.FileName = fileName;
			this.Items.Add(newItem);
			return newItem;
		}
		
		public ISolutionFolder CreateFolder(string name)
		{
			SolutionFolder newFolder = new SolutionFolder(parentSolution, Guid.NewGuid());
			newFolder.Name = name;
			this.Items.Add(newFolder);
			return newFolder;
		}
	}
}
