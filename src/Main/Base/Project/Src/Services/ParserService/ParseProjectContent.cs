// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		
		readonly ParsedFileListener myListener;
		IProjectContent projectContent;
		IAssemblyReference[] references = { MinimalCorlib.Instance };
		bool initializing;
		bool disposed;
		
		public ParseProjectContentContainer(MSBuildBasedProject project, IProjectContent initialProjectContent)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.projectContent = initialProjectContent.SetAssemblyName(project.AssemblyName);
			this.myListener = new ParsedFileListener(OnParsedFileUpdated);
			
			this.initializing = true;
			LoadSolutionProjects.AddJob(Initialize, "Loading " + project.Name + "...", GetInitializationWorkAmount());
		}

		void OnParsedFileUpdated(IParsedFile oldFile, IParsedFile newFile)
		{
			// This method is called by the parser service within the parser service lock.
			lock (lockObj) {
				if (!disposed)
					projectContent = projectContent.UpdateProjectContent(oldFile, newFile);
			}
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
			foreach (var fileName in GetFilesToParse(project.Items)) {
				ParserService.RemoveParsedFileListener(fileName, myListener);
			}
			initializing = false;
		}
		
		public IProjectContent ProjectContent {
			get {
				lock (lockObj) {
					return projectContent;
				}
			}
		}
		
		const int LoadingReferencesWorkAmount = 15; // time necessary for loading references, in relation to time for a single C# file
		
		int GetInitializationWorkAmount()
		{
			return project.Items.Count + LoadingReferencesWorkAmount;
		}
		
		void Initialize(IProgressMonitor progressMonitor)
		{
			ICollection<ProjectItem> projectItems = project.Items;
			lock (lockObj) {
				if (disposed) {
					throw new ObjectDisposedException("ParseProjectContent");
				}
			}
			ProjectService.ProjectItemAdded += OnProjectItemAdded;
			ProjectService.ProjectItemRemoved += OnProjectItemRemoved;
			double scalingFactor = 1.0 / (project.Items.Count + LoadingReferencesWorkAmount);
			using (IProgressMonitor initReferencesProgressMonitor = progressMonitor.CreateSubTask(LoadingReferencesWorkAmount * scalingFactor),
			       parseProgressMonitor = progressMonitor.CreateSubTask(projectItems.Count * scalingFactor))
			{
				var resolveReferencesTask = ResolveReferencesAsync(projectItems, initReferencesProgressMonitor);
				
				ParseFiles(projectItems, parseProgressMonitor);
				
				resolveReferencesTask.Wait();
			}
			initializing = false;
		}
		
		IEnumerable<FileName> GetFilesToParse(IEnumerable<ProjectItem> projectItems)
		{
			return (
				from p in projectItems
				where p.ItemType == ItemType.Compile && !String.IsNullOrEmpty(p.FileName)
				select FileName.Create(p.FileName));
		}
		
		void ParseFiles(ICollection<ProjectItem> projectItems, IProgressMonitor progressMonitor)
		{
			ParseableFileContentFinder finder = new ParseableFileContentFinder();
			var fileList = GetFilesToParse(projectItems).ToList();
			
			object progressLock = new object();
			double fileCountInverse = 1.0 / fileList.Count;
			Parallel.ForEach(
				fileList,
				new ParallelOptions {
					MaxDegreeOfParallelism = Environment.ProcessorCount,
					CancellationToken = progressMonitor.CancellationToken
				},
				fileName => {
					// Don't read files we don't have a parser for.
					// This avoids loading huge files (e.g. sdps) when we have no intention of parsing them.
					if (ParserService.HasParser(fileName)) {
						ParserService.AddParsedFileListener(fileName, myListener, startAsyncParse: false);
						ITextSource content = finder.Create(fileName);
						if (content != null) {
							// Parse the file on this thread so that AddParsedFileListener() does not
							// start an asynchronous parse operation.
							ParserService.ParseFile(fileName, content);
						}
					}
					lock (progressLock) {
						progressMonitor.Progress += fileCountInverse;
					}
				}
			);
		}
		
		System.Threading.Tasks.Task ResolveReferencesAsync(ICollection<ProjectItem> projectItems, IProgressMonitor progressMonitor)
		{
			return System.Threading.Tasks.Task.Factory.StartNew(
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
							var pc = AssemblyParserService.GetAssembly(FileName.Create(file), progressMonitor.CancellationToken);
							if (pc != null) {
								newReferences.Add(pc);
							}
						}
						progressMonitor.Progress += (1.0 - assemblyResolvingProgress) / assemblyFiles.Count;
					}
					lock (lockObj) {
						projectContent = projectContent.RemoveAssemblyReferences(this.references).AddAssemblyReferences(newReferences);
						this.references = newReferences.ToArray();
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
			if (e.ProjectItem.ItemType == ItemType.Compile) {
				var fileName = FileName.Create(e.ProjectItem.FileName);
				ParserService.AddParsedFileListener(fileName, myListener, startAsyncParse: true);
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
			
			if (e.ProjectItem.ItemType == ItemType.Compile) {
				ParserService.RemoveParsedFileListener(FileName.Create(e.ProjectItem.FileName), myListener);
			}
		}
	}
}
