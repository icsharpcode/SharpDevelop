// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextEditor.Document;

using RegistryContentPair = System.Collections.Generic.KeyValuePair<ICSharpCode.SharpDevelop.Dom.ProjectContentRegistry, ICSharpCode.SharpDevelop.Dom.IProjectContent>;

namespace ICSharpCode.SharpDevelop
{
	public static class ParserService
	{
		static IList<ParserDescriptor> parser;
		static IList<ProjectContentRegistryDescriptor> registries;
		
		static Dictionary<IProject, IProjectContent> projectContents = new Dictionary<IProject, IProjectContent>();
		static Dictionary<string, ParseInformation> parsings = new Dictionary<string, ParseInformation>(StringComparer.OrdinalIgnoreCase);
		static ProjectContentRegistry defaultProjectContentRegistry = new ProjectContentRegistry();
		
		static string domPersistencePath;
		
		internal static void InitializeParserService()
		{
			if (parser == null) {
				parser = AddInTree.BuildItems<ParserDescriptor>("/Workspace/Parser", null, false);
				registries = AddInTree.BuildItems<ProjectContentRegistryDescriptor>("/Workspace/ProjectContentRegistry", null, false);
				
				if (!string.IsNullOrEmpty(domPersistencePath)) {
					Directory.CreateDirectory(domPersistencePath);
					defaultProjectContentRegistry.ActivatePersistence(domPersistencePath);
				}
				ProjectService.SolutionClosed += ProjectServiceSolutionClosed;
			}
		}
		
		/// <summary>
		/// Gets/Sets the cache directory used for DOM persistence.
		/// </summary>
		public static string DomPersistencePath {
			get {
				return domPersistencePath;
			}
			set {
				if (parser != null)
					throw new InvalidOperationException("Cannot set DomPersistencePath after ParserService was initialized");
				domPersistencePath = value;
			}
		}
		
		public static ProjectContentRegistry DefaultProjectContentRegistry {
			get {
				return defaultProjectContentRegistry;
			}
		}
		
		public static IProjectContent CurrentProjectContent {
			[DebuggerStepThrough]
			get {
				if (ProjectService.CurrentProject == null || !projectContents.ContainsKey(ProjectService.CurrentProject)) {
					return DefaultProjectContent;
				}
				return projectContents[ProjectService.CurrentProject];
			}
		}
		
		/// <summary>
		/// Gets the list of project contents of all open projects. Does not include assembly project contents.
		/// </summary>
		public static IEnumerable<IProjectContent> AllProjectContents {
			get {
				return projectContents.Values;
			}
		}
		
		static void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			abortLoadSolutionProjectsThread = true;
			
			lock (reParse1) { // clear queue of reparse thread
				reParse1.Clear();
				reParse2.Clear();
			}
			lock (projectContents) {
				foreach (IProjectContent content in projectContents.Values) {
					content.Dispose();
				}
				projectContents.Clear();
			}
			lock (parsings) {
				parsings.Clear();
			}
			lock (parseQueue) {
				parseQueue.Clear();
			}
			lastUpdateHash.Clear();
		}
		
		static Thread loadSolutionProjectsThread;
		static bool   abortLoadSolutionProjectsThread;
		
		// do not use an event for this because a solution might be loaded before ParserService
		// is initialized
		internal static void OnSolutionLoaded()
		{
			if (loadSolutionProjectsThread != null) {
				if (!abortLoadSolutionProjectsThread)
					throw new InvalidOperationException("Cannot open new solution without closing old solution!");
				if (!loadSolutionProjectsThread.Join(50)) {
					// loadSolutionProjects might be waiting for main thread, so give it
					// a chance to complete safethread calls by putting this method at
					// the end of the invoking queue
					WorkbenchSingleton.SafeThreadAsyncCall(OnSolutionLoaded);
					return;
				}
			}
			loadSolutionProjectsThread = new Thread(new ThreadStart(LoadSolutionProjects));
			loadSolutionProjectsThread.SetApartmentState(ApartmentState.STA); // allow loadSolutionProjects thread access to MSBuild
			loadSolutionProjectsThread.Name = "loadSolutionProjects";
			loadSolutionProjectsThread.Priority = ThreadPriority.BelowNormal;
			loadSolutionProjectsThread.IsBackground = true;
			loadSolutionProjectsThread.Start();
		}
		
		public static bool LoadSolutionProjectsThreadRunning {
			get {
				return loadSolutionProjectsThread != null;
			}
		}
		
		static void LoadSolutionProjects()
		{
			try {
				abortLoadSolutionProjectsThread = false;
				LoggingService.Info("Start LoadSolutionProjects thread");
				LoadSolutionProjectsInternal();
			} finally {
				LoggingService.Info("LoadSolutionProjects thread ended");
				loadSolutionProjectsThread = null;
				OnLoadSolutionProjectsThreadEnded(EventArgs.Empty);
			}
		}
		
		static void LoadSolutionProjectsInternal()
		{
			IProgressMonitor progressMonitor = StatusBarService.CreateProgressMonitor();
			List<ParseProjectContent> createdContents = new List<ParseProjectContent>();
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				try {
					ParseProjectContent newContent = project.CreateProjectContent();
					if (newContent != null) {
						lock (projectContents) {
							projectContents[project] = newContent;
						}
						createdContents.Add(newContent);
					}
				} catch (Exception e) {
					MessageService.ShowError(e, "Error while retrieving project contents from " + project);
				}
			}
			WorkbenchSingleton.SafeThreadAsyncCall(ProjectService.ParserServiceCreatedProjectContents);
			try {
				// multiply Count with 2 so that the progress bar is only at 50% when references are done
				progressMonitor.BeginTask("Loading references...", createdContents.Count * 2, false);
				int workAmount = 0;
				for (int i = 0; i < createdContents.Count; i++) {
					if (abortLoadSolutionProjectsThread) return;
					ParseProjectContent newContent = createdContents[i];
					progressMonitor.WorkDone = i;
					try {
						newContent.Initialize1(progressMonitor);
						workAmount += newContent.GetInitializationWorkAmount();
					} catch (Exception e) {
						MessageService.ShowError(e, "Error while initializing project references:" + newContent);
					}
				}
				// multiply workamount with two and start at workAmount so that the progress bar continues
				// from 50% towards 100%.
				progressMonitor.BeginTask("${res:ICSharpCode.SharpDevelop.Internal.ParserService.Parsing}...", workAmount * 2, false);
				progressMonitor.WorkDone = workAmount;
				foreach (ParseProjectContent newContent in createdContents) {
					if (abortLoadSolutionProjectsThread) return;
					try {
						newContent.Initialize2(progressMonitor);
					} catch (Exception e) {
						MessageService.ShowError(e, "Error while initializing project contents:" + newContent);
					}
				}
			} finally {
				progressMonitor.Done();
			}
		}
		
		static void InitAddedProject(object state)
		{
			ParseProjectContent newContent = (ParseProjectContent)state;
			IProgressMonitor progressMonitor = StatusBarService.CreateProgressMonitor();
			newContent.Initialize1(progressMonitor);
			progressMonitor.BeginTask("${res:ICSharpCode.SharpDevelop.Internal.ParserService.Parsing}...", newContent.GetInitializationWorkAmount(), false);
			newContent.Initialize2(progressMonitor);
			progressMonitor.Done();
		}
		
		#region Reparse projects
		// queue of projects waiting to reparse references
		static Queue<ParseProjectContent> reParse1 = new Queue<ParseProjectContent>();
		
		// queue of projects waiting to reparse code
		static Queue<ParseProjectContent> reParse2 = new Queue<ParseProjectContent>();
		static Thread reParseThread;
		
		static void ReparseProjects()
		{
			LoggingService.Info("reParse thread started");
			Thread.Sleep(100); // enable main thread to fill the queues completely
			try {
				ReparseProjectsInternal();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		static void ReparseProjectsInternal()
		{
			bool parsing = false;
			ParseProjectContent job;
			IProgressMonitor progressMonitor = StatusBarService.CreateProgressMonitor();
			
			while (true) {
				// get next job
				lock (reParse1) {
					if (reParse1.Count > 0) {
						if (parsing) {
							progressMonitor.Done();
						}
						parsing = false;
						job = reParse1.Dequeue();
					} else if (reParse2.Count > 0) {
						if (!parsing) {
							int workAmount = 0;
							foreach (ParseProjectContent ppc in reParse2) {
								workAmount += ppc.GetInitializationWorkAmount();
							}
							progressMonitor.BeginTask("${res:ICSharpCode.SharpDevelop.Internal.ParserService.Parsing}...", workAmount, false);
						}
						parsing = true;
						job = reParse2.Dequeue();
					} else {
						// all jobs done
						reParseThread = null;
						if (parsing) {
							progressMonitor.Done();
						}
						LoggingService.Info("reParse thread finished all jobs");
						return;
					}
				}
				
				// execute job
				if (parsing) {
					LoggingService.Info("reparsing code for " + job.Project);
					job.ReInitialize2(progressMonitor);
				} else {
					LoggingService.Debug("reloading references for " + job.Project);
					job.ReInitialize1(progressMonitor);
				}
			}
		}
		
		public static void Reparse(IProject project, bool initReferences, bool parseCode)
		{
			ParseProjectContent pc = GetProjectContent(project) as ParseProjectContent;
			if (pc != null) {
				lock (reParse1) {
					if (initReferences && !reParse1.Contains(pc)) {
						LoggingService.Debug("Enqueue for reinitializing references: " + project);
						reParse1.Enqueue(pc);
					}
					if (parseCode && !reParse2.Contains(pc)) {
						LoggingService.Debug("Enqueue for reparsing code: " + project);
						reParse2.Enqueue(pc);
					}
					if (reParseThread == null) {
						LoggingService.Info("Starting reParse thread");
						reParseThread = new Thread(new ThreadStart(ReparseProjects));
						reParseThread.SetApartmentState(ApartmentState.STA); // allow reParseThread access to MSBuild
						reParseThread.Name = "reParse";
						reParseThread.Priority = ThreadPriority.BelowNormal;
						reParseThread.IsBackground = true;
						reParseThread.Start();
					}
				}
			}
		}
		#endregion
		
		/// <remarks>Can return null.</remarks>
		internal static IProjectContent CreateProjectContentForAddedProject(IProject project)
		{
			lock (projectContents) {
				ParseProjectContent newContent = project.CreateProjectContent();
				if (newContent != null) {
					projectContents[project] = newContent;
					ThreadPool.QueueUserWorkItem(InitAddedProject, newContent);
				}
				return newContent;
			}
		}
		
		internal static void RemoveProjectContentForRemovedProject(IProject project)
		{
			lock (projectContents) {
				projectContents.Remove(project);
			}
		}
		
		public static IProjectContent GetProjectContent(IProject project)
		{
			lock (projectContents) {
				if (projectContents.ContainsKey(project)) {
					return projectContents[project];
				}
			}
			return null;
		}
		
		static Queue<KeyValuePair<string, string>> parseQueue = new Queue<KeyValuePair<string, string>>();
		
		static void ParseQueue()
		{
			while (true) {
				KeyValuePair<string, string> entry;
				lock (parseQueue) {
					if (parseQueue.Count == 0)
						return;
					entry = parseQueue.Dequeue();
				}
				ParseFile(entry.Key, entry.Value);
			}
		}
		
		public static void EnqueueForParsing(string fileName)
		{
			EnqueueForParsing(fileName, GetParseableFileContent(fileName));
		}
		
		public static void EnqueueForParsing(string fileName, string fileContent)
		{
			lock (parseQueue) {
				parseQueue.Enqueue(new KeyValuePair<string, string>(fileName, fileContent));
			}
		}
		
		public static void StartAsyncParse(string fileName, string fileContent)
		{
			ThreadPool.QueueUserWorkItem(
				delegate {
					ParseFile(fileName, fileContent);
				});
		}
		
		public static void StartParserThread()
		{
			abortParserUpdateThread = false;
			Thread parserThread = new Thread(new ThreadStart(ParserUpdateThread));
			parserThread.Name = "parser";
			parserThread.Priority = ThreadPriority.BelowNormal;
			parserThread.IsBackground  = true;
			parserThread.Start();
		}
		
		public static void StopParserThread()
		{
			abortParserUpdateThread = true;
		}
		
		static volatile bool abortParserUpdateThread = false;
		
		static Dictionary<string, int> lastUpdateHash = new Dictionary<string, int>();
		
		static void ParserUpdateThread()
		{
			LoggingService.Info("ParserUpdateThread started");
			Thread.Sleep(750);
			
			// preload mscorlib, we're going to need it probably
			IProjectContent dummyVar = defaultProjectContentRegistry.Mscorlib;
			
			while (!abortParserUpdateThread) {
				try {
					ParseQueue();
					ParserUpdateStep();
				} catch (Exception e) {
					ICSharpCode.Core.MessageService.ShowError(e);
					
					// don't fire an exception every 2 seconds at the user, give him at least
					// time to read the first :-)
					Thread.Sleep(10000);
				}
				Thread.Sleep(2000);
			}
			LoggingService.Info("ParserUpdateThread stopped");
		}
		
		public static void ParseCurrentViewContent()
		{
			ParserUpdateStep();
		}
		
		static void ParserUpdateStep()
		{
			IViewContent activeViewContent = null;
			string fileName = null;
			bool isUntitled = false;
			try {
				WorkbenchSingleton.SafeThreadCall(
					delegate {
						try {
							activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;
							if (activeViewContent != null && activeViewContent.PrimaryFile != null) {
								fileName = activeViewContent.PrimaryFileName;
								isUntitled = activeViewContent.PrimaryFile.IsUntitled;
							}
						} catch (Exception ex) {
							MessageService.ShowError(ex.ToString());
						}
					});
			} catch (InvalidOperationException ex) { // includes ObjectDisposedException
				// maybe workbench has been disposed while waiting for the SafeThreadCall
				// can occur after workbench unload or after aborting SharpDevelop with
				// Application.Exit()
				LoggingService.Warn("InvalidOperationException while trying to invoke GetActiveViewContent() " + ex);
				return; // abort this thread
			}
			IEditable editable = activeViewContent as IEditable;
			if (editable != null) {
				string text = null;
				
				if (!(fileName == null || fileName.Length == 0)) {
					ParseInformation parseInformation = null;
					bool updated = false;
					if (text == null) {
						text = editable.Text;
						if (text == null) return;
					}
					int hash = text.GetHashCode();
					if (!lastUpdateHash.ContainsKey(fileName) || lastUpdateHash[fileName] != hash) {
						parseInformation = ParseFile(fileName, text, !isUntitled);
						lastUpdateHash[fileName] = hash;
						updated = true;
					}
					if (updated) {
						if (parseInformation != null && editable is IParseInformationListener) {
							((IParseInformationListener)editable).ParseInformationUpdated(parseInformation);
						}
					}
					OnParserUpdateStepFinished(new ParserUpdateStepEventArgs(fileName, text, updated, parseInformation));
				}
			}
		}
		
		public static void ParseViewContent(IViewContent viewContent)
		{
			string text = ((IEditable)viewContent).Text;
			ParseInformation parseInformation = ParseFile(viewContent.PrimaryFileName,
			                                              text, !viewContent.PrimaryFile.IsUntitled);
			if (parseInformation != null && viewContent is IParseInformationListener) {
				((IParseInformationListener)viewContent).ParseInformationUpdated(parseInformation);
			}
		}
		
		/// <summary>
		/// <para>This event is called every two seconds. It is called directly after the parser has updated the
		/// project content and it is called after the parser noticed that there is nothing to update.</para>
		/// <para><b>WARNING: This event is called on the parser thread - You need to use Invoke if you do
		/// anything in your event handler that could touch the GUI.</b></para>
		/// </summary>
		public static event ParserUpdateStepEventHandler ParserUpdateStepFinished;
		
		static void OnParserUpdateStepFinished(ParserUpdateStepEventArgs e)
		{
			if (ParserUpdateStepFinished != null) {
				ParserUpdateStepFinished(typeof(ParserService), e);
			}
		}
		
		public static ParseInformation ParseFile(string fileName)
		{
			return ParseFile(fileName, null);
		}
		
		public static ParseInformation ParseFile(string fileName, string fileContent)
		{
			return ParseFile(fileName, fileContent, true);
		}
		
		static IProjectContent GetProjectContent(string fileName)
		{
			lock (projectContents) {
				foreach (KeyValuePair<IProject, IProjectContent> projectContent in projectContents) {
					if (projectContent.Key.IsFileInProject(fileName)) {
						return projectContent.Value;
					}
				}
			}
			return null;
		}
		
		static DefaultProjectContent defaultProjectContent;
		
		public static IProjectContent DefaultProjectContent {
			get {
				if (defaultProjectContent == null) {
					lock (projectContents) {
						if (defaultProjectContent == null) {
							CreateDefaultProjectContent();
						}
					}
				}
				return defaultProjectContent;
			}
		}
		
		static void CreateDefaultProjectContent()
		{
			LoggingService.Info("Creating default project content");
			//LoggingService.Debug("Stacktrace is:\n" + Environment.StackTrace);
			defaultProjectContent = new DefaultProjectContent();
			defaultProjectContent.AddReferencedContent(defaultProjectContentRegistry.Mscorlib);
			Thread t = new Thread(new ThreadStart(CreateDefaultProjectContentReferences));
			t.IsBackground = true;
			t.Priority = ThreadPriority.BelowNormal;
			t.Name = "CreateDefaultPC";
			t.Start();
		}
		
		static void CreateDefaultProjectContentReferences()
		{
			IList<string> defaultReferences = AddInTree.BuildItems<string>("/SharpDevelop/Services/ParserService/SingleFileGacReferences", null, false);
			foreach (string defaultReference in defaultReferences) {
				ReferenceProjectItem item = new ReferenceProjectItem(null, defaultReference);
				defaultProjectContent.AddReferencedContent(ParserService.GetProjectContentForReference(item));
			}
			if (WorkbenchSingleton.Workbench != null) {
				WorkbenchSingleton.Workbench.ActiveViewContentChanged += delegate {
					if (WorkbenchSingleton.Workbench.ActiveViewContent != null) {
						string file = WorkbenchSingleton.Workbench.ActiveViewContent.PrimaryFileName;
						if (file != null) {
							IParser parser = GetParser(file);
							if (parser != null && parser.Language != null) {
								defaultProjectContent.Language = parser.Language;
								defaultProjectContent.DefaultImports = parser.Language.CreateDefaultImports(defaultProjectContent);
							}
						}
					}
				};
			}
		}
		
		public static ParseInformation ParseFile(string fileName, string fileContent, bool updateCommentTags)
		{
			return ParseFile(null, fileName, fileContent, updateCommentTags);
		}
		
		public static ParseInformation ParseFile(IProjectContent fileProjectContent, string fileName, string fileContent, bool updateCommentTags)
		{
			if (fileName == null) throw new ArgumentNullException("fileName");
			
			IParser parser = GetParser(fileName);
			if (parser == null) {
				return null;
			}
			
			ICompilationUnit parserOutput = null;
			
			try {
				if (fileProjectContent == null) {
					// GetProjectContent is expensive because it compares all file names, so
					// we accept the project content as optional parameter.
					fileProjectContent = GetProjectContent(fileName);
					if (fileProjectContent == null) {
						fileProjectContent = DefaultProjectContent;
					}
				}
				
				if (fileContent == null) {
					if (!File.Exists(fileName)) {
						return null;
					}
					fileContent = GetParseableFileContent(fileName);
				}
				parserOutput = parser.Parse(fileProjectContent, fileName, fileContent);
				parserOutput.Freeze();
				
				ParseInformation parseInformation;
				lock (parsings) {
					if (!parsings.TryGetValue(fileName, out parseInformation)) {
						parsings[fileName] = parseInformation = new ParseInformation();
					}
				}
				ICompilationUnit oldUnit = parseInformation.MostRecentCompilationUnit;
				fileProjectContent.UpdateCompilationUnit(oldUnit, parserOutput, fileName);
				parseInformation.SetCompilationUnit(parserOutput);
				if (updateCommentTags) {
					TaskService.UpdateCommentTags(fileName, parserOutput.TagComments);
				}
				try {
					OnParseInformationUpdated(new ParseInformationEventArgs(fileName, fileProjectContent, oldUnit, parserOutput));
				} catch (Exception e) {
					MessageService.ShowError(e);
				}
				return parseInformation;
			} catch (Exception e) {
				MessageService.ShowError(e, "Error parsing " + fileName);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the content of the file using encoding auto-detection (or DefaultFileEncoding, if that fails).
		/// If the file is already open, gets the text in the opened view content.
		/// </summary>
		public static string GetParseableFileContent(string fileName)
		{
			IViewContent viewContent = FileService.GetOpenFile(fileName);
			IEditable editable = viewContent as IEditable;
			if (editable != null) {
				return editable.Text;
			}
			//string res = project.GetParseableFileContent(fileName);
			//if (res != null)
			//	return res;
			
			OpenedFile file = FileService.GetOpenedFile(fileName);
			if (file != null) {
				IFileDocumentProvider p = file.CurrentView as IFileDocumentProvider;
				if (p != null) {
					IDocument document = p.GetDocumentForFile(file);
					if (document != null) {
						return document.TextContent;
					}
				}
				
				using(Stream s = file.OpenRead()) {
					// load file
					Encoding encoding = DefaultFileEncoding;
					return ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(s, ref encoding);
				}
			}
			
			// load file
			return ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(fileName, DefaultFileEncoding);
		}
		
		public static Encoding DefaultFileEncoding {
			get {
				return DefaultEditor.Gui.Editor.SharpDevelopTextEditorProperties.Instance.Encoding;
			}
		}
		
		public static ParseInformation GetParseInformation(string fileName)
		{
			if (fileName == null || fileName.Length == 0) {
				return null;
			}
			ParseInformation parseInfo;
			lock (parsings) {
				if (parsings.TryGetValue(fileName, out parseInfo))
					return parseInfo;
			}
			return ParseFile(fileName);
		}
		
		/// <summary>
		/// Registers a compilation unit in the parser service.
		/// Does not fire the OnParseInformationUpdated event, please use this for unit tests only!
		/// </summary>
		public static ParseInformation RegisterParseInformation(string fileName, ICompilationUnit cu)
		{
			ParseInformation parseInformation;
			lock (parsings) {
				if (!parsings.TryGetValue(fileName, out parseInformation)) {
					parsings[fileName] = parseInformation = new ParseInformation();
				}
			}
			parseInformation.SetCompilationUnit(cu);
			return parseInformation;
		}
		
		public static void ClearParseInformation(string fileName)
		{
			if (fileName == null || fileName.Length == 0) {
				return;
			}
			LoggingService.Info("ClearParseInformation: " + fileName);
			ParseInformation parseInfo;
			lock (parsings) {
				if (parsings.TryGetValue(fileName, out parseInfo))
					parsings.Remove(fileName);
				else
					return;
			}
			ICompilationUnit oldUnit = parseInfo.MostRecentCompilationUnit;
			if (oldUnit != null) {
				IProjectContent pc = parseInfo.MostRecentCompilationUnit.ProjectContent;
				pc.RemoveCompilationUnit(oldUnit);
				try {
					OnParseInformationUpdated(new ParseInformationEventArgs(fileName, pc, oldUnit, null));
				} catch (Exception e) {
					MessageService.ShowError(e);
				}
			}
		}
		
		public static IExpressionFinder GetExpressionFinder(string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.CreateExpressionFinder(fileName);
			}
			return null;
		}
		
		public static readonly string[] DefaultTaskListTokens = {"HACK", "TODO", "UNDONE", "FIXME"};
		
		public static IParser GetParser(string fileName)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			IParser curParser = null;
			foreach (ParserDescriptor descriptor in parser) {
				if (descriptor.CanParse(fileName)) {
					curParser = descriptor.Parser;
					break;
				}
			}
			
			if (curParser != null) {
				curParser.LexerTags = PropertyService.Get("SharpDevelop.TaskListTokens", DefaultTaskListTokens);
			}
			
			return curParser;
		}
		
		////////////////////////////////////
		
		public static ArrayList CtrlSpace(int caretLine, int caretColumn,
		                                  string fileName, string fileContent, ExpressionContext context)
		{
			IResolver resolver = CreateResolver(fileName);
			if (resolver != null) {
				return resolver.CtrlSpace(caretLine, caretColumn, GetParseInformation(fileName), fileContent, context);
			}
			return null;
		}
		
		public static IResolver CreateResolver(string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.CreateResolver();
			}
			return null;
		}
		
		public static ResolveResult Resolve(ExpressionResult expressionResult,
		                                    int caretLineNumber, int caretColumn,
		                                    string fileName, string fileContent)
		{
			if (expressionResult.Region.IsEmpty) {
				expressionResult.Region = new DomRegion(caretLineNumber, caretColumn);
			}
			IResolver resolver = CreateResolver(fileName);
			if (resolver != null) {
				ParseInformation parseInfo = GetParseInformation(fileName);
				return resolver.Resolve(expressionResult, parseInfo, fileContent);
			}
			return null;
		}

		static void OnParseInformationUpdated(ParseInformationEventArgs e)
		{
			ParseInformationEventHandler handler = ParseInformationUpdated;
			if (handler != null) {
				handler(null, e);
			}
		}
		
		static void OnLoadSolutionProjectsThreadEnded(EventArgs e)
		{
			EventHandler handler = LoadSolutionProjectsThreadEnded;
			if (handler != null) {
				handler(null, e);
			}
		}
		
		public static event ParseInformationEventHandler ParseInformationUpdated;
		public static event EventHandler LoadSolutionProjectsThreadEnded;
		
		public static ProjectContentRegistry GetRegistryForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem || item.Project == null) {
				return defaultProjectContentRegistry;
			}
			foreach (ProjectContentRegistryDescriptor registry in registries) {
				if (registry.UseRegistryForProject(item.Project)) {
					ProjectContentRegistry r = registry.Registry;
					if (r != null) {
						return r;
					} else {
						return defaultProjectContentRegistry; // fallback when registry class not found
					}
				}
			}
			return defaultProjectContentRegistry;
		}
		
		public static IProjectContent GetExistingProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				if (((ProjectReferenceProjectItem)item).ReferencedProject == null)
				{
					return null;
				}
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			return GetRegistryForReference(item).GetExistingProjectContent(item.FileName);
		}
		
		public static IProjectContent GetProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				if (((ProjectReferenceProjectItem)item).ReferencedProject == null)
				{
					return null;
				}
				return ParserService.GetProjectContent(((ProjectReferenceProjectItem)item).ReferencedProject);
			}
			return GetRegistryForReference(item).GetProjectContentForReference(item.Include, item.FileName);
		}
		
		/// <summary>
		/// Refreshes the project content for the specified reference if required.
		/// This method does nothing if the reference is not an assembly reference, is not loaded or already is up-to-date.
		/// </summary>
		public static void RefreshProjectContentForReference(ReferenceProjectItem item)
		{
			if (item is ProjectReferenceProjectItem) {
				return;
			}
			ProjectContentRegistry registry = GetRegistryForReference(item);
			registry.RunLocked(
				delegate {
					IProjectContent rpc = GetExistingProjectContentForReference(item);
					if (rpc == null) {
						LoggingService.Debug("RefreshProjectContentForReference: not refreshing (rpc==null) " + item.FileName);
						return;
					}
					if (rpc.IsUpToDate) {
						LoggingService.Debug("RefreshProjectContentForReference: not refreshing (rpc.IsUpToDate) " + item.FileName);
						return;
					}
					LoggingService.Debug("RefreshProjectContentForReference " + item.FileName);
					
					HashSet<IProject> projectsToRefresh = new HashSet<IProject>();
					HashSet<IProjectContent> unloadedReferenceContents = new HashSet<IProjectContent>();
					UnloadReferencedContent(projectsToRefresh, unloadedReferenceContents, registry, rpc);
					
					foreach (IProject p in projectsToRefresh) {
						Reparse(p, true, false);
					}
				});
		}
		
		static void UnloadReferencedContent(HashSet<IProject> projectsToRefresh, HashSet<IProjectContent> unloadedReferenceContents, ProjectContentRegistry referencedContentRegistry, IProjectContent referencedContent)
		{
			LoggingService.Debug("Unload referenced content " + referencedContent);
			
			List<RegistryContentPair> otherContentsToUnload = new List<RegistryContentPair>();
			foreach (ProjectContentRegistryDescriptor registry in registries) {
				if (registry.IsRegistryLoaded) {
					foreach (IProjectContent pc in registry.Registry.GetLoadedProjectContents()) {
						if (pc.ReferencedContents.Contains(referencedContent)) {
							if (unloadedReferenceContents.Add(pc)) {
								LoggingService.Debug("Mark dependent content for unloading " + pc);
								otherContentsToUnload.Add(new RegistryContentPair(registry.Registry, pc));
							}
						}
					}
				}
			}
			
			foreach (IProjectContent pc in ParserService.AllProjectContents) {
				IProject project = (IProject)pc.Project;
				if (projectsToRefresh.Contains(project))
					continue;
				if (pc.ReferencedContents.Remove(referencedContent)) {
					LoggingService.Debug("UnloadReferencedContent: Mark project for reparsing " + project.Name);
					projectsToRefresh.Add(project);
				}
			}
			
			foreach (RegistryContentPair pair in otherContentsToUnload) {
				UnloadReferencedContent(projectsToRefresh, unloadedReferenceContents, pair.Key, pair.Value);
			}
			
			referencedContentRegistry.UnloadProjectContent(referencedContent);
		}
	}
}
