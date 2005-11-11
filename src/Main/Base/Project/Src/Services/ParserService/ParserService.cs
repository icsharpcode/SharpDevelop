// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Xml;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.Core
{
	public static class ParserService
	{
		static ParserDescriptor[] parser;
		
		static Dictionary<IProject, IProjectContent> projectContents = new Dictionary<IProject, IProjectContent>();
		static Dictionary<string, ParseInformation> parsings = new Dictionary<string, ParseInformation>();
		
		public static IProjectContent CurrentProjectContent {
			[DebuggerStepThrough]
			get {
				if (forcedContent != null) return forcedContent;
				
				if (ProjectService.CurrentProject == null || !projectContents.ContainsKey(ProjectService.CurrentProject)) {
					return DefaultProjectContent;
				}
				return projectContents[ProjectService.CurrentProject];
			}
		}
		
		static IProjectContent forcedContent;
		/// <summary>
		/// Used for unit tests ONLY!!
		/// </summary>
		public static void ForceProjectContent(IProjectContent content)
		{
			forcedContent = content;
		}
		
		public static IEnumerable<IProjectContent> AllProjectContents {
			get {
				return projectContents.Values;
			}
		}
		
		
		static ParserService()
		{
			parser = (ParserDescriptor[])AddInTree.BuildItems("/Workspace/Parser", null, false).ToArray(typeof(ParserDescriptor));
			
			ProjectService.SolutionClosed += ProjectServiceSolutionClosed;
		}
		
		static void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			abortLoadSolutionProjectsThread = true;
			lock (projectContents) {
				foreach (IProjectContent content in projectContents.Values) {
					content.Dispose();
				}
				projectContents.Clear();
				parsings.Clear();
			}
		}
		
		static Thread loadSolutionProjectsThread;
		static bool   abortLoadSolutionProjectsThread;
		
		// do not use an event for this because a solution might be loaded before ParserService
		// is initialized
		internal static void OnSolutionLoaded()
		{
			if (loadSolutionProjectsThread != null) {
				if (!abortLoadSolutionProjectsThread)
					throw new InvalidOperationException("Cannot open new combine without closing old combine!");
				loadSolutionProjectsThread.Join();
			}
			loadSolutionProjectsThread = new Thread(new ThreadStart(LoadSolutionProjects));
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
			}
		}
		
		static void LoadSolutionProjectsInternal()
		{
			List<ParseProjectContent> createdContents = new List<ParseProjectContent>();
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				try {
					ParseProjectContent newContent = project.CreateProjectContent();
					lock (projectContents) {
						projectContents[project] = newContent;
					}
					createdContents.Add(newContent);
				} catch (Exception e) {
					ICSharpCode.Core.MessageService.ShowError(e, "Error while retrieving project contents from " + project);
				}
			}
			int workAmount = 0;
			foreach (ParseProjectContent newContent in createdContents) {
				if (abortLoadSolutionProjectsThread) return;
				try {
					newContent.Initialize1();
					workAmount += newContent.GetInitializationWorkAmount();
				} catch (Exception e) {
					ICSharpCode.Core.MessageService.ShowError(e, "Error while initializing project references:" + newContent);
				}
			}
			StatusBarService.ProgressMonitor.BeginTask("Parsing...", workAmount);
			foreach (ParseProjectContent newContent in createdContents) {
				if (abortLoadSolutionProjectsThread) return;
				try {
					newContent.Initialize2();
				} catch (Exception e) {
					ICSharpCode.Core.MessageService.ShowError(e, "Error while initializing project contents:" + newContent);
				}
			}
			StatusBarService.ProgressMonitor.Done();
		}
		
		static void InitAddedProject(object state)
		{
			ParseProjectContent newContent = (ParseProjectContent)state;
			newContent.Initialize1();
			newContent.Initialize2();
		}
		
		public static IProjectContent CreateProjectContentForAddedProject(IProject project)
		{
			lock (projectContents) {
				ParseProjectContent newContent = project.CreateProjectContent();
				projectContents[project] = newContent;
				ThreadPool.QueueUserWorkItem(InitAddedProject, newContent);
				return newContent;
			}
		}
		
		public static IProjectContent GetProjectContent(IProject project)
		{
			if (projectContents.ContainsKey(project)) {
				return projectContents[project];
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
		
		public static void StartParserThread()
		{
			abortParserUpdateThread = false;
			Thread parserThread = new Thread(new ThreadStart(ParserUpdateThread));
			parserThread.Priority = ThreadPriority.BelowNormal;
			parserThread.IsBackground  = true;
			parserThread.Start();
		}
		
		public static void StopParserThread()
		{
			abortParserUpdateThread = true;
		}
		
		static bool abortParserUpdateThread = false;
		
		static Dictionary<string, int> lastUpdateHash = new Dictionary<string, int>();
		
		static void ParserUpdateThread()
		{
			LoggingService.Info("ParserUpdateThread started");
			// preload mscorlib, we're going to need it anyway
			IProjectContent dummyVar = ProjectContentRegistry.Mscorlib;
			
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
		
		static object[] GetWorkbench()
		{
			IWorkbenchWindow activeWorkbenchWindow = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow;
			if (activeWorkbenchWindow == null)
				return null;
			IBaseViewContent activeViewContent = activeWorkbenchWindow.ActiveViewContent;
			if (activeViewContent == null)
				return null;
			return new object[] { activeViewContent, activeWorkbenchWindow.ViewContent };
		}
		
		static void ParserUpdateStep()
		{
			object[] workbench = (object[])WorkbenchSingleton.SafeThreadCall(typeof(ParserService), "GetWorkbench");
			if (workbench != null) {
				IEditable editable = workbench[0] as IEditable;
				if (editable != null) {
					string fileName = null;
					
					IViewContent viewContent = (IViewContent)workbench[1];
					IParseableContent parseableContent = workbench[0] as IParseableContent;
					
					//ivoko: Pls, do not throw text = parseableContent.ParseableText away. I NEED it.
					string text = null;
					if (parseableContent != null) {
						fileName = parseableContent.ParseableContentName;
						text = parseableContent.ParseableText;
					} else {
						fileName = viewContent.IsUntitled ? viewContent.UntitledName : viewContent.FileName;
					}
					
					if (!(fileName == null || fileName.Length == 0)) {
						ParseInformation parseInformation = null;
						bool updated = false;
						if (text == null) {
							text = editable.Text;
							if (text == null) return;
						}
						int hash = text.GetHashCode();
						if (!lastUpdateHash.ContainsKey(fileName) || lastUpdateHash[fileName] != hash) {
							parseInformation = ParseFile(fileName, text, !viewContent.IsUntitled, true);
							lastUpdateHash[fileName] = hash;
							updated = true;
						}
						if (updated) {
							if (parseInformation != null && editable is IParseInformationListener) {
								((IParseInformationListener)editable).ParseInformationUpdated(parseInformation);
							}
						}
						OnParserUpdateStepFinished(new ParserUpdateStepEventArgs(fileName, text, updated));
					}
				}
			}
		}
		
		public static void ParseViewContent(IViewContent viewContent)
		{
			string text = ((IEditable)viewContent).Text;
			ParseInformation parseInformation = ParseFile(viewContent.FileName, text, !viewContent.IsUntitled, true);
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
			return ParseFile(fileName, fileContent, true, true);
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
		
		static IProjectContent defaultProjectContent;
		
		public static IProjectContent DefaultProjectContent {
			get {
				if (defaultProjectContent == null) {
					lock (projectContents) {
						if (defaultProjectContent == null) {
							LoggingService.Info("Creating default project content");
							defaultProjectContent = new DefaultProjectContent();
							defaultProjectContent.ReferencedContents.Add(ProjectContentRegistry.Mscorlib);
							string[] defaultReferences = new string[] {
								"System",
								"System.Data",
								"System.Drawing",
								"System.Windows.Forms",
								"System.XML",
							};
							foreach (string defaultReference in defaultReferences) {
								ReferenceProjectItem item = new ReferenceProjectItem(null, defaultReference);
								IProjectContent pc = ProjectContentRegistry.GetProjectContentForReference(item);
								defaultProjectContent.ReferencedContents.Add(pc);
							}
						}
					}
				}
				return defaultProjectContent;
			}
		}
		
		public static ParseInformation ParseFile(string fileName, string fileContent, bool updateCommentTags, bool fireUpdate)
		{
			return ParseFile(null, fileName, fileContent, updateCommentTags, fireUpdate);
		}
		
		public static ParseInformation ParseFile(IProjectContent fileProjectContent, string fileName, string fileContent, bool updateCommentTags, bool fireUpdate)
		{
			IParser parser = GetParser(fileName);
			if (parser == null) {
				return null;
			}
			
			ICompilationUnit parserOutput = null;
			
			if (fileContent == null) {
				if (ProjectService.OpenSolution != null) {
					foreach (IProject project in ProjectService.OpenSolution.Projects) {
						if (project.IsFileInProject(fileName)) {
							fileContent = project.GetParseableFileContent(fileName);
							break;
						}
					}
				}
			}
			try {
				if (fileProjectContent == null) {
					// GetProjectContent is expensive because it compares all file names, so
					// we accept the project content as optional parameter.
					fileProjectContent = GetProjectContent(fileName);
					if (fileProjectContent == null) {
						fileProjectContent = DefaultProjectContent;
					}
				}
				
				if (fileContent != null) {
					parserOutput = parser.Parse(fileProjectContent, fileName, fileContent);
				} else {
					if (!File.Exists(fileName)) {
						return null;
					}
					parserOutput = parser.Parse(fileProjectContent, fileName);
				}
				
				if (parsings.ContainsKey(fileName)) {
					ParseInformation parseInformation = parsings[fileName];
					fileProjectContent.UpdateCompilationUnit(parseInformation.MostRecentCompilationUnit, parserOutput as ICompilationUnit, fileName, updateCommentTags);
				} else {
					fileProjectContent.UpdateCompilationUnit(null, parserOutput, fileName, updateCommentTags);
				}
				return UpdateParseInformation(parserOutput as ICompilationUnit, fileName, updateCommentTags, fireUpdate);
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			return null;
		}
		
		public static ParseInformation UpdateParseInformation(ICompilationUnit parserOutput, string fileName, bool updateCommentTags, bool fireEvent)
		{
			if (!parsings.ContainsKey(fileName)) {
				parsings[fileName] = new ParseInformation();
			}
			
			ParseInformation parseInformation = parsings[fileName];
			
			if (fireEvent) {
				try {
					OnParseInformationUpdated(new ParseInformationEventArgs(fileName, parseInformation, parserOutput));
				} catch (Exception e) {
					MessageService.ShowError(e);
				}
			}
			
			if (parserOutput.ErrorsDuringCompile) {
				parseInformation.DirtyCompilationUnit = parserOutput;
			} else {
				parseInformation.ValidCompilationUnit = parserOutput;
				parseInformation.DirtyCompilationUnit = null;
			}
			
			return parseInformation;
		}
		
		public static string GetParseableFileContent(string fileName)
		{
			IWorkbenchWindow window = FileService.GetOpenFile(fileName);
			if (window != null) {
				IViewContent viewContent = window.ViewContent;
				IEditable editable = viewContent as IEditable;
				if (editable != null) {
					return editable.Text;
				}
			}
			//string res = project.GetParseableFileContent(fileName);
			//if (res != null)
			//	return res;
			
			// load file
			Properties textEditorProperties = PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties());
			Encoding tmp = Encoding.GetEncoding(textEditorProperties.Get("Encoding", 1252));
			return ICSharpCode.TextEditor.Util.FileReader.ReadFileContent(fileName, ref tmp, tmp);
		}
		
		public static ParseInformation GetParseInformation(string fileName)
		{
			if (fileName == null || fileName.Length == 0) {
				return null;
			}
			if (!parsings.ContainsKey(fileName)) {
				return ParseFile(fileName);
			}
			return parsings[fileName];
		}
		
		public static void ClearParseInformation(string fileName)
		{
			if (fileName == null || fileName.Length == 0) {
				return;
			}
			if (parsings.ContainsKey(fileName)) {
				ParseInformation parseInfo = parsings[fileName];
				parsings.Remove(fileName);
				OnParseInformationUpdated(new ParseInformationEventArgs(fileName, parseInfo, null));
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
		
		public static ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName, string fileContent, ExpressionContext context)
		{
			IResolver resolver = CreateResolver(fileName);
			if (resolver != null) {
				return resolver.CtrlSpace(caretLine, caretColumn, fileName, fileContent, context);
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
		                                    int caretLineNumber,
		                                    int caretColumn,
		                                    string fileName,
		                                    string fileContent)
		{
			IResolver resolver = CreateResolver(fileName);
			if (resolver != null) {
				return resolver.Resolve(expressionResult, caretLineNumber, caretColumn, fileName, fileContent);
			}
			return null;
		}

		static void OnParseInformationUpdated(ParseInformationEventArgs e)
		{
			if (ParseInformationUpdated != null) {
				ParseInformationUpdated(null, e);
			}
		}
		
		public static event ParseInformationEventHandler ParseInformationUpdated;
	}
}
