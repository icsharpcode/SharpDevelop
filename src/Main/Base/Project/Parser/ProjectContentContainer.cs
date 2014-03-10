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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	sealed class ProjectContentContainer : IDisposable
	{
		readonly IProject project;
		readonly IParserService parserService = SD.ParserService;
		readonly IUpdateableAssemblyModel assemblyModel;
		
		/// <summary>
		/// Lock for accessing mutable fields of this class.
		/// To avoids deadlocks, the ParserService must not be called while holding this lock.
		/// </summary>
		readonly object lockObj = new object();
		
		IProjectContent projectContent;
		IAssemblyReference[] references = { MinimalCorlib.Instance };
		bool disposed;
		
		/// <summary>
		/// Counter that gets decremented whenever a file gets parsed.
		/// Once the counter reaches zero, it stays at zero and triggers
		/// serialization of the project content on disposal.
		/// This is intended to prevent serialization of (almost) unchanged projects.
		/// </summary>
		bool serializedProjectContentIsUpToDate;
		
		// time necessary for loading references, in relation to time for a single C# file
		const int LoadingReferencesWorkAmount = 15;
		
		readonly string cacheFileName;
		
		#region Constructor + Dispose
		public ProjectContentContainer(MSBuildBasedProject project, IProjectContent initialProjectContent)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.projectContent = initialProjectContent.SetAssemblyName(project.AssemblyName).SetLocation(project.OutputAssemblyFullPath).AddAssemblyReferences(references);
			this.assemblyModel = (IUpdateableAssemblyModel)project.AssemblyModel;
			this.assemblyModel.AssemblyName = this.projectContent.AssemblyName;
			this.assemblyModel.FullAssemblyName = this.projectContent.FullAssemblyName;
			this.cacheFileName = GetCacheFileName(project.FileName);
			
			SD.ProjectService.ProjectItemAdded += OnProjectItemAdded;
			SD.ProjectService.ProjectItemRemoved += OnProjectItemRemoved;
			SD.AssemblyParserService.AssemblyRefreshed += OnAssemblyRefreshed;
			
			List<FileName> filesToParse = new List<FileName>();
			foreach (var file in project.Items.OfType<FileProjectItem>()) {
				if (IsParseableFile(file)) {
					var fileName = file.FileName;
					parserService.AddOwnerProject(fileName, project, startAsyncParse: false, isLinkedFile: file.IsLink);
					filesToParse.Add(fileName);
				}
			}
			
			SD.ParserService.LoadSolutionProjectsThread.AddJob(
				monitor => Initialize(monitor, filesToParse),
				"Loading " + project.Name + "...", filesToParse.Count + LoadingReferencesWorkAmount);
		}
		
		public void Dispose()
		{
			SD.ProjectService.ProjectItemAdded   -= OnProjectItemAdded;
			SD.ProjectService.ProjectItemRemoved -= OnProjectItemRemoved;
			SD.AssemblyParserService.AssemblyRefreshed -= OnAssemblyRefreshed;
			
			IProjectContent pc;
			bool serializeOnDispose;
			lock (lockObj) {
				if (disposed)
					return;
				disposed = true;
				pc = this.projectContent;
				serializeOnDispose = !this.serializedProjectContentIsUpToDate;
			}
			foreach (var unresolvedFile in pc.Files) {
				parserService.RemoveOwnerProject(FileName.Create(unresolvedFile.FileName), project);
			}
			if (serializeOnDispose)
				SerializeAsync(cacheFileName, pc).FireAndForget();
		}
		#endregion
		
		#region Caching logic (serialization)
		
		static Task SerializeAsync(string cacheFileName, IProjectContent pc)
		{
			if (cacheFileName == null)
				return Task.FromResult<object>(null);
			Task task = IOTaskScheduler.Factory.StartNew(
				delegate {
					pc = pc.RemoveAssemblyReferences(pc.AssemblyReferences);
					int serializableFileCount = 0;
					List<string> nonSerializableUnresolvedFiles = new List<string>();
					foreach (var unresolvedFile in pc.Files) {
						if (IsSerializable(unresolvedFile))
							serializableFileCount++;
						else
							nonSerializableUnresolvedFiles.Add(unresolvedFile.FileName);
					}
					// remove non-serializable parsed files
					if (nonSerializableUnresolvedFiles.Count > 0)
						pc = pc.RemoveFiles(nonSerializableUnresolvedFiles);
					if (serializableFileCount > 3) {
						LoggingService.Debug("Serializing " + serializableFileCount + " files to " + cacheFileName);
						SaveToCache(cacheFileName, pc);
					} else {
						RemoveCache(cacheFileName);
					}
				}, SD.ShutdownService.DelayedShutdownToken);
			SD.ShutdownService.AddBackgroundTask(task);
			return task;
		}
		
		static bool IsSerializable(IUnresolvedFile unresolvedFile)
		{
			return unresolvedFile != null && unresolvedFile.GetType().IsSerializable && unresolvedFile.LastWriteTime != default(DateTime);
		}
		
		static string GetCacheFileName(FileName projectFileName)
		{
			string persistencePath = SD.AssemblyParserService.DomPersistencePath;
			if (persistencePath == null)
				return null;
			string cacheFileName = Path.GetFileNameWithoutExtension(projectFileName);
			if (cacheFileName.Length > 32)
				cacheFileName = cacheFileName.Substring(cacheFileName.Length - 32); // use 32 last characters
			cacheFileName = Path.Combine(persistencePath, cacheFileName + "." + projectFileName.ToString().GetStableHashCode().ToString("x8") + ".prj");
			return cacheFileName;
		}
		
		/// <summary>
		/// Magic number that identifies the SharpDevelop version used to create the cache file.
		/// </summary>
		const int cacheMagicNumber = 5002;
		
		static IProjectContent TryReadFromCache(string cacheFileName)
		{
			if (cacheFileName == null || !File.Exists(cacheFileName))
				return null;
			LoggingService.Debug("Deserializing " + cacheFileName);
			try {
				using (FileStream fs = new FileStream(cacheFileName, FileMode.Open, FileAccess.Read, FileShare.Read | FileShare.Delete, 4096, FileOptions.SequentialScan)) {
					using (BinaryReader reader = new BinaryReaderWith7BitEncodedInts(fs)) {
						if (reader.ReadInt32() != cacheMagicNumber) {
							LoggingService.Warn("Incorrect magic number");
							return null;
						}
						FastSerializer s = new FastSerializer();
						return (IProjectContent)s.Deserialize(reader);
					}
				}
			} catch (IOException ex) {
				LoggingService.Warn(ex);
				return null;
			} catch (UnauthorizedAccessException ex) {
				LoggingService.Warn(ex);
				return null;
			} catch (SerializationException ex) {
				LoggingService.Warn(ex);
				return null;
			} catch (InvalidCastException ex) {
				// can happen if serialized types are incompatible to expected types
				LoggingService.Warn(ex);
				return null;
			} catch (FormatException ex) {
				// e.g. invalid 7-bit-encoded int
				LoggingService.Warn(ex);
				return null;
			}
		}
		
		static void SaveToCache(string cacheFileName, IProjectContent pc)
		{
			try {
				Directory.CreateDirectory(Path.GetDirectoryName(cacheFileName));
				using (FileStream fs = new FileStream(cacheFileName, FileMode.Create, FileAccess.Write)) {
					using (BinaryWriter writer = new BinaryWriterWith7BitEncodedInts(fs)) {
						writer.Write(cacheMagicNumber);
						FastSerializer s = new FastSerializer();
						s.Serialize(writer, pc);
					}
				}
			} catch (IOException ex) {
				LoggingService.Warn(ex);
				// Can happen if two SD instances are trying to access the file at the same time.
				// We'll just let one of them win, and the instance that got the exception won't write to the cache at all.
				// Similarly, we also ignore the other kinds of IO exceptions.
			} catch (UnauthorizedAccessException ex) {
				LoggingService.Warn(ex);
			}
		}
		
		static void RemoveCache(string cacheFileName)
		{
			if (cacheFileName == null)
				return;
			try {
				File.Delete(cacheFileName);
			} catch (IOException ex) {
				LoggingService.Warn(ex);
			} catch (UnauthorizedAccessException ex) {
				LoggingService.Warn(ex);
			}
		}
		
		#endregion
		
		public IProjectContent ProjectContent {
			get {
				lock (lockObj) {
					return projectContent;
				}
			}
		}
		
		public void ParseInformationUpdated(IUnresolvedFile oldFile, IUnresolvedFile newFile)
		{
			// This method is called by the parser service within the parser service (per-file) lock.
			lock (lockObj) {
				if (!disposed) {
					if (newFile != null)
						projectContent = projectContent.AddOrUpdateFiles(newFile);
					else
						projectContent = projectContent.RemoveFiles(oldFile.FileName);
					serializedProjectContentIsUpToDate = false;
					SD.ParserService.InvalidateCurrentSolutionSnapshot();
					SD.MainThread.InvokeAsyncAndForget(delegate { assemblyModel.Update(oldFile, newFile); });
				}
			}
		}
		
		public void SetAssemblyName(string newAssemblyName)
		{
			lock (lockObj) {
				if (!disposed) {
					if (projectContent.FullAssemblyName == newAssemblyName)
						return;
					projectContent = projectContent.SetAssemblyName(newAssemblyName);
					assemblyModel.AssemblyName = projectContent.AssemblyName;
					assemblyModel.FullAssemblyName = projectContent.FullAssemblyName;
					SD.ParserService.InvalidateCurrentSolutionSnapshot();
				}
			}
		}
		
		public void SetLocation(string newLocation)
		{
			lock (lockObj) {
				if (!disposed) {
					if (projectContent.Location == newLocation)
						return;
					projectContent = projectContent.SetLocation(newLocation);
					SD.ParserService.InvalidateCurrentSolutionSnapshot();
				}
			}
		}
		
		public void SetCompilerSettings(object compilerSettings)
		{
			lock (lockObj) {
				if (!disposed) {
					projectContent = projectContent.SetCompilerSettings(compilerSettings);
					SD.ParserService.InvalidateCurrentSolutionSnapshot();
				}
			}
		}
		
		#region Initialize
		void Initialize(IProgressMonitor progressMonitor, List<FileName> filesToParse)
		{
			IReadOnlyCollection<ProjectItem> projectItems = project.Items.CreateSnapshot();
			lock (lockObj) {
				if (disposed) {
					return;
				}
			}

			double scalingFactor = 1.0 / (projectItems.Count + LoadingReferencesWorkAmount);
			using (IProgressMonitor initReferencesProgressMonitor = progressMonitor.CreateSubTask(LoadingReferencesWorkAmount * scalingFactor),
			       parseProgressMonitor = progressMonitor.CreateSubTask(projectItems.Count * scalingFactor))
			{
				var resolveReferencesTask = Task.Run(
					delegate {
						DoResolveReferences(initReferencesProgressMonitor);
					}, initReferencesProgressMonitor.CancellationToken);
				
				ParseFiles(filesToParse, parseProgressMonitor);
				
				resolveReferencesTask.Wait();
			}
		}
		#endregion
		
		#region ParseFiles
		bool IsParseableFile(FileProjectItem projectItem)
		{
			if (projectItem == null || string.IsNullOrEmpty(projectItem.FileName))
				return false;
			return projectItem.ItemType == ItemType.Compile || projectItem.ItemType == ItemType.Page;
		}
		
		void ParseFiles(List<FileName> filesToParse, IProgressMonitor progressMonitor)
		{
			IProjectContent cachedPC = TryReadFromCache(cacheFileName);
			ParseableFileContentFinder finder = new ParseableFileContentFinder();
			
			object progressLock = new object();
			double fileCountInverse = 1.0 / filesToParse.Count;
			int fileCountLoadedFromCache = 0;
			int fileCountParsed = 0;
			int fileCountParsedAndSerializable = 0;
			Parallel.ForEach(
				filesToParse,
				new ParallelOptions {
					MaxDegreeOfParallelism = Environment.ProcessorCount,
					CancellationToken = progressMonitor.CancellationToken
				},
				fileName => {
					ITextSource content = finder.CreateForOpenFile(fileName);
					bool wasLoadedFromCache = false;
					IUnresolvedFile unresolvedFile = null;
					if (content == null && cachedPC != null) {
						unresolvedFile = cachedPC.GetFile(fileName);
						if (unresolvedFile != null && unresolvedFile.LastWriteTime == File.GetLastWriteTimeUtc(fileName)) {
							parserService.RegisterUnresolvedFile(fileName, project, unresolvedFile);
							wasLoadedFromCache = true;
						}
					}
					if (!wasLoadedFromCache) {
						if (content == null) {
							try {
								content = SD.FileService.GetFileContentFromDisk(fileName);
							} catch (IOException) {
							} catch (UnauthorizedAccessException) {
							}
						}
						if (content != null) {
							unresolvedFile = parserService.ParseFile(fileName, content, project);
						}
					}
					lock (progressLock) {
						if (wasLoadedFromCache) {
							fileCountLoadedFromCache++;
						} else {
							fileCountParsed++;
							if (IsSerializable(unresolvedFile))
								fileCountParsedAndSerializable++;
						}
//						SD.MainThread.InvokeAsyncAndForget(delegate { assemblyModel.Update(null, unresolvedFile); });
						progressMonitor.Progress += fileCountInverse;
					}
				});
			LoggingService.Debug(projectContent.AssemblyName + ": ParseFiles() finished. "
			                     + fileCountLoadedFromCache + " files were re-used from CC cache; "
			                     + fileCountParsed + " files were parsed (" + fileCountParsedAndSerializable + " of those are serializable)");
			lock (lockObj) {
				serializedProjectContentIsUpToDate = (fileCountLoadedFromCache > 0 && fileCountParsedAndSerializable == 0);
			}
		}
		
		AtomicBoolean reparseCodeStartedButNotYetRunning;
		
		public void ReparseCode()
		{
			if (reparseCodeStartedButNotYetRunning.Set()) {
				var filesToParse = (
					from item in project.Items.OfType<FileProjectItem>()
					where IsParseableFile(item)
					select item.FileName
				).ToList();
				SD.ParserService.LoadSolutionProjectsThread.AddJob(
					monitor => {
						reparseCodeStartedButNotYetRunning.Reset();
						DoReparseCode(filesToParse, monitor);
					},
					"Loading " + project.Name + "...", filesToParse.Count);
			}
		}
		
		void DoReparseCode(List<FileName> filesToParse, IProgressMonitor progressMonitor)
		{
			ParseableFileContentFinder finder = new ParseableFileContentFinder();
			double fileCountInverse = 1.0 / filesToParse.Count;
			object progressLock = new object();
			Parallel.ForEach(
				filesToParse,
				new ParallelOptions {
					MaxDegreeOfParallelism = Environment.ProcessorCount,
					CancellationToken = progressMonitor.CancellationToken
				},
				fileName => {
					// Clear cached parse information so that the file is forced to be reparsed even if it is unchanged
					parserService.ClearParseInformation(fileName);
					
					ITextSource content = finder.Create(fileName);
					if (content != null) {
						parserService.ParseFile(fileName, content, project);
					}
					lock (progressLock) {
						progressMonitor.Progress += fileCountInverse;
					}
				});
		}
		#endregion
		
		#region ResolveReferences
		void DoResolveReferences(IProgressMonitor progressMonitor)
		{
			var referenceItems = project.ResolveAssemblyReferences(progressMonitor.CancellationToken);
			const double assemblyResolvingProgress = 0.3; // 30% asm resolving, 70% asm loading
			progressMonitor.Progress += assemblyResolvingProgress;
			progressMonitor.CancellationToken.ThrowIfCancellationRequested();
			
			List<FileName> assemblyFiles = new List<FileName>();
			List<IAssemblyReference> newReferences = new List<IAssemblyReference>();
			
			foreach (var reference in referenceItems) {
				ProjectReferenceProjectItem projectReference = reference as ProjectReferenceProjectItem;
				if (projectReference != null) {
					newReferences.Add(projectReference);
				} else {
					assemblyFiles.Add(reference.FileName);
				}
			}
			
			foreach (var file in assemblyFiles) {
				progressMonitor.CancellationToken.ThrowIfCancellationRequested();
				if (File.Exists(file)) {
					var pc = SD.AssemblyParserService.GetAssembly(file, false, progressMonitor.CancellationToken);
					if (pc != null) {
						newReferences.Add(pc);
					}
				}
				progressMonitor.Progress += (1.0 - assemblyResolvingProgress) / assemblyFiles.Count;
			}
			lock (lockObj) {
				if (!disposed) {
					projectContent = projectContent.RemoveAssemblyReferences(this.references).AddAssemblyReferences(newReferences);
					this.references = newReferences.ToArray();
					SD.ParserService.InvalidateCurrentSolutionSnapshot();
					SD.MainThread.InvokeAsyncAndForget(
						delegate {
							assemblyModel.UpdateReferences(projectContent.AssemblyReferences.Select(ResolveReferenceForAssemblyModel).Where(r => r != null).ToList());
						});
				}
			}
		}
		
		AtomicBoolean reparseReferencesStartedButNotYetRunning;
		
		public void ReparseReferences()
		{
			if (reparseReferencesStartedButNotYetRunning.Set()) {
				SD.ParserService.LoadSolutionProjectsThread.AddJob(
					monitor => {
						reparseReferencesStartedButNotYetRunning.Reset();
						DoResolveReferences(monitor);
					},
					"Loading " + project.Name + "...", LoadingReferencesWorkAmount);
			}
		}
		
		void OnAssemblyRefreshed(object sender, RefreshAssemblyEventArgs e)
		{
			lock (lockObj) {
				int index = Array.IndexOf(this.references, e.OldAssembly);
				if (index >= 0 && e.NewAssembly != null) {
					this.references[index] = e.NewAssembly;
					projectContent = projectContent.RemoveAssemblyReferences(e.OldAssembly).AddAssemblyReferences(e.NewAssembly);
					SD.ParserService.InvalidateCurrentSolutionSnapshot();
					SD.MainThread.InvokeAsyncAndForget(
						delegate {
							assemblyModel.UpdateReferences(projectContent.AssemblyReferences.Select(ResolveReferenceForAssemblyModel).Where(r => r != null).ToList());
						});
				}
			}
		}
		
		DomAssemblyName ResolveReferenceForAssemblyModel(IAssemblyReference reference)
		{
			if (reference is IUnresolvedAssembly)
				return new DomAssemblyName(((IUnresolvedAssembly)reference).FullAssemblyName);
			if (reference is ProjectReferenceProjectItem) {
				var project = ((ProjectReferenceProjectItem)reference).ReferencedProject;
				if (project == null) return null;
				if (project.ProjectContent == null) {
					SD.Log.InfoFormatted("ResolveReference: ProjectContent for project '{0}', language {1} was not found. Cannot resolve reference!", project.Name, project.Language);
					return null;
				}
				return new DomAssemblyName(project.ProjectContent.FullAssemblyName);
			}
			return null;
		}
		#endregion
		
		#region Project Item Added/Removed
		void OnProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			
			ReferenceProjectItem reference = e.ProjectItem as ReferenceProjectItem;
			if (reference != null) {
				ReparseReferences();
			}
			FileProjectItem fileProjectItem = e.ProjectItem as FileProjectItem;
			if (IsParseableFile(fileProjectItem)) {
				var fileName = e.ProjectItem.FileName;
				SD.ParserService.AddOwnerProject(fileName, project, startAsyncParse: true, isLinkedFile: fileProjectItem.IsLink);
			}
		}
		
		void OnProjectItemRemoved(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			
			ReferenceProjectItem reference = e.ProjectItem as ReferenceProjectItem;
			if (reference != null) {
				try {
					ReparseReferences();
				} catch (Exception ex) {
					MessageService.ShowException(ex);
				}
			}
			
			FileProjectItem fileProjectItem = e.ProjectItem as FileProjectItem;
			if (IsParseableFile(fileProjectItem)) {
				SD.ParserService.RemoveOwnerProject(e.ProjectItem.FileName, project);
			}
		}
		#endregion
		
		public IAssemblyModel AssembyModel {
			get { return assemblyModel; }
		}
	}
}
