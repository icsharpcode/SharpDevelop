// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
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
		static Dictionary<string, ParseInformation> parsings        = new Dictionary<string, ParseInformation>();
		
		public static IProjectContent CurrentProjectContent {
			get {
				if (ProjectService.CurrentProject == null || !projectContents.ContainsKey(ProjectService.CurrentProject)) {
					return new CaseSensitiveProjectContent();
				}
				return projectContents[ProjectService.CurrentProject];
			}
		}
		
		static ParserService()
		{
			try {
				parser = (ParserDescriptor[])(AddInTree.GetTreeNode("/Workspace/Parser").BuildChildItems(null)).ToArray(typeof(ParserDescriptor));
			} catch (TreePathNotFoundException) {
				parser = new ParserDescriptor[] {};
			}
			
			ProjectService.SolutionLoaded += new SolutionEventHandler(OpenCombine);
			ProjectService.SolutionClosed += new EventHandler(ProjectServiceSolutionClosed);
		}
		
		static void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			lock (projectContents) {
				projectContents.Clear();
			}
		}
		
		public static void OpenCombine(object sender, SolutionEventArgs e)
		{
			Thread t = new Thread(new ThreadStart(LoadSolutionProjects));
			t.Priority = ThreadPriority.Lowest;
			t.IsBackground = true;
			t.Start();
		}
		
		static void LoadSolutionProjects()
		{
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				try {
					IProjectContent newContent = CaseSensitiveProjectContent.Create(project);
					lock (projectContents) {
						projectContents[project] = newContent;
					}
				} catch (Exception e) {
					Console.WriteLine("Error while retrieving project contents from {0} : {1}", project, e);
				}
			}
		}
		
		public static IProjectContent GetProjectContent(IProject project)
		{
			if (projectContents.ContainsKey(project)) {
				return projectContents[project];
			}
			return null;
		}
		
		public static void StartParserThread()
		{
			Thread parserThread = new Thread(new ThreadStart(ParserUpdateThread));
			parserThread.Priority = ThreadPriority.Lowest;
			parserThread.IsBackground  = true;
			parserThread.Start();
		}
		
		static bool doneParserThread = false;
		static Dictionary<string, int> lastUpdateSize = new Dictionary<string, int>();
		
		static void ParserUpdateThread()
		{
			while (!doneParserThread) {
				try {
					if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null && WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent != null) {
						IEditable editable = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as IEditable;
						if (editable != null) {
							string fileName = null;
							
							IViewContent viewContent = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent;
							IParseableContent parseableContent = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as IParseableContent;
							
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
								}
								int hash = text.Length;
								if (lastUpdateSize[fileName] == null || (int)lastUpdateSize[fileName] != hash) {
									parseInformation = ParseFile(fileName, text, !viewContent.IsUntitled, true);
									lastUpdateSize[fileName] = hash;
									updated = true;
								}
								if (updated) {
									if (parseInformation != null && editable is IParseInformationListener) {
										((IParseInformationListener)editable).ParseInformationUpdated(parseInformation);
									}
								}
							}
						}
					}
				} catch (Exception) {
				}
				Thread.Sleep(2000);
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
		
		public static ParseInformation ParseFile(string fileName, string fileContent, bool updateCommentTags, bool fireUpdate)
		{
			IParser parser = GetParser(fileName);
			if (parser == null) {
				return null;
			}
			
			ICompilationUnitBase parserOutput = null;
			
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
				if (fileContent != null) {
					parserOutput = parser.Parse(fileName, fileContent);
				} else {
					if (!File.Exists(fileName)) {
						return null;
					}
					parserOutput = parser.Parse(fileName);
				}
				
				lock (projectContents) {
					foreach (KeyValuePair<IProject, IProjectContent> projectContent in projectContents) {
						if (projectContent.Key.IsFileInProject(fileName)) {
							if (parsings.ContainsKey(fileName)) {
								ParseInformation parseInformation = parsings[fileName];
								projectContent.Value.UpdateCompilationUnit(parseInformation.MostRecentCompilationUnit as ICompilationUnit, parserOutput as ICompilationUnit, fileName, updateCommentTags);
							} else {
								projectContent.Value.UpdateCompilationUnit(null, parserOutput as ICompilationUnit, fileName, updateCommentTags);
							}
						}
					}
				}
				return UpdateParseInformation(parserOutput as ICompilationUnit, fileName, updateCommentTags, fireUpdate);
			} catch (Exception e) {
				MessageService.ShowError(e);
			}
			return null;
		}
		
		static ParseInformation UpdateParseInformation(ICompilationUnit parserOutput, string fileName, bool updateCommentTags, bool fireEvent)
		{
			if (!parsings.ContainsKey(fileName)) {
				parsings[fileName] = new ParseInformation();
			}
			
			ParseInformation parseInformation = parsings[fileName];
			
			if (fireEvent) {
				try {
					OnParseInformationUpdated(new ParseInformationEventArgs(fileName, parseInformation, parserOutput));
				} catch (Exception e) { 
					Console.WriteLine(e); 
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
		
		public static IExpressionFinder GetExpressionFinder(string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.ExpressionFinder;
			}
			return null;
		}
		
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
				string tasklisttokens = PropertyService.Get("SharpDevelop.TaskListTokens", "HACK;TODO;UNDONE;FIXME");
				curParser.LexerTags = tasklisttokens.Split(';');
			}
			
			return curParser;
		}
		
		////////////////////////////////////
		
		public static ArrayList CtrlSpace(int caretLine, int caretColumn, string fileName)
		{
			IParser parser = GetParser(fileName);
			if (parser != null) {
				return parser.CreateResolver().CtrlSpace(caretLine, caretColumn, fileName);
			}
			return null;
		}
		
		public static ResolveResult Resolve(string expression,
		                             int caretLineNumber,
		                             int caretColumn,
		                             string fileName,
		                             string fileContent)
		{
			// added exception handling here to prevent silly parser exceptions from
			// being thrown and corrupting the textarea control
			//try {
				IParser parser = GetParser(fileName);
				if (parser != null) {
					return parser.CreateResolver().Resolve(expression, caretLineNumber, caretColumn, fileName);
				}
				return null;
			//} catch {
//				return null;
			//}
		}

		static void OnParseInformationUpdated(ParseInformationEventArgs e)
		{
			if (ParseInformationUpdated != null) {
				ParseInformationUpdated(null, e);
			}
		}

		public static event ParseInformationEventHandler ParseInformationUpdated;
	}
	
	[Serializable]
	public class DummyCompilationUnit : AbstractCompilationUnit
	{
		List<IComment> miscComments = new List<IComment>();
		List<IComment> dokuComments = new List<IComment>();
		
		public override List<IComment> MiscComments {
			get {
				return miscComments;
			}
		}
		
		public override List<IComment> DokuComments {
			get {
				return dokuComments;
			}
		}
	}
}
