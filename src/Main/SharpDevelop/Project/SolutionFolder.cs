// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
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
		readonly Guid idGuid;
		
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
			
			void ValidateItem(ISolutionItem item)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				if (item.ParentSolution != folder.parentSolution)
					throw new ArgumentException("The item belongs to a different solution than the folder");
				if (item.ParentFolder != null)
					throw new ArgumentException("The item already has a parent folder");
			}
			
			protected override void ClearItems()
			{
				var oldItems = this.ToArray();
				base.ClearItems();
				using (BlockReentrancy()) {
					foreach (var item in oldItems) {
						folder.parentSolution.ReportRemovedItem(item);
						item.ParentFolder = null;
					}
				}
			}
			
			protected override void InsertItem(int index, ISolutionItem item)
			{
				ValidateItem(item);
				base.InsertItem(index, item);
			}
			
			protected override void SetItem(int index, ISolutionItem item)
			{
				ValidateItem(item);
				base.SetItem(index, item);
			}
			
			protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
			{
				switch (e.Action) {
					case NotifyCollectionChangedAction.Add:
					case NotifyCollectionChangedAction.Remove:
					case NotifyCollectionChangedAction.Replace:
						if (e.OldItems != null) {
							foreach (ISolutionItem item in e.OldItems) {
								folder.parentSolution.ReportRemovedItem(item);
								item.ParentFolder = null;
							}
						}
						if (e.NewItems != null) {
							foreach (ISolutionItem item in e.NewItems) {
								item.ParentFolder = folder;
								folder.parentSolution.ReportAddedItem(item);
							}
						}
						break;
						// ignore Move; and Reset is special-cased in ClearItems()
				}
				base.OnCollectionChanged(e);
				folder.parentSolution.IsDirty = true;
			}
		}
		
		readonly SolutionItemsCollection items;
		
		public IMutableModelCollection<ISolutionItem> Items {
			get { return items; }
		}
		#endregion
		
		public string Name { get; set; }
		
		public ISolutionFolder ParentFolder { get; set; }
		
		public ISolution ParentSolution {
			get { return parentSolution; }
		}
		
		public Guid IdGuid {
			get { return idGuid; }
		}
		
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
			ProjectLoadInformation loadInfo = new ProjectLoadInformation(parentSolution, fileName, fileName.GetFileNameWithoutExtension());
			var descriptor = ProjectBindingService.GetCodonPerProjectFile(fileName);
			if (descriptor != null) {
				loadInfo.TypeGuid = descriptor.Guid;
			}
			IProject project = ProjectBindingService.LoadProject(loadInfo);
			Debug.Assert(project.IdGuid != Guid.Empty);
			this.Items.Add(project);
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
