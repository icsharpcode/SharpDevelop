// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	/// <summary>
	/// Stores the compilation units for files.
	/// </summary>
	public static class ParserService
	{
		static readonly object syncLock = new object();
		static IList<ParserDescriptor> parserDescriptors;
		static Dictionary<IProjectContent, IProject> projectContents = new Dictionary<IProjectContent, IProject>();
		static Dictionary<FileName, FileEntry> fileEntryDict = new Dictionary<FileName, FileEntry>();
		
		#region Manage Project Contents
		/// <summary>
		/// Gets the project content for the current project.
		/// </summary>
		/// <remarks>
		/// This property is thread-safe; but that the notion of 'current project'
		/// might not be meaningful on threads other than the main thread.
		/// </remarks>
		public static IProjectContent CurrentProjectContent {
			[DebuggerStepThrough]
			get {
				IProject currentProject = ProjectService.CurrentProject;
				if (currentProject != null)
					return currentProject.ProjectContent;
				else
					return DefaultProjectContent;
			}
		}
		
		/// <summary>
		/// Gets the type resolve context for the current project.
		/// </summary>
		/// <remarks>
		/// To improve performance and ensure the returned data is consistent, use the following code pattern:
		/// <code>
		/// using (var context = ParserService.CurrentTypeResolveContext.Synchronize()) {
		/// 	...
		/// }
		/// </code>
		/// 
		/// This property is thread-safe; but the notion of 'current project'
		/// might not be meaningful on threads other than the main thread.
		/// </remarks>
		public static ITypeResolveContext CurrentTypeResolveContext {
			get {
				IProject currentProject = ProjectService.CurrentProject;
				if (currentProject != null) {
					return currentProject.TypeResolveContext ?? GetDefaultTypeResolveContext();
				} else {
					return GetDefaultTypeResolveContext();
				}
			}
		}
		
		[Obsolete("Use project.ProjectContent instead")]
		public static IProjectContent GetProjectContent(IProject project)
		{
			return project.ProjectContent;
		}
		
		/// <summary>
		/// Gets the project that owns the specified project content.
		/// Returns null for referenced assemblies.
		/// </summary>
		public static IProject GetProject(IProjectContent projectContent)
		{
			if (projectContent == null)
				return null;
			lock (syncLock) {
				IProject project;
				if (projectContents.TryGetValue(projectContent, out project))
					return project;
				else
					return null;
			}
		}
		
		/// <summary>
		/// Gets the list of project contents of all open projects. Does not include assembly project contents.
		/// </summary>
		public static IEnumerable<IProjectContent> AllProjectContents {
			get {
				lock (syncLock) {
					return projectContents.Keys.ToArray();
				}
			}
		}
		
		/// <summary>
		/// Gets all project contents that contain the specified file.
		/// </summary>
		static List<IProjectContent> GetProjectContents(string fileName)
		{
			List<IProjectContent> result = new List<IProjectContent>();
			List<IProjectContent> linkResults = new List<IProjectContent>();
			
			KeyValuePair<IProjectContent, IProject>[] pairs;
			lock (syncLock) {
				pairs = projectContents.ToArray();
			}
			foreach (var pair in pairs) {
				FileProjectItem file = pair.Value.FindFile(fileName);
				if (file != null) {
					// Prefer normal files over linked files.
					// The order matters because GetParseInformation() will return the ICompilationUnit
					// for the first result.
					if (file.IsLink)
						linkResults.Add(pair.Key);
					else
						result.Add(pair.Key);
				}
			}
			result.AddRange(linkResults);
			if (result.Count == 0)
				result.Add(DefaultProjectContent);
			return result;
		}
		
		internal static void RegisterProjectContentForAddedProject(IProject project)
		{
			IProjectContent newContent = project.ProjectContent;
			if (newContent != null) {
				lock (syncLock) {
					projectContents[newContent] = project;
				}
			}
		}
		
		internal static void RemoveProjectContentForRemovedProject(IProject project)
		{
			lock (syncLock) {
				foreach (var pair in projectContents.ToArray()) {
					if (pair.Value == project)
						projectContents.Remove(pair.Key);
				}
			}
		}
		#endregion
		
		#region Default Project Content
		static readonly SimpleProjectContent defaultProjectContent = new SimpleProjectContent();
		static readonly Lazy<Task<IProjectContent>[]> defaultReferences = new Lazy<Task<IProjectContent>[]>(
			delegate {
				string[] assemblies = {
					"mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System.Xml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System.Xml.Linq, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
					"Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
				};
				List<Task<IProjectContent>> tasks = new List<Task<IProjectContent>>();
				foreach (string assemblyName in assemblies) {
					DomAssemblyName name = new DomAssemblyName(assemblyName);
					string fileName = AssemblyParserService.FindReferenceAssembly(name.ShortName);
					if (fileName == null) {
						fileName = GacInterop.FindAssemblyInNetGac(name);
					}
					if (fileName != null) {
						tasks.Add(AssemblyParserService.GetAssemblyAsync(FileName.Create(fileName)));
					}
				}
				return tasks.ToArray();
			});
		
		/// <summary>
		/// Gets the default project content used for files outside of projects.
		/// </summary>
		public static IProjectContent DefaultProjectContent {
			get { return defaultProjectContent; }
		}
		
		/// <summary>
		/// Gets the type resolve context for the default project content.
		/// </summary>
		static ITypeResolveContext GetDefaultTypeResolveContext()
		{
			List<ITypeResolveContext> references = new List<ITypeResolveContext>();
			references.Add(defaultProjectContent);
			foreach (var task in defaultReferences.Value) {
				if (task.IsCompleted && !task.IsFaulted)
					references.Add(task.Result);
			}
			return new CompositeTypeResolveContext(references);
		}
		#endregion
		
		#region Initialization + ParserThread
		internal static void InitializeParserService()
		{
			if (parserDescriptors == null) {
				parserDescriptors = AddInTree.BuildItems<ParserDescriptor>("/Workspace/Parser", null, false);
			}
		}
		
		static DispatcherTimer timer;
		
		internal static void StartParserThread()
		{
			WorkbenchSingleton.DebugAssertMainThread();
			timer = new DispatcherTimer(DispatcherPriority.Background);
			timer.Interval = TimeSpan.FromSeconds(1.5);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		internal static void StopParserThread()
		{
			timer.Stop();
		}
		
		static System.Threading.Tasks.Task lastParseRun;
		
		static void timer_Tick(object sender, EventArgs e)
		{
			if (lastParseRun != null) {
				// don't start another parse run if the last one is still running
				if (!lastParseRun.IsCompleted)
					return;
				lastParseRun = null;
			}
			
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent == null)
				return;
			FileName fileName = viewContent.PrimaryFileName;
			if (fileName == null)
				return;
			if (GetParser(fileName) == null)
				return;
			
			ITextSource snapshot;
			IEditable editable = viewContent as IEditable;
			if (editable != null)
				snapshot = editable.CreateSnapshot();
			else
				snapshot = GetParseableFileContent(viewContent.PrimaryFileName);
			
			lastParseRun = ParseAsync(fileName, snapshot).ContinueWith(
				delegate(Task<IParsedFile> backgroundTask) {
					IParsedFile parseInfo = backgroundTask.Result;
					RaiseParserUpdateStepFinished(new ParserUpdateStepEventArgs(fileName, snapshot, parseInfo));
				});
		}
		#endregion
		
		#region CreateParser / TaskListTokens
		static readonly string[] DefaultTaskListTokens = {"HACK", "TODO", "UNDONE", "FIXME"};
		
		/// <summary>
		/// Gets/Sets the task list tokens.
		/// This property is thread-safe.
		/// </summary>
		public static string[] TaskListTokens {
			get { return PropertyService.Get("SharpDevelop.TaskListTokens", DefaultTaskListTokens); }
			set { PropertyService.Set("SharpDevelop.TaskListTokens", value); }
		}
		
		/// <summary>
		/// Creates a new IParser instance that can parse the specified file.
		/// This method is thread-safe.
		/// </summary>
		public static IParser CreateParser(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (parserDescriptors == null)
				return null;
			foreach (ParserDescriptor descriptor in parserDescriptors) {
				if (descriptor.CanParse(fileName)) {
					IParser p = descriptor.CreateParser();
					if (p != null) {
						p.LexerTags = TaskListTokens;
						return p;
					}
				}
			}
			return null;
		}
		#endregion
		
		#region GetParseableFileContent
		/// <summary>
		/// Gets the default file encoding.
		/// This property is thread-safe.
		/// </summary>
		public static Encoding DefaultFileEncoding {
			get {
				return Encoding.GetEncoding(FileService.DefaultFileEncodingCodePage);
			}
		}
		
		/// <summary>
		/// Gets the content of the specified file.
		/// This method is thread-safe. This method involves waiting for the main thread, so using it while
		/// holding a lock can lead to deadlocks.
		/// </summary>
		public static ITextSource GetParseableFileContent(string fileName)
		{
			return Gui.WorkbenchSingleton.SafeThreadFunction(GetParseableFileContentInternal, fileName);
		}
		
		static ITextSource GetParseableFileContentInternal(string fileName)
		{
			//ITextBuffer res = project.GetParseableFileContent(fileName);
			//if (res != null)
			//	return res;
			
			OpenedFile file = FileService.GetOpenedFile(fileName);
			if (file != null) {
				IFileDocumentProvider p = file.CurrentView as IFileDocumentProvider;
				if (p != null) {
					IDocument document = p.GetDocumentForFile(file);
					if (document != null) {
						return document.CreateSnapshot();
					}
				}
				
				using(Stream s = file.OpenRead()) {
					// load file
					return new StringTextSource(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(s, DefaultFileEncoding));
				}
			}
			
			// load file
			return new StringTextSource(ICSharpCode.AvalonEdit.Utils.FileReader.ReadFileContent(fileName, DefaultFileEncoding));
		}
		#endregion
		
		#region Parse Information Management
		static readonly IParsedFile[] emptyCompilationUnitArray = new IParsedFile[0];
		
		sealed class FileEntry
		{
			readonly FileName fileName;
			internal readonly IParser parser;
			volatile IParsedFile mainParsedFile;
			ITextSourceVersion bufferVersion;
			IParsedFile[] oldUnits = emptyCompilationUnitArray;
			bool disposed;
			
			public FileEntry(FileName fileName)
			{
				this.fileName = fileName;
				this.parser = CreateParser(fileName);
			}
			
			/// <summary>
			/// Intended for unit tests only
			/// </summary>
			public void RegisterParseInformation(IParsedFile cu)
			{
				lock (this) {
					this.oldUnits = new IParsedFile[] { cu };
					this.mainParsedFile = cu;
				}
			}
			
			public IParsedFile GetParseInformation(IProjectContent content)
			{
				IParsedFile p = GetExistingParseInformation(content);
				if (p != null)
					return p;
				else
					return ParseFile(content, null);
			}
			
			public IParsedFile GetExistingParseInformation(IProjectContent content)
			{
				if (content == null) {
					return this.mainParsedFile; // read volatile
				} else {
					IParsedFile p = this.mainParsedFile; // read volatile
					if (p != null && p.ProjectContent == content)
						return p;
					lock (this) {
						if (this.oldUnits != null) {
							IParsedFile cu = this.oldUnits.FirstOrDefault(c => c.ProjectContent == content);
							return cu;
						} else {
							return null;
						}
					}
				}
			}
			
			public IParsedFile ParseFile(IProjectContent parentProjectContent, ITextSource fileContent)
			{
				if (parser == null)
					return null;
				
				if (fileContent == null) {
					// GetParseableFileContent must not be called inside any lock
					// (otherwise we'd risk deadlocks because GetParseableFileContent must invoke on the main thread)
					fileContent = GetParseableFileContent(fileName);
				}
				
				ITextSourceVersion fileContentVersion = fileContent.Version;
				List<IProjectContent> projectContents;
				lock (this) {
					if (this.disposed)
						return null;
					
					if (fileContentVersion != null && this.bufferVersion != null && this.bufferVersion.BelongsToSameDocumentAs(fileContentVersion)) {
						if (this.bufferVersion.CompareAge(fileContentVersion) >= 0) {
							// Special case: (necessary due to parentProjectContent optimization)
							// Detect when a file belongs to multiple projects but the ParserService hasn't realized
							// that, yet. In this case, do another parse run to detect all parent projects.
							if (!(parentProjectContent != null && this.oldUnits.Length == 1 && this.oldUnits[0].ProjectContent != parentProjectContent)) {
								return this.mainParsedFile;
							}
						}
					}
					
					if (parentProjectContent != null && (oldUnits.Length == 0 || (oldUnits.Length == 1 && oldUnits[0].ProjectContent == parentProjectContent))) {
						// Optimization: if parentProjectContent is specified and doesn't conflict with what we already know,
						// we will use it instead of doing an expensive GetProjectContents call.
						projectContents = new List<IProjectContent>();
						projectContents.Add(parentProjectContent);
					} else {
						projectContents = GetProjectContents(fileName);
					}
				}
				// We now leave the lock to do the actual parsing.
				// This is done to allow IParser implementations to invoke methods on the main thread without
				// risking deadlocks.
				
				// parse once for each project content that contains the file
				ParseInformation[] newParseInfo = new ParseInformation[projectContents.Count];
				IParsedFile[] newUnits = new IParsedFile[projectContents.Count];
				ParseInformation resultParseInfo = null;
				IParsedFile resultUnit = null;
				for (int i = 0; i < projectContents.Count; i++) {
					IProjectContent pc = projectContents[i];
					try {
						newParseInfo[i] = parser.Parse(pc, fileName, fileContent);
					} catch (Exception ex) {
						throw new ApplicationException("Error parsing " + fileName, ex);
					}
					if (newParseInfo[i] == null)
						throw new NullReferenceException(parser.GetType().Name + ".Parse() returned null");
					newUnits[i] = newParseInfo[i].ParsedFile;
					if (i == 0 || pc == parentProjectContent) {
						resultParseInfo = newParseInfo[i];
						resultUnit = newUnits[i];
					}
				}
				lock (this) {
					if (this.disposed)
						return null;
					
					// ensure we never go backwards in time (we need to repeat this check after we've reacquired the lock)
					if (fileContentVersion != null && this.bufferVersion != null && this.bufferVersion.BelongsToSameDocumentAs(fileContentVersion)) {
						if (this.bufferVersion.CompareAge(fileContentVersion) >= 0) {
							if (parentProjectContent != null) {
								IParsedFile oldUnit = oldUnits.FirstOrDefault(o => o.ProjectContent == parentProjectContent);
								if (oldUnit != null)
									return oldUnit;
							}
							return this.mainParsedFile;
						}
					}
					
					for (int i = 0; i < newUnits.Length; i++) {
						IProjectContent pc = projectContents[i];
						// update the compilation unit
						IParsedFile oldUnit = oldUnits.FirstOrDefault(o => o.ProjectContent == pc);
						// ensure the new unit is frozen beforewe make it visible to the outside world
						newUnits[i].Freeze();
						pc.UpdateProjectContent(oldUnit, newUnits[i]);
						RaiseParseInformationUpdated(new ParseInformationEventArgs(oldUnit, newParseInfo[i], newParseInfo[i] == resultParseInfo));
					}
					
					// remove all old units that don't exist anymore
					foreach (IParsedFile oldUnit in oldUnits) {
						if (!newUnits.Any(n => n.ProjectContent == oldUnit.ProjectContent)) {
							oldUnit.ProjectContent.UpdateProjectContent(oldUnit, null);
							RaiseParseInformationUpdated(new ParseInformationEventArgs(oldUnit, null, false));
						}
					}
					
					this.bufferVersion = fileContentVersion;
					this.oldUnits = newUnits;
					this.mainParsedFile = resultUnit;
					return resultUnit;
				}
			}
			
			public void Clear()
			{
				IParsedFile parseInfo;
				IParsedFile[] oldUnits;
				lock (this) {
					// by setting the disposed flag, we'll cause all running ParseFile() calls to return null and not
					// call into the parser anymore, so we can do the remainder of the clean-up work outside the lock
					this.disposed = true;
					parseInfo = this.mainParsedFile;
					oldUnits = this.oldUnits;
					this.oldUnits = null;
					this.bufferVersion = null;
					this.mainParsedFile = null;
				}
				foreach (IParsedFile oldUnit in oldUnits) {
					oldUnit.ProjectContent.UpdateProjectContent(oldUnit, null);
					bool isPrimary = parseInfo == oldUnit;
					RaiseParseInformationUpdated(new ParseInformationEventArgs(oldUnit, null, isPrimary));
				}
			}
			
			public Task<IParsedFile> BeginParse(ITextSource fileContent)
			{
				// TODO: don't use background task if fileContent was specified and up-to-date parse info is available
				return System.Threading.Tasks.Task.Factory.StartNew(
					delegate {
						try {
							return ParseFile(null, fileContent);
						} catch (Exception ex) {
							MessageService.ShowException(ex, "Error during async parse");
							return null;
						}
					}
				);
			}
		}
		
		static FileEntry GetFileEntry(FileName fileName, bool createOnDemand)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			FileEntry entry;
			lock (syncLock) {
				if (!fileEntryDict.TryGetValue(fileName, out entry)) {
					if (!createOnDemand)
						return null;
					entry = new FileEntry(fileName);
					fileEntryDict.Add(fileName, entry);
				}
			}
			return entry;
		}
		
		/// <summary>
		/// Removes all parse information (both IParsedFile and ParseInformation) for the specified file.
		/// This method is thread-safe.
		/// </summary>
		public static void ClearParseInformation(FileName fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			LoggingService.Info("ClearParseInformation: " + fileName);
			
			FileEntry entry;
			lock (syncLock) {
				if (fileEntryDict.TryGetValue(fileName, out entry)) {
					fileEntryDict.Remove(fileName);
				}
			}
			if (entry != null)
				entry.Clear();
		}
		
		/// <summary>
		/// This is the old method returning potentially-stale parse information.
		/// Use Parse()/ParseFile() instead if you need fresh parse info; otherwise use GetExistingParsedFile().
		/// </summary>
		[Obsolete("Use Parse()/ParseFile() instead if you need fresh parse info; otherwise use GetExistingParsedFile().")]
		public static ParseInformation GetParseInformation(string fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			return ParseFile(FileName.Create(fileName));
		}
		
		/// <summary>
		/// Gets full parse information for the specified file, if it is available.
		/// </summary>
		/// <returns>
		/// Returns the ParseInformation for the specified file,
		/// or null if it is not in the parse information cache.
		/// 
		/// If only the IParsedFile is available (non-full parse information), this method
		/// returns null.
		/// </returns>
		/// <remarks>
		/// This method is thread-safe.
		/// 
		/// The ParserService may drop elements from the cache at any moment,
		/// only IParsedFile will be stored for a longer time.
		/// </remarks>
		public static ParseInformation GetCachedParseInformation(FileName fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			FileEntry entry = GetFileEntry(fileName, false);
			if (entry != null)
				return entry.GetCachedParseInformation(null);
			else
				return null;
		}
		
		/// <summary>
		/// Gets parse information for the specified file.
		/// </summary>
		/// <returns>
		/// Returns the IParsedFile for the specified file,
		/// or null if the file has not been parsed yet.
		/// </returns>
		/// <remarks>This method is thread-safe.</remarks>
		public static IParsedFile GetExistingParsedFile(FileName fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			FileEntry entry = GetFileEntry(fileName, false);
			if (entry != null)
				return entry.GetExistingParsedFile(null);
			else
				return null;
		}
		
		/// <summary>
		/// Gets parse information for the specified file in the context of the
		/// specified project content.
		/// </summary>
		/// <param name="parentProjectContent">
		/// Project content to use as a parent project for the parse run.
		/// Specifying the project content explicitly can be useful when a file is used in multiple projects.
		/// </param>
		/// <param name="fileName">Name of the file.</param>
		/// <returns>
		/// Returns the IParsedFile for the specified file,
		/// or null if the file has not been parsed for that project content.
		/// </returns>
		/// <remarks>This method is thread-safe.</remarks>
		public static IParsedFile GetExistingParsedFile(IProjectContent parentProjectContent, FileName fileName)
		{
			if (string.IsNullOrEmpty(fileName))
				return null;
			FileEntry entry = GetFileEntry(fileName, false);
			if (entry != null)
				return entry.GetExistingParsedFile(parentProjectContent);
			else
				return null;
		}
		
		/// <summary>
		/// Parses the specified file.
		/// Produces full parse information.
		/// </summary>
		/// <param name="fileName">Name of the file to parse</param>
		/// <param name="fileContent">Optional: Content of the file to parse.
		/// The fileContent is taken as a hint - if a newer version than it is already available, that will be used instead.
		/// </param>
		/// <returns>
		/// Returns the ParseInformation for the specified file, or null if the file cannot be parsed.
		/// For files currently open in an editor, this method does not necessary reparse, but may return
		/// an existing cached parse information (but only if it's still up-to-date).
		/// </returns>
		/// <remarks>
		/// This method is thread-safe. This parser being used may involve locking or waiting for the main thread,
		/// so using this method while holding a lock can lead to deadlocks.
		/// </remarks>
		public static ParseInformation Parse(FileName fileName, ITextSource fileContent = null)
		{
			return GetFileEntry(fileName, true).Parse(null, fileContent);
		}
		
		/// <summary>
		/// Asynchronous version of <see cref="Parse(FileName, ITextSource)"/>.
		/// </summary>
		/// <param name="fileName">Name of the file to parse</param>
		/// <param name="fileContent">Optional: Content of the file to parse.
		/// The fileContent is taken as a hint - if a newer version than it is already available, that will be used instead.
		/// </param>
		/// <returns>
		/// Returns the ParseInformation for the specified file, or null if the file cannot be parsed.
		/// For files currently open in an editor, this method does not necessary reparse, but may return
		/// an existing cached parse information (but only if it's still up-to-date).
		/// </returns>
		/// <remarks>
		/// This method is thread-safe. This parser being used may involve locking or waiting for the main thread,
		/// so using this method while holding a lock can lead to deadlocks.
		/// </remarks>
		public static Task<ParseInformation> ParseAsync(FileName fileName, ITextSource fileContent = null)
		{
			if (fileContent == null)
				throw new ArgumentNullException("fileContent");
			// create snapshot (in case someone passes a mutable document to BeginParse)
			return GetFileEntry(fileName, true).BeginParse(fileContent.CreateSnapshot());
		}
		
		
		/// <summary>
		/// Parses the specified file.
		/// This method does not request full parse information 
		/// </summary>
		/// <param name="parentProjectContent">
		/// Project content to use as a parent project for the parse run.
		/// Specifying the project content explicitly can be useful when a file is used in multiple projects.
		/// </param>
		/// <param name="fileName">Name of the file to parse</param>
		/// <param name="fileContent">Optional: Content of the file to parse.
		/// The fileContent is taken as a hint - if a newer version than it is already available, that will be used instead.
		/// </param>
		/// <returns>
		/// Returns the ParseInformation for the specified file, or null if the file cannot be parsed.
		/// For files currently open in an editor, this method does not necessary reparse, but may return
		/// an existing cached parse information (but only if it's still up-to-date).
		/// </returns>
		/// <remarks>
		/// This method is thread-safe. This parser being used may involve locking or waiting for the main thread,
		/// so using this method while holding a lock can lead to deadlocks.
		/// </remarks>
		public static IParsedFile ParseFile(IProjectContent parentProjectContent, FileName fileName, ITextSource fileContent = null)
		{
			return GetFileEntry(fileName, true).Parse(parentProjectContent, fileContent);
		}
		
		/// <summary>
		/// Parses the current view content.
		/// This method can only be called from the main thread.
		/// </summary>
		public static ParseInformation ParseCurrentViewContent()
		{
			WorkbenchSingleton.AssertMainThread();
			IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
			if (viewContent != null)
				return ParseViewContent(viewContent);
			else
				return null;
		}
		
		/// <summary>
		/// Parses the specified view content.
		/// This method can only be called from the main thread.
		/// </summary>
		public static ParseInformation ParseViewContent(IViewContent viewContent)
		{
			if (viewContent == null)
				throw new ArgumentNullException("viewContent");
			WorkbenchSingleton.AssertMainThread();
			if (string.IsNullOrEmpty(viewContent.PrimaryFileName))
				return null;
			IEditable editable = viewContent as IEditable;
			if (editable != null)
				return ParseFile(viewContent.PrimaryFileName, editable.CreateSnapshot());
			else
				return ParseFile(viewContent.PrimaryFileName);
		}
		
		/// <summary>
		/// Gets the parser instance that is responsible for the specified file.
		/// Will create a new IParser instance on demand.
		/// This method is thread-safe.
		/// </summary>
		public static IParser GetParser(string fileName)
		{
			return GetFileEntry(fileName, true).parser;
		}
		
		/// <summary>
		/// Registers a compilation unit in the parser service.
		/// Does not fire the OnParseInformationUpdated event, please use this for unit tests only!
		/// </summary>
		public static void RegisterParseInformation(string fileName, IParsedFile cu)
		{
			FileEntry entry = GetFileEntry(fileName, true);
			entry.RegisterParseInformation(cu);
		}
		
		/// <summary>
		/// Replaces the list of available parsers.
		/// Please use this for unit tests only!
		/// </summary>
		public static void RegisterAvailableParsers(params ParserDescriptor[] descriptors)
		{
			lock (syncLock) {
				parserDescriptors = new List<ParserDescriptor>();
				parserDescriptors.AddRange(descriptors);
			}
		}
		
		#endregion
		
		#region ParseInformationUpdated / ParserUpdateStepFinished events
		/// <summary>
		/// Occurs whenever parse information was updated. This event is raised on the main thread.
		/// </summary>
		public static event EventHandler<ParseInformationEventArgs> ParseInformationUpdated = delegate {};
		
		static void RaiseParseInformationUpdated(ParseInformationEventArgs e)
		{
			// RaiseParseInformationUpdated is called inside a lock, but we don't want to raise the event inside that lock.
			// To ensure events are raised in the same order, we always invoke on the main thread.
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					string addition;
					if (e.OldParsedFile == null)
						addition = " (new)";
					else if (e.NewParsedFile == null)
						addition = " (removed)";
					else
						addition = " (updated)";
					LoggingService.Debug("ParseInformationUpdated " + e.FileName + addition);
					ParseInformationUpdated(null, e);
				});
		}
		
		/// <summary>
		/// Occurs when the parse step started by a timer finishes.
		/// This event is raised on the main thread.
		/// </summary>
		public static event EventHandler<ParserUpdateStepEventArgs> ParserUpdateStepFinished = delegate {};
		
		static void RaiseParserUpdateStepFinished(ParserUpdateStepEventArgs e)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					ParserUpdateStepFinished(null, e);
				});
		}
		
		#endregion
		
		#region LoadSolutionProjects
		
		/// <summary>
		/// Gets whether the LoadSolutionProjects thread is currently running.
		/// </summary>
		public static bool LoadSolutionProjectsThreadRunning {
			get { return LoadSolutionProjects.IsThreadRunning; }
		}
		
		/// <summary>
		/// Occurs when the 'load solution projects' thread has finished.
		/// This event is not raised when the 'load solution projects' is aborted because the solution was closed.
		/// This event is raised on the main thread.
		/// </summary>
		public static event EventHandler LoadSolutionProjectsThreadEnded {
			add { LoadSolutionProjects.ThreadEnded += value; }
			remove { LoadSolutionProjects.ThreadEnded -= value; }
		}
		
		internal static void OnSolutionLoaded()
		{
		}
		
		internal static void OnSolutionClosed()
		{
			LoadSolutionProjects.CancelAllJobs();
			lock (syncLock) {
				projectContents.Clear();
			}
			ClearAllFileEntries();
		}
		
		static void ClearAllFileEntries()
		{
			FileEntry[] entries;
			lock (syncLock) {
				entries = fileEntryDict.Values.ToArray();
				fileEntryDict.Clear();
			}
			foreach (FileEntry entry in entries)
				entry.Clear();
		}
		#endregion
	}
}
