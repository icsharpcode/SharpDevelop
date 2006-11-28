// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Default implementation of the IProject interface.
	/// </summary>
	public abstract class AbstractProject : AbstractSolutionFolder, IProject
	{
		// Member documentation: see IProject members.
		
		#region static methods - DO NOT BELONG HERE; PLEASE MOVE
		public static string GetConfigurationNameFromKey(string key)
		{
			int pos = key.IndexOf('|');
			if (pos < 0)
				return key;
			else
				return key.Substring(0, pos);
		}
		
		public static string GetPlatformNameFromKey(string key)
		{
			return key.Substring(key.IndexOf('|') + 1);
		}
		#endregion
		
		#region IDisposable implementation
		bool isDisposed;
		
		[Browsable(false)]
		public bool IsDisposed {
			get { return isDisposed; }
		}
		
		public event EventHandler Disposed;
		
		public virtual void Dispose()
		{
			WorkbenchSingleton.AssertMainThread();
			
			isDisposed = true;
			if (Disposed != null) {
				Disposed(this, EventArgs.Empty);
			}
		}
		#endregion
		
		#region IMementoCapable implementation
		internal static List<string> filesToOpenAfterSolutionLoad = new List<string>();
		
		/// <summary>
		/// Saves project preferences (currently opened files, bookmarks etc.) to the
		/// a property container.
		/// </summary>
		public virtual Properties CreateMemento()
		{
			WorkbenchSingleton.AssertMainThread();
			
			Properties properties = new Properties();
			properties.Set("bookmarks", ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.GetProjectBookmarks(this).ToArray());
			List<string> files = new List<string>();
			foreach (ICSharpCode.SharpDevelop.Gui.IViewContent vc
			         in ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench.ViewContentCollection)
			{
				string fileName = vc.FileName;
				if (fileName != null && IsFileInProject(fileName)) {
					files.Add(fileName);
				}
			}
			properties.Set("files", files.ToArray());
			return properties;
		}
		
		public virtual void SetMemento(Properties memento)
		{
			WorkbenchSingleton.AssertMainThread();
			
			foreach (ICSharpCode.SharpDevelop.Bookmarks.SDBookmark mark in memento.Get("bookmarks", new ICSharpCode.SharpDevelop.Bookmarks.SDBookmark[0])) {
				ICSharpCode.SharpDevelop.Bookmarks.BookmarkManager.AddMark(mark);
			}
			foreach (string fileName in memento.Get("files", new string[0])) {
				filesToOpenAfterSolutionLoad.Add(fileName);
			}
		}
		#endregion
		
		#region Filename / Directory
		volatile string fileName;
		string cachedDirectoryName;
		
		/// <summary>
		/// Gets the name of the project file.
		/// (Full file name, example: @"D:\Serralongue\SharpDevelop\samples\CustomPad\CustomPad.csproj")
		/// 
		/// Only the getter is thread-safe.
		/// </summary>
		[ReadOnly(true)]
		public string FileName {
			get {
				return fileName ?? "";
			}
			set {
				WorkbenchSingleton.AssertMainThread();
				Debug.Assert(Path.IsPathRooted(value));
				
				lock (SyncRoot) { // locking still required for Directory
					fileName = value;
					cachedDirectoryName = null;
				}
			}
		}
		
		/// <summary>
		/// Gets the directory of the project file.
		/// This is equivalent to Path.GetDirectoryName(project.FileName);
		/// (Example: @"D:\Serralongue\SharpDevelop\samples\CustomPad")
		/// 
		/// This member is thread-safe.
		/// </summary>
		[Browsable(false)]
		public string Directory {
			get {
				lock (SyncRoot) {
					if (cachedDirectoryName == null) {
						try {
							cachedDirectoryName = Path.GetDirectoryName(this.FileName);
						} catch (Exception) {
							cachedDirectoryName = "";
						}
					}
					return cachedDirectoryName;
				}
			}
		}
		#endregion
		
		#region ProjectSections
		List<ProjectSection> projectSections = new List<ProjectSection>();
		
		[Browsable(false)]
		public List<ProjectSection> ProjectSections {
			get {
				WorkbenchSingleton.AssertMainThread();
				return projectSections;
			}
		}
		#endregion
		
		#region Language Properties / Ambience
		[Browsable(false)]
		public virtual ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get {
				return ICSharpCode.SharpDevelop.Dom.LanguageProperties.None;
			}
		}
		
		[Browsable(false)]
		public virtual ICSharpCode.SharpDevelop.Dom.IAmbience Ambience {
			get {
				return null;
			}
		}
		#endregion
		
		#region Configuration / Platform management
		string activeConfiguration = "Debug";
		string activePlatform = "AnyCPU";
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:Dialog.ProjectOptions.Configuration}")]
		public string ActiveConfiguration {
			get { return activeConfiguration; }
			set {
				WorkbenchSingleton.AssertMainThread();
				
				if (activeConfiguration != value) {
					activeConfiguration = value;
					
					OnActiveConfigurationChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler ActiveConfigurationChanged;
		
		protected virtual void OnActiveConfigurationChanged(EventArgs e)
		{
			if (ActiveConfigurationChanged != null) {
				ActiveConfigurationChanged(this, e);
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:Dialog.ProjectOptions.Platform}")]
		public string ActivePlatform {
			get { return activePlatform; }
			set {
				WorkbenchSingleton.AssertMainThread();
				
				if (activePlatform != value) {
					activePlatform = value;
					
					OnActivePlatformChanged(EventArgs.Empty);
				}
			}
		}
		
		public event EventHandler ActivePlatformChanged;
		
		protected virtual void OnActivePlatformChanged(EventArgs e)
		{
			if (ActivePlatformChanged != null) {
				ActivePlatformChanged(this, e);
			}
		}
		
		[Browsable(false)]
		public virtual ICollection<string> ConfigurationNames {
			get {
				return new string[] { "Debug", "Release" };
			}
		}
		
		[Browsable(false)]
		public virtual ICollection<string> PlatformNames {
			get {
				return new string[] { "AnyCPU" };
			}
		}
		#endregion
		
		#region Save
		public void Save()
		{
			Save(this.FileName);
		}
		
		public virtual void Save(string fileName)
		{
		}
		#endregion
		
		/// <summary>
		/// Gets the list of available file item types. This member is thread-safe.
		/// </summary>
		[Browsable(false)]
		public virtual ICollection<ItemType> AvailableFileItemTypes {
			get {
				return ItemType.DefaultFileItems;
			}
		}
		
		/// <summary>
		/// Gets the list of items in the project. This member is thread-safe.
		/// The returned collection is guaranteed not to change - adding new items or removing existing items
		/// will create a new collection.
		/// </summary>
		[Browsable(false)]
		public virtual ReadOnlyCollection<ProjectItem> Items {
			get {
				return new ReadOnlyCollection<ProjectItem>(new ProjectItem[0]);
			}
		}
		
		/// <summary>
		/// Gets all items in the project that have the specified item type.
		/// This member is thread-safe.
		/// </summary>
		public virtual IEnumerable<ProjectItem> GetItemsOfType(ItemType itemType)
		{
			foreach (ProjectItem item in this.Items) {
				if (item.ItemType == itemType) {
					yield return item;
				}
			}
		}
		
		[ReadOnly(true)]
		public virtual string AssemblyName {
			get {
				return this.Name;
			}
			set {
			}
		}
		
		[Browsable(false)]
		public virtual string RootNamespace {
			get {
				return this.Name;
			}
			set {
			}
		}
		
		/// <summary>
		/// Gets the full path of the output assembly.
		/// Returns null when the project does not output any assembly.
		/// </summary>
		[Browsable(false)]
		public virtual string OutputAssemblyFullPath {
			get {
				return null;
			}
		}
		
		[Browsable(false)]
		public virtual string AppDesignerFolder {
			get {
				return "";
			}
			set {
				throw new NotSupportedException();
			}
		}
		
		/// <summary>
		/// Gets the name of the language binding used for the project.
		/// </summary>
		[ReadOnly(true)]
		public virtual string Language {
			get {
				return "";
			}
		}
		
		[Browsable(false)]
		public virtual bool IsStartable {
			get {
				return false;
			}
		}
		
		public virtual void Start(bool withDebugging)
		{
			throw new NotSupportedException();
		}
		
		/// <summary>
		/// Returns true, if a specific file (given by it's name) is inside this project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		public bool IsFileInProject(string fileName)
		{
			return FindFile(fileName) != null;
		}
		
		/// <summary>
		/// Returns the project item for a specific file; or null if the file is not found in the project.
		/// This member is thread-safe.
		/// </summary>
		/// <param name="fileName">The <b>fully qualified</b> file name of the file</param>
		public FileProjectItem FindFile(string fileName)
		{
			lock (SyncRoot) {
				return Linq.Find(Linq.OfType<FileProjectItem>(this.Items),
				                 delegate(FileProjectItem item) {
				                 	return FileUtility.IsEqualFileName(item.FileName, fileName);
				                 });
				// return this.Items.OfType<FileProjectItem>().Find(
				//			 item => FileUtility.IsEqualFileName(item.FileName, outputFileName));
			}
		}
		
		ParseProjectContent IProject.CreateProjectContent()
		{
			return this.CreateProjectContent();
		}
		protected virtual ParseProjectContent CreateProjectContent()
		{
			return null;
		}
		
		public virtual void StartBuild(BuildOptions options)
		{
		}
		
		/// <summary>
		/// Creates a new projectItem for the passed itemType
		/// </summary>
		public virtual ProjectItem CreateProjectItem(Microsoft.Build.BuildEngine.BuildItem item)
		{
			return new UnknownProjectItem(this, item);
		}
		
		#region Dirty
		bool isDirty;
		
		public event EventHandler DirtyChanged;
		
		[Browsable(false)]
		public bool IsDirty {
			get { return isDirty; }
			set {
				isDirty = value;
				if (DirtyChanged != null) {
					DirtyChanged(this, EventArgs.Empty);
				}
			}
		}
		#endregion
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, this.Name);
		}
		
		/// <summary>
		/// Gets the default item type the specified file should have.
		/// Returns ItemType.None for unknown items.
		/// Every override should call base.GetDefaultItemType for unknown file extensions.
		/// </summary>
		public virtual ItemType GetDefaultItemType(string fileName)
		{
			return ItemType.None;
		}
	}
}
