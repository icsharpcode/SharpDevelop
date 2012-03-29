// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Parser
{
	public class ParseProjectContentContainer : IDisposable
	{
		readonly MSBuildBasedProject project;
		
		/// <summary>
		/// Lock for accessing mutable fields of this class.
		/// To avoids deadlocks, the ParserService must not be called while holding this lock.
		/// </summary>
		readonly object lockObj = new object();
		
		IProjectContent projectContent;
		IAssemblyReference[] references = { MinimalCorlib.Instance };
		bool disposed;
		
		// time necessary for loading references, in relation to time for a single C# file
		const int LoadingReferencesWorkAmount = 15;
		
		public ParseProjectContentContainer(MSBuildBasedProject project, IProjectContent initialProjectContent)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.projectContent = initialProjectContent.SetAssemblyName(project.AssemblyName);
			
			ProjectService.ProjectItemAdded += OnProjectItemAdded;
			ProjectService.ProjectItemRemoved += OnProjectItemRemoved;
			
			var parserService = SD.ParserService;
			List<FileName> filesToParse = new List<FileName>();
			foreach (var file in project.Items.OfType<FileProjectItem>()) {
				if (IsParseableFile(file)) {
					var fileName = FileName.Create(file.FileName);
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
			ProjectService.ProjectItemAdded   -= OnProjectItemAdded;
			ProjectService.ProjectItemRemoved -= OnProjectItemRemoved;
			lock (lockObj) {
				if (disposed)
					return;
				disposed = true;
			}
			foreach (var parsedFile in projectContent.Files) {
				SD.ParserService.RemoveOwnerProject(FileName.Create(parsedFile.FileName), project);
			}
		}
		
		public IProjectContent ProjectContent {
			get {
				lock (lockObj) {
					return projectContent;
				}
			}
		}
		
		public void ParseInformationUpdated(IParsedFile oldFile, IParsedFile newFile)
		{
			// This method is called by the parser service within the parser service (per-file) lock.
			lock (lockObj) {
				if (!disposed)
					projectContent = projectContent.UpdateProjectContent(oldFile, newFile);
				SD.ParserService.InvalidateCurrentSolutionSnapshot();
			}
		}
		
		bool IsParseableFile(FileProjectItem projectItem)
		{
			if (projectItem == null || string.IsNullOrEmpty(projectItem.FileName))
				return false;
			return projectItem.ItemType == ItemType.Compile || projectItem.ItemType == ItemType.Page;
		}
		
		void Initialize(IProgressMonitor progressMonitor, List<FileName> filesToParse)
		{
			ICollection<ProjectItem> projectItems = project.Items;
			lock (lockObj) {
				if (disposed) {
					throw new ObjectDisposedException("ParseProjectContent");
				}
			}

			double scalingFactor = 1.0 / (project.Items.Count + LoadingReferencesWorkAmount);
			using (IProgressMonitor initReferencesProgressMonitor = progressMonitor.CreateSubTask(LoadingReferencesWorkAmount * scalingFactor),
			       parseProgressMonitor = progressMonitor.CreateSubTask(projectItems.Count * scalingFactor))
			{
				var resolveReferencesTask = ResolveReferencesAsync(projectItems, initReferencesProgressMonitor);
				
				ParseFiles(filesToParse, parseProgressMonitor);
				
				resolveReferencesTask.Wait();
			}
		}
		
		void ParseFiles(IReadOnlyList<FileName> filesToParse, IProgressMonitor progressMonitor)
		{
			ParseableFileContentFinder finder = new ParseableFileContentFinder();
			
			object progressLock = new object();
			double fileCountInverse = 1.0 / filesToParse.Count;
			Parallel.ForEach(
				filesToParse,
				new ParallelOptions {
					MaxDegreeOfParallelism = Environment.ProcessorCount,
					CancellationToken = progressMonitor.CancellationToken
				},
				fileName => {
					ITextSource content = finder.Create(fileName);
					if (content != null) {
						SD.ParserService.ParseFile(fileName, content, project);
					}
					lock (progressLock) {
						progressMonitor.Progress += fileCountInverse;
					}
				}
			);
		}
		
		Task ResolveReferencesAsync(ICollection<ProjectItem> projectItems, IProgressMonitor progressMonitor)
		{
			return Task.Run(
				delegate {
					var referenceItems = project.ResolveAssemblyReferences(progressMonitor.CancellationToken);
					const double assemblyResolvingProgress = 0.3; // 30% asm resolving, 70% asm loading
					progressMonitor.Progress += assemblyResolvingProgress;
					progressMonitor.CancellationToken.ThrowIfCancellationRequested();
					
					List<string> assemblyFiles = new List<string>();
					List<IAssemblyReference> newReferences = new List<IAssemblyReference>();
					
					foreach (var reference in referenceItems) {
						ProjectReferenceProjectItem projectReference = reference as ProjectReferenceProjectItem;
						if (projectReference != null) {
							newReferences.Add(projectReference);
						} else {
							assemblyFiles.Add(reference.FileName);
						}
					}
					
					foreach (string file in assemblyFiles) {
						progressMonitor.CancellationToken.ThrowIfCancellationRequested();
						if (File.Exists(file)) {
							var pc = SD.AssemblyParserService.GetAssembly(FileName.Create(file), progressMonitor.CancellationToken);
							if (pc != null) {
								newReferences.Add(pc);
							}
						}
						progressMonitor.Progress += (1.0 - assemblyResolvingProgress) / assemblyFiles.Count;
					}
					lock (lockObj) {
						projectContent = projectContent.RemoveAssemblyReferences(this.references).AddAssemblyReferences(newReferences);
						this.references = newReferences.ToArray();
						SD.ParserService.InvalidateCurrentSolutionSnapshot();
					}
				}, progressMonitor.CancellationToken);
		}
		
		// ensure that com references are built serially because we cannot invoke multiple instances of MSBuild
		static Queue<Action> callAfterAddComReference = new Queue<Action>();
		static bool buildingComReference;
		
		void OnProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			
			ReferenceProjectItem reference = e.ProjectItem as ReferenceProjectItem;
			if (reference != null) {
				if (reference.ItemType == ItemType.COMReference) {
					Action action = delegate {
						// Compile project to ensure interop library is generated
						project.Save(); // project is not yet saved when ItemAdded fires, so save it here
						string message = StringParser.Parse("\n${res:MainWindow.CompilerMessages.CreatingCOMInteropAssembly}\n");
						TaskService.BuildMessageViewCategory.AppendText(message);
						BuildCallback afterBuildCallback = delegate {
							ReparseReferences();
							lock (callAfterAddComReference) {
								if (callAfterAddComReference.Count > 0) {
									// run next enqueued action
									callAfterAddComReference.Dequeue()();
								} else {
									buildingComReference = false;
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
					ReparseReferences();
				}
			}
			FileProjectItem fileProjectItem = e.ProjectItem as FileProjectItem;
			if (IsParseableFile(fileProjectItem)) {
				var fileName = FileName.Create(e.ProjectItem.FileName);
				SD.ParserService.AddOwnerProject(fileName, project, startAsyncParse: true, isLinkedFile: fileProjectItem.IsLink);
			}
		}
		
		void ReparseReferences()
		{
			throw new NotImplementedException();
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
				SD.ParserService.RemoveOwnerProject(FileName.Create(e.ProjectItem.FileName), project);
			}
		}
	}
}
