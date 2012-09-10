// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Registered in /SharpDevelop/CustomTools/
	/// </summary>
	public interface ICustomTool
	{
		void GenerateCode(FileProjectItem item, CustomToolContext context);
	}
	
	#region CustomToolContext
	/// <summary>
	/// Provides ProgressMonitor and MessageView to custom tools.
	/// Also provides helper methods that are useful for custom tools.
	/// </summary>
	public sealed class CustomToolContext
	{
		IProject project;
		IProgressMonitor progressMonitor;
		internal bool RunningSeparateThread;
		
		public CustomToolContext(IProject project)
			: this(project, new DummyProgressMonitor())
		{
		}
		
		public CustomToolContext(IProject project, IProgressMonitor progressMonitor)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			this.project = project;
			this.ProgressMonitor = progressMonitor;
		}
		
		/// <summary>
		/// Returns the project the custom tool is being run for. The IProject interface
		/// is not thread-safe!
		/// </summary>
		public IProject Project {
			get { return project; }
		}
		
		public string OutputNamespace { get; set; }
		
		/// <summary>
		/// Runs a method asynchronously. Prevents another CustomTool invocation
		/// on the same file while action is running.
		/// </summary>
		public void RunAsync(Action action)
		{
			RunningSeparateThread = true;
			System.Threading.ThreadPool.QueueUserWorkItem(
				delegate {
					try {
						action();
					} catch (Exception ex) {
						MessageService.ShowException(ex);
					} finally {
						CustomToolsService.NotifyAsyncFinish(this);
					}
				});
		}
		
		static MessageViewCategory customToolMessageView;
		
		internal static MessageViewCategory StaticMessageView {
			get {
				if (customToolMessageView == null) {
					MessageViewCategory.Create(ref customToolMessageView, "Custom Tool");
				}
				return customToolMessageView;
			}
		}
		
		/// <summary>
		/// Returns the message view where custom tools can write to. This member is thread-safe.
		/// </summary>
		public MessageViewCategory MessageView {
			get {
				return StaticMessageView;
			}
		}
		
		public string GetOutputFileName(FileProjectItem baseItem, string additionalExtension)
		{
			return GetOutputFileName(baseItem, additionalExtension, true);
		}
		
		public string GetOutputFileName(FileProjectItem baseItem, string additionalExtension, bool isPrimaryOutput)
		{
			if (baseItem == null)
				throw new ArgumentNullException("baseItem");
			if (baseItem.Project != project)
				throw new ArgumentException("baseItem is not from project this CustomToolContext belongs to");
			
			if (isPrimaryOutput) {
				string lastGenOutput = baseItem.GetEvaluatedMetadata("LastGenOutput");
				if (!string.IsNullOrEmpty(lastGenOutput)) {
					return Path.Combine(Path.GetDirectoryName(baseItem.FileName), lastGenOutput);
				}
			}
			
			string newExtension = null;
			if (project.LanguageProperties.CodeDomProvider != null) {
				newExtension = project.LanguageProperties.CodeDomProvider.FileExtension;
			}
			if (string.IsNullOrEmpty(newExtension)) {
				if (string.IsNullOrEmpty(additionalExtension)) {
					newExtension = ".unknown";
				} else {
					newExtension = additionalExtension;
					additionalExtension = "";
				}
			}
			if (!newExtension.StartsWith(".")) {
				newExtension = "." + newExtension;
			}
			
			string newFileName = Path.ChangeExtension(baseItem.FileName, additionalExtension + newExtension);
			int retryIndex = 0;
			while (true) {
				FileProjectItem item = project.FindFile(newFileName);
				// If the file does not exist in the project, we can use that name.
				if (item == null)
					return newFileName;
				// If the file already exists in the project, use it only if it belongs to our base item
				if (string.Equals(item.DependentUpon, Path.GetFileName(baseItem.FileName), StringComparison.OrdinalIgnoreCase))
					return newFileName;
				// Otherwise, find another free file name
				retryIndex++;
				newFileName = Path.ChangeExtension(baseItem.FileName, additionalExtension + retryIndex + newExtension);
			}
		}
		
		public FileProjectItem EnsureOutputFileIsInProject(FileProjectItem baseItem, string outputFileName)
		{
			return EnsureOutputFileIsInProject(baseItem, outputFileName, true);
		}
		
		public FileProjectItem EnsureOutputFileIsInProject(FileProjectItem baseItem, string outputFileName, bool isPrimaryOutput)
		{
			if (baseItem == null)
				throw new ArgumentNullException("baseItem");
			if (baseItem.Project != project)
				throw new ArgumentException("baseItem is not from project this CustomToolContext belongs to");
			
			WorkbenchSingleton.AssertMainThread();
			bool saveProject = false;
			if (isPrimaryOutput) {
				if (baseItem.GetEvaluatedMetadata("LastGenOutput") != Path.GetFileName(outputFileName)) {
					saveProject = true;
					baseItem.SetEvaluatedMetadata("LastGenOutput", Path.GetFileName(outputFileName));
				}
			}
			FileProjectItem outputItem = project.FindFile(outputFileName);
			if (outputItem == null) {
				outputItem = new FileProjectItem(project, ItemType.Compile);
				outputItem.FileName = outputFileName;
				outputItem.DependentUpon = Path.GetFileName(baseItem.FileName);
				outputItem.SetEvaluatedMetadata("AutoGen", "True");
				ProjectService.AddProjectItem(project, outputItem);
				FileService.FireFileCreated(outputFileName, false);
				saveProject = true;
				ProjectBrowserPad.RefreshViewAsync();
			}
			if (saveProject)
				project.Save();
			return outputItem;
		}
		
		public void WriteCodeDomToFile(FileProjectItem baseItem, string outputFileName, CodeCompileUnit ccu)
		{
			WorkbenchSingleton.AssertMainThread();
			CodeDomProvider provider = project.LanguageProperties.CodeDomProvider;
			CodeGeneratorOptions options = new CodeDOMGeneratorUtility().CreateCodeGeneratorOptions;
			
			if (project.LanguageProperties == LanguageProperties.VBNet) {
				// the root namespace is implicit in VB
				foreach (CodeNamespace ns in ccu.Namespaces) {
					if (string.Equals(ns.Name, project.RootNamespace, StringComparison.OrdinalIgnoreCase)) {
						ns.Name = string.Empty;
					} else if (ns.Name.StartsWith(project.RootNamespace + ".", StringComparison.OrdinalIgnoreCase)) {
						ns.Name = ns.Name.Substring(project.RootNamespace.Length + 1);
					}
				}
			}
			
			string codeOutput;
			using (StringWriter writer = new StringWriter()) {
				if (provider == null) {
					writer.WriteLine("No CodeDom provider was found for this language.");
				} else {
					provider.GenerateCodeFromCompileUnit(ccu, writer, options);
				}
				codeOutput = writer.ToString();
			}
			
			FileUtility.ObservedSave(delegate(string fileName) {
			                         	File.WriteAllText(fileName, codeOutput, Encoding.UTF8);
			                         },
			                         outputFileName, FileErrorPolicy.Inform);
			EnsureOutputFileIsInProject(baseItem, outputFileName);
			ParserService.BeginParse(outputFileName, new StringTextBuffer(codeOutput));
		}
		
		public void GenerateCodeDomAsync(FileProjectItem baseItem, string outputFileName, Func<CodeCompileUnit> func)
		{
			RunAsync(delegate {
			         	CodeCompileUnit ccu = func();
			         	WorkbenchSingleton.SafeThreadAsyncCall(WriteCodeDomToFile, baseItem, outputFileName, ccu);
			         });
		}
		
		/// <summary>
		/// Gets/Sets the progress monitor this tool can report progress to. This property can never be null.
		/// </summary>
		public IProgressMonitor ProgressMonitor {
			get { return progressMonitor; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
				progressMonitor = value;
			}
		}
	}
	#endregion
	
	#region CustomToolDescriptor
	sealed class CustomToolDescriptor
	{
		string name;
		string fileNamePattern;
		string className;
		ICustomTool tool;
		AddIn addIn;
		
		public string Name {
			get { return name; }
		}
		
		public ICustomTool Tool {
			get {
				if (tool == null) {
					tool = (ICustomTool)addIn.CreateObject(className);
				}
				return tool;
			}
		}
		
		public bool CanRunOnFile(string fileName)
		{
			if (string.IsNullOrEmpty(fileNamePattern)) // no regex specified
				return true;
			return Regex.IsMatch(fileName, fileNamePattern, RegexOptions.IgnoreCase);
		}
		
		public CustomToolDescriptor(string name, string fileNamePattern, string className, AddIn addIn)
		{
			this.name = name;
			this.fileNamePattern = fileNamePattern;
			this.className = className;
			this.addIn = addIn;
		}
	}
	#endregion
	
	#region CustomToolDoozer
	/// <summary>
	/// Creates CustomToolDescriptor objects.
	/// </summary>
	/// <attribute name="id" use="required">
	/// ID used to identify the custom tool.
	/// </attribute>
	/// <attribute name="class" use="required">
	/// Name of the ICustomTool class.
	/// </attribute>
	/// <attribute name="fileNamePattern" use="optional">
	/// Regular expression that specifies the file names for which the custom tool
	/// can be used. Example: "\.res(x|ources)$"
	/// </attribute>
	/// <usage>Only in /SharpDevelop/CustomTools</usage>
	/// <returns>
	/// An CustomToolDescriptor object that wraps a ICustomTool object.
	/// </returns>
	/// <example title="Strongly typed resource generator">
	/// &lt;Path name = "/SharpDevelop/CustomTools"&gt;
	/// 	&lt;CustomTool id    = "ResXFileCodeGenerator"
	/// 	            class = "ResourceEditor.ResourceCodeGeneratorTool"
	/// 	            fileNamePattern = "\.res(x|ources)$"/&gt;
	/// &lt;/Path&gt;
	/// </example>
	public sealed class CustomToolDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public object BuildItem(BuildItemArgs args)
		{
			Codon codon = args.Codon;
			return new CustomToolDescriptor(codon.Id, codon.Properties["fileNamePattern"],
			                                codon.Properties["class"], codon.AddIn);
		}
	}
	#endregion
	
	#region CustomToolsService
	public static class CustomToolsService
	{
		class CustomToolRun {
			internal CustomToolContext context;
			internal string file;
			internal FileProjectItem baseItem;
			internal ICustomTool customTool;
			internal bool showMessageBoxOnErrors;
			
			public CustomToolRun(CustomToolContext context, string file, FileProjectItem baseItem, ICustomTool customTool, bool showMessageBoxOnErrors)
			{
				this.context = context;
				this.file = file;
				this.baseItem = baseItem;
				this.customTool = customTool;
				this.showMessageBoxOnErrors = showMessageBoxOnErrors;
			}
		}
		
		static bool initialized;
		static Queue<CustomToolRun> toolRuns = new Queue<CustomToolRun>();
		static Dictionary<string, CustomToolDescriptor> toolDict;
		static List<CustomToolDescriptor> customToolList;
		static CustomToolRun activeToolRun;
		static BeforeBuildCustomToolRunner beforeBuildCustomToolRunner;
		
		internal static void Initialize()
		{
			customToolList = AddInTree.BuildItems<CustomToolDescriptor>("/SharpDevelop/CustomTools", null, false);
			toolDict = new Dictionary<string, CustomToolDescriptor>(StringComparer.OrdinalIgnoreCase);
			foreach (CustomToolDescriptor desc in customToolList) {
				toolDict[desc.Name] = desc;
			}
			
			if (!initialized) {
				initialized = true;
				FileUtility.FileSaved += OnFileSaved;
			}
			
			beforeBuildCustomToolRunner = new BeforeBuildCustomToolRunner();
		}
		
		static void OnFileSaved(object sender, FileNameEventArgs e)
		{
			Solution solution = ProjectService.OpenSolution;
			if (solution == null) return;
			IProject project = solution.FindProjectContainingFile(e.FileName);
			if (project == null) return;
			FileProjectItem item = project.FindFile(e.FileName);
			if (item == null) return;
			if (!string.IsNullOrEmpty(item.CustomTool)) {
				RunCustomTool(item, false);
			}
		}
		
		public static IEnumerable<string> GetCustomToolNames()
		{
			return customToolList.ConvertAll<string>(delegate(CustomToolDescriptor desc) {
			                                         	return desc.Name;
			                                         });
		}
		
		public static IEnumerable<string> GetCompatibleCustomToolNames(FileProjectItem item)
		{
			string fileName = item.FileName;
			foreach (CustomToolDescriptor desc in customToolList) {
				if (desc.CanRunOnFile(fileName)) {
					yield return desc.Name;
				}
			}
		}
		
		public static ICustomTool GetCustomTool(string name)
		{
			lock (toolDict) {
				CustomToolDescriptor tool;
				if (toolDict.TryGetValue(name, out tool))
					return tool.Tool;
				else
					return null;
			}
		}
		
		/// <summary>
		/// Runs the custom tool specified by the base items' CustomTool property on the base item.
		/// </summary>
		public static void RunCustomTool(FileProjectItem baseItem, bool showMessageBoxOnErrors)
		{
			if (baseItem == null)
				throw new ArgumentNullException("baseItem");
			
			if (string.IsNullOrEmpty(baseItem.CustomTool))
				return;
			
			ICustomTool customTool = GetCustomTool(baseItem.CustomTool);
			if (customTool == null) {
				string message = "Cannot find custom tool '" + baseItem.CustomTool + "'.";
				if (!baseItem.CustomTool.StartsWith("MSBuild:")) {
					CustomToolContext.StaticMessageView.AppendLine(message);
				}
				if (showMessageBoxOnErrors) {
					MessageService.ShowError(message);
				}
			} else {
				RunCustomTool(baseItem, customTool, showMessageBoxOnErrors);
			}
		}
		
		/// <summary>
		/// Runs the specified custom tool on the base item.
		/// </summary>
		public static void RunCustomTool(FileProjectItem baseItem, ICustomTool customTool, bool showMessageBoxOnErrors)
		{
			if (baseItem == null)
				throw new ArgumentNullException("baseItem");
			if (customTool == null)
				throw new ArgumentNullException("customTool");
			WorkbenchSingleton.AssertMainThread();
			
			string fileName = baseItem.FileName;
			if (toolRuns.Any(run => FileUtility.IsEqualFileName(run.file, fileName)))
			{
				// file already in queue, do not enqueue it again
				return;
			}
			CustomToolContext context = new CustomToolContext(baseItem.Project);
			context.OutputNamespace = baseItem.GetEvaluatedMetadata("CustomToolNamespace");
			if (string.IsNullOrEmpty(context.OutputNamespace)) {
				context.OutputNamespace = GetDefaultNamespace(baseItem.Project, baseItem.FileName);
			}
			RunCustomTool(new CustomToolRun(context, fileName, baseItem, customTool, showMessageBoxOnErrors));
		}
		
		/// <summary>
		/// Gets the namespace the file should have in the specified project.
		/// </summary>
		public static string GetDefaultNamespace(IProject project, string fileName)
		{
			if (project == null)
				throw new ArgumentNullException("project");
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			
			if (project.LanguageProperties == Dom.LanguageProperties.VBNet) {
				return project.RootNamespace;
			} else {
				string relPath = FileUtility.GetRelativePath(project.Directory, Path.GetDirectoryName(fileName));
				string[] subdirs = relPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				StringBuilder standardNameSpace = new StringBuilder(project.RootNamespace);
				foreach(string subdir in subdirs) {
					if (subdir == "." || subdir == ".." || subdir.Length == 0)
						continue;
					if (subdir.Equals("src", StringComparison.OrdinalIgnoreCase))
						continue;
					if (subdir.Equals("source", StringComparison.OrdinalIgnoreCase))
						continue;
					if (standardNameSpace.Length > 0)
						standardNameSpace.Append('.');
					standardNameSpace.Append(NewFileDialog.GenerateValidClassOrNamespaceName(subdir, true));
				}
				return standardNameSpace.ToString();
			}
		}
		
		static void RunCustomTool(CustomToolRun run)
		{
			if (activeToolRun != null) {
				toolRuns.Enqueue(run);
			} else {
				try {
					run.customTool.GenerateCode(run.baseItem, run.context);
				} catch (Exception ex) {
					LoggingService.Error(ex);
					run.context.MessageView.AppendLine("Custom tool '" + run.baseItem.CustomTool + "' failed.");
					if (run.showMessageBoxOnErrors) {
						MessageService.ShowError("Custom tool '" + run.baseItem.CustomTool
						                         + "' failed:" + Environment.NewLine + ex.ToString());
					}
				}
				if (run.context.RunningSeparateThread) {
					activeToolRun = run;
				}
			}
		}
		
		internal static void NotifyAsyncFinish(CustomToolContext context)
		{
			WorkbenchSingleton.SafeThreadAsyncCall(
				delegate {
					activeToolRun = null;
					if(toolRuns.Count > 0) {
						CustomToolRun nextRun = toolRuns.Dequeue();
						if(nextRun != null) {
							RunCustomTool(nextRun);
						}
					}
				});
		}
	}
	#endregion
	
	#region ExecuteCustomToolCommand
	public sealed class ExecuteCustomToolCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			FileNode node = Owner as FileNode;
			if (node != null) {
				FileProjectItem item = node.ProjectItem as FileProjectItem;
				if (item != null) {
					CustomToolsService.RunCustomTool(item, true);
				}
			}
		}
	}
	#endregion
}
