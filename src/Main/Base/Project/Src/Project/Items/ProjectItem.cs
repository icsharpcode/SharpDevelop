// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// A project item is based either on an MSBuild build item, or "manually" saves the
	/// type/include/metadata. The project item is strictly bound to it's parent project.
	/// The MSBuild build item is used while the item is added to the project (IsAddedToProject
	/// is true). During that time, Include may not be an empty string.
	/// However, prior to the item being added to the project, Include may be an empty string
	/// (this is also the default for new items created using the (IProject, ItemType) constructor.
	/// </summary>
	public abstract class ProjectItem : LocalizedObject, IDisposable, ICloneable
	{
		IProject project;
		volatile string fileNameCache;
		bool treatIncludeAsLiteral;
		
		// either use: (bound mode)
		IProjectItemBackendStore buildItem;
		
		// or: (virtual mode)
		string virtualInclude;
		ItemType virtualItemType;
		Dictionary<string, string> virtualMetadata = new Dictionary<string, string>();
		
		protected ProjectItem(IProject project, IProjectItemBackendStore buildItem)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.buildItem = buildItem;
			this.treatIncludeAsLiteral = true;
		}
		
		protected ProjectItem(IProject project, ItemType itemType)
			: this(project, itemType, null)
		{
		}
		
		protected ProjectItem(IProject project, ItemType itemType, string include)
			: this(project, itemType, include, true)
		{
		}
		
		protected ProjectItem(IProject project, ItemType itemType, string include, bool treatIncludeAsLiteral)
		{
			this.project = project;
			this.virtualItemType = itemType;
			this.virtualInclude = include ?? "";
			this.virtualMetadata = new Dictionary<string, string>();
			this.treatIncludeAsLiteral = treatIncludeAsLiteral;
		}
		
		[Browsable(false)]
		public IProject Project {
			get {
				return project;
			}
		}
		
		[Browsable(false)]
		public bool TreatIncludeAsLiteral {
			get { return treatIncludeAsLiteral; }
			set { treatIncludeAsLiteral = value; }
		}
		
		/// <summary>
		/// Gets the object used for synchronization. This is project.SyncRoot for items inside a project; or
		/// virtualMetadata for items without project.
		/// </summary>
		object SyncRoot {
			get {
				if (project != null)
					return project.SyncRoot;
				else
					return virtualMetadata;
			}
		}
		
		/// <summary>
		/// Gets if the item is added to it's owner project.
		/// </summary>
		[Browsable(false)]
		internal bool IsAddedToProject {
			get {
				return buildItem != null;
			}
		}
		
		[Browsable(false)]
		internal IProjectItemBackendStore BuildItem {
			get { return buildItem; }
			set {
				if (project is AbstractProject) {
					((AbstractProject)project).ClearFindFileCache();
				}
				
				if (value != null) {
					virtualMetadata = null;
					virtualItemType = default(ItemType);
					virtualInclude = null;
				} else {
					virtualItemType = this.ItemType;
					virtualInclude = this.Include;
					virtualMetadata = new Dictionary<string, string>();
					foreach (string name in this.MetadataNames) {
						virtualMetadata[name] = this.GetMetadata(name);
					}
				}
				buildItem = value;
			}
		}
		
		[Browsable(false)]
		public ItemType ItemType {
			get {
				lock (SyncRoot) {
					if (buildItem != null)
						return buildItem.ItemType;
					else
						return virtualItemType;
				}
			}
			set {
				lock (SyncRoot) {
					if (buildItem != null)
						buildItem.ItemType = value;
					else
						virtualItemType = value;
				}
			}
		}
		
		[Browsable(false)]
		public string Include {
			get {
				lock (SyncRoot) {
					if (buildItem != null)
						return buildItem.EvaluatedInclude;
					else
						return virtualInclude;
				}
			}
			set {
				lock (SyncRoot) {
					if (project is AbstractProject) {
						((AbstractProject)project).ClearFindFileCache();
					}
					
					if (buildItem != null)
						buildItem.EvaluatedInclude = value;
					else
						virtualInclude = value ?? "";
					fileNameCache = null;
				}
			}
		}
		
		#region Metadata access
		public bool HasMetadata(string metadataName)
		{
			lock (SyncRoot) {
				if (buildItem != null)
					return buildItem.HasMetadata(metadataName);
				else
					return virtualMetadata.ContainsKey(metadataName);
			}
		}
		
		/// <summary>
		/// Gets the evaluated value of the metadata item with the specified name.
		/// Returns an empty string for non-existing meta data items.
		/// </summary>
		public string GetEvaluatedMetadata(string metadataName)
		{
			lock (SyncRoot) {
				if (buildItem != null) {
					return buildItem.GetEvaluatedMetadata(metadataName) ?? "";
				} else {
					string val;
					virtualMetadata.TryGetValue(metadataName, out val);
					if (val == null)
						return "";
					else
						return MSBuildInternals.Unescape(val);
				}
			}
		}
		
		/// <summary>
		/// Gets the value of the metadata item with the specified name.
		/// Returns defaultValue for non-existing meta data items.
		/// </summary>
		public T GetEvaluatedMetadata<T>(string metadataName, T defaultValue)
		{
			return GenericConverter.FromString(GetEvaluatedMetadata(metadataName), defaultValue);
		}
		
		/// <summary>
		/// Gets the escaped/unevaluated value of the metadata item with the specified name.
		/// Returns an empty string for non-existing meta data items.
		/// </summary>
		public string GetMetadata(string metadataName)
		{
			lock (SyncRoot) {
				if (buildItem != null) {
					return buildItem.GetMetadata(metadataName) ?? "";
				} else {
					string val;
					virtualMetadata.TryGetValue(metadataName, out val);
					return val ?? "";
				}
			}
		}
		
		/// <summary>
		/// Sets the value of the specified meta data item. The value is escaped before
		/// setting it to ensure characters like ';' or '$' are not interpreted by MSBuild.
		/// Setting value to null or an empty string results in removing the metadata item.
		/// </summary>
		public void SetEvaluatedMetadata(string metadataName, string value)
		{
			if (string.IsNullOrEmpty(value)) {
				RemoveMetadata(metadataName);
			} else {
				lock (SyncRoot) {
					if (buildItem != null)
						buildItem.SetEvaluatedMetadata(metadataName, value);
					else
						virtualMetadata[metadataName] = MSBuildInternals.Escape(value);
				}
			}
		}
		
		/// <summary>
		/// Sets the value of the specified meta data item. The value is escaped before
		/// setting it to ensure characters like ';' or '$' are not interpreted by MSBuild.
		/// </summary>
		public void SetEvaluatedMetadata<T>(string metadataName, T value)
		{
			SetEvaluatedMetadata(metadataName, GenericConverter.ToString(value));
		}
		
		/// <summary>
		/// Sets the value of the specified meta data item.
		/// Setting value to null or an empty string results in removing the metadata item.
		/// </summary>
		public void SetMetadata(string metadataName, string value)
		{
			if (string.IsNullOrEmpty(value)) {
				RemoveMetadata(metadataName);
			} else {
				lock (SyncRoot) {
					if (buildItem != null)
						buildItem.SetMetadata(metadataName, value);
					else
						virtualMetadata[metadataName] = value;
				}
			}
		}
		
		/// <summary>
		/// Removes the specified meta data item.
		/// </summary>
		public void RemoveMetadata(string metadataName)
		{
			lock (SyncRoot) {
				if (buildItem != null)
					buildItem.RemoveMetadata(metadataName);
				else
					virtualMetadata.Remove(metadataName);
			}
		}
		
		/// <summary>
		/// Gets the names of all existing meta data items on this project item. The resulting collection
		/// is a copy that will not be affected by future changes to the project item.
		/// </summary>
		[Browsable(false)]
		public IEnumerable<string> MetadataNames {
			get {
				lock (SyncRoot) {
					if (buildItem != null)
						return buildItem.MetadataNames;
					else
						return virtualMetadata.Keys.ToArray();
				}
			}
		}
		#endregion
		
		/// <summary>
		/// Copies all meta data from this item to the target item.
		/// </summary>
		public virtual void CopyMetadataTo(ProjectItem targetItem)
		{
			lock (SyncRoot) {
				lock (targetItem.SyncRoot) {
					foreach (string name in this.MetadataNames) {
						targetItem.SetMetadata(name, this.GetMetadata(name));
					}
				}
			}
		}
		
		/// <summary>
		/// Clones this project item. Unless overridden, cloning works by cloning the underlying
		/// MSBuild item and creating a new project item for it.
		/// Using the default Clone() implementation requires that the item is has the Project
		/// property set - cloning a ProjectItem without a project will result in a NotSupportedException.
		/// </summary>
		public virtual ProjectItem Clone()
		{
			if (this.Project != null) {
				return CloneFor(this.Project);
			} else {
				throw new NotSupportedException();
			}
		}
		
		/// <summary>
		/// Clones this project item by cloning the underlying
		/// MSBuild item and creating a new project item in the target project for it.
		/// </summary>
		public ProjectItem CloneFor(IProject targetProject)
		{
			if (targetProject == null)
				throw new ArgumentNullException("project");
			
			// use CreateProjectItem to ensure the clone has the same class
			//  (derived from ProjectItem)
			ProjectItem copy = targetProject.CreateProjectItem(new CloneBuildItem(this));
			// remove reference to cloned item, leaving an unbound project item
			copy.BuildItem = null;
			return copy;
			
		}
		
		class CloneBuildItem : IProjectItemBackendStore
		{
			ProjectItem parent;
			
			public CloneBuildItem(ProjectItem parent)
			{
				this.parent = parent;
			}
			
			/// Gets the owning project.
			public IProject Project {
				get { throw new NotSupportedException(); }
			}
			
			public string UnevaluatedInclude {
				get { return parent.Include; }
				set { throw new NotSupportedException(); }
			}
			public string EvaluatedInclude {
				get { return parent.Include; }
				set { throw new NotSupportedException(); }
			}
			public ItemType ItemType {
				get { return parent.ItemType; }
				set { throw new NotSupportedException(); }
			}
			
			public string GetEvaluatedMetadata(string name)
			{
				return parent.GetEvaluatedMetadata(name);
			}
			public string GetMetadata(string name)
			{
				return parent.GetMetadata(name);
			}
			public bool HasMetadata(string name)
			{
				return parent.HasMetadata(name);
			}
			public void RemoveMetadata(string name)
			{
				parent.RemoveMetadata(name);
			}
			public void SetEvaluatedMetadata(string name, string value)
			{
				parent.SetEvaluatedMetadata(name, value);
			}
			public void SetMetadata(string name, string value)
			{
				parent.SetMetadata(name, value);
			}
			public IEnumerable<string> MetadataNames { 
				get { return parent.MetadataNames; }
			}
		}
		
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		
		/// <summary>
		/// Gets/Sets the full path of the file represented by "Include".
		/// For ProjectItems that are not assigned to any project, the getter returns the value of Include
		/// and the setter throws a NotSupportedException.
		/// </summary>
		[Browsable(false)]
		public virtual string FileName {
			get {
				if (project == null) {
					return this.Include;
				}
				string fileName = this.fileNameCache;
				if (fileName == null) {
					lock (SyncRoot) {
						fileName = FileUtility.NormalizePath(Path.Combine(project.Directory, this.Include));
						fileNameCache = fileName;
					}
				}
				return fileName;
			}
			set {
				if (project == null) {
					throw new NotSupportedException("Not supported for items without project.");
				}
				this.Include = FileUtility.GetRelativePath(project.Directory, value);
			}
		}
		
		bool disposed;
		
		public virtual void Dispose()
		{
			disposed = true;
		}
		
		[Browsable(false)]
		public bool IsDisposed {
			get { return disposed; }
		}
		
		public override string ToString()
		{
			return String.Format("[{0}: <{1} Include='{2}'>]",
			                     GetType().Name, this.ItemType.ItemName, this.Include);
		}
		
		protected override void FilterProperties(PropertyDescriptorCollection globalizedProps)
		{
			base.FilterProperties(globalizedProps);
			foreach (PropertyDescriptor p in AddInTree.BuildItems<PropertyDescriptor>("/SharpDevelop/Views/ProjectBrowser/ContextSpecificProperties", this, false)) {
				globalizedProps.Add(p);
			}
		}
		
		public override void InformSetValue(PropertyDescriptor propertyDescriptor, object component, object value)
		{
			base.InformSetValue(propertyDescriptor, component, value);
			if (project != null) {
				project.Save();
			}
		}
	}
}
