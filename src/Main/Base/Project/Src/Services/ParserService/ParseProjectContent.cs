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
	public class ParseProjectContent : SimpleProjectContent, IDisposable
	{
		readonly MSBuildBasedProject project;
		readonly object lockObj = new object();
		volatile ITypeResolveContext typeResolveContext;
		bool initializing;
		bool disposed;
		
		public ParseProjectContent(MSBuildBasedProject project)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.typeResolveContext = MinimalResolveContext.Instance;
			
			this.initializing = true;
			LoadSolutionProjects.AddJob(Initialize, "Loading " + project.Name + "...", GetInitializationWorkAmount());
		}
		
		public void Dispose()
		{
			lock (lockObj) {
				ProjectService.ProjectItemAdded   -= OnProjectItemAdded;
				ProjectService.ProjectItemRemoved -= OnProjectItemRemoved;
				disposed = true;
			}
			initializing = false;
		}
		
		public ITypeResolveContext TypeResolveContext {
			get { return typeResolveContext; }
		}
		
		public override string AssemblyName {
			get { return project.AssemblyName; }
		}
		
		public override string ToString()
		{
			return string.Format("[{0}: {1}]", GetType().Name, project.Name);
		}
		
		const int LoadingReferencesWorkAmount = 15; // in number of C# files
		
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
				ProjectService.ProjectItemAdded += OnProjectItemAdded;
				ProjectService.ProjectItemRemoved += OnProjectItemRemoved;
			}
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
		
		void ParseFiles(ICollection<ProjectItem> projectItems, IProgressMonitor progressMonitor)
		{
			ParseableFileContentFinder finder = new ParseableFileContentFinder();
			var fileContents = (
				from p in projectItems.AsParallel().WithCancellation(progressMonitor.CancellationToken)
				where !ItemType.NonFileItemTypes.Contains(p.ItemType) && !String.IsNullOrEmpty(p.FileName)
				select FileName.Create(p.FileName)
			).ToList();
			
			object progressLock = new object();
			double fileCountInverse = 1.0 / fileContents.Count;
			Parallel.ForEach(
				fileContents,
				new ParallelOptions {
					MaxDegreeOfParallelism = Environment.ProcessorCount,
					CancellationToken = progressMonitor.CancellationToken
				},
				fileName => {
					// Don't read files we don't have a parser for.
					// This avoids loading huge files (e.g. sdps) when we have no intention of parsing them.
					if (ParserService.GetParser(fileName) != null) {
						ITextSource content = finder.Create(fileName);
						if (content != null)
							ParserService.ParseFile(this, fileName, content);
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
					project.ResolveAssemblyReferences();
					const double assemblyResolvingProgress = 0.3;
					progressMonitor.Progress += assemblyResolvingProgress;
					progressMonitor.CancellationToken.ThrowIfCancellationRequested();
					
					List<string> assemblyFiles = new List<string>();
					string mscorlib = project.MscorlibPath;
					if (mscorlib != null)
						assemblyFiles.Add(mscorlib);
					
					List<ITypeResolveContext> contexts = new List<ITypeResolveContext>();
					contexts.Add(this);
					foreach (ProjectItem item in projectItems) {
						ReferenceProjectItem reference = item as ReferenceProjectItem;
						if (reference != null) {
							if (ItemType.ReferenceItemTypes.Contains(reference.ItemType)) {
								ProjectReferenceProjectItem projectReference = reference as ProjectReferenceProjectItem;
								if (projectReference != null) {
									IProject p = projectReference.ReferencedProject;
									if (p != null)
										contexts.Add(p.ProjectContent);
								} else {
									assemblyFiles.Add(reference.FileName);
								}
							}
						}
					}
					
					foreach (string file in assemblyFiles) {
						progressMonitor.CancellationToken.ThrowIfCancellationRequested();
						if (File.Exists(file)) {
							var pc = AssemblyParserService.GetAssembly(FileName.Create(file), progressMonitor.CancellationToken);
							if (pc != null) {
								contexts.Add(pc);
							}
						}
						progressMonitor.Progress += (1.0 - assemblyResolvingProgress) / assemblyFiles.Count;
					}
					this.typeResolveContext = new CompositeTypeResolveContext(contexts);
				}, progressMonitor.CancellationToken);
		}
		
		// ensure that com references are built serially because we cannot invoke multiple instances of MSBuild
		static Queue<System.Windows.Forms.MethodInvoker> callAfterAddComReference = new Queue<System.Windows.Forms.MethodInvoker>();
		static bool buildingComReference;
		
		void OnProjectItemAdded(object sender, ProjectItemEventArgs e)
		{
			if (e.Project != project) return;
			
			ReferenceProjectItem reference = e.ProjectItem as ReferenceProjectItem;
			if (reference != null) {
				if (reference.ItemType == ItemType.COMReference) {
					System.Windows.Forms.MethodInvoker action = delegate {
						// Compile project to ensure interop library is generated
						project.Save(); // project is not yet saved when ItemAdded fires, so save it here
						TaskService.BuildMessageViewCategory.AppendText("\n${res:MainWindow.CompilerMessages.CreatingCOMInteropAssembly}\n");
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
			if (e.ProjectItem.ItemType == ItemType.Import) {
				throw new NotImplementedException();
				//UpdateDefaultImports(project.Items);
			} else if (e.ProjectItem.ItemType == ItemType.Compile) {
				if (System.IO.File.Exists(e.ProjectItem.FileName)) {
					ParserService.ParseFileAsync(FileName.Create(e.ProjectItem.FileName));
				}
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
			
			if (e.ProjectItem.ItemType == ItemType.Import) {
				throw new NotImplementedException();
				//UpdateDefaultImports(project.Items);
			} else if (e.ProjectItem.ItemType == ItemType.Compile) {
				ParserService.ClearParseInformation(FileName.Create(e.ProjectItem.FileName));
			}
		}
		
		/*
		int languageDefaultImportCount = -1;
		
		void UpdateDefaultImports(ICollection<ProjectItem> items)
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
		}*/
	}
}
