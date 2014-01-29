// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace CppBackendBinding
{
	/// <summary>
	/// C++ project class. Handlings project loading and saving.
	/// </summary>
	public class CppProject : AbstractProject, IProject
	{
		sealed class ReadOnlyConfigurationOrPlatformNameCollection : ImmutableModelCollection<string>, IConfigurationOrPlatformNameCollection
		{
			public ReadOnlyConfigurationOrPlatformNameCollection(IEnumerable<string> items)
				: base(items)
			{
			}
			
			public string ValidateName(string name)
			{
				return Contains(name) ? name : null;
			}
			
			public void Add(string newName, string copyFrom)
			{
				throw new NotSupportedException();
			}
			
			public void Remove(string name)
			{
				throw new NotSupportedException();
			}
			
			public void Rename(string oldName, string newName)
			{
				throw new NotSupportedException();
			}
		}
		
		class CppProjectItemsCollection : SimpleModelCollection<ProjectItem>
		{
			readonly CppProject project;
			readonly bool internalUpdating;
			
			public CppProjectItemsCollection(CppProject project)
			{
				internalUpdating = true;
				this.project = project;
				this.AddRange(this.project.items.Select(item => item.ProjectItem));
				internalUpdating = false;
			}
			
			protected override void OnAdd(ProjectItem item)
			{
				base.OnAdd(item);
				
				lock (project.SyncRoot) {
					if (!internalUpdating) {
						if (project.items.Exists(fi => fi.ProjectItem == item))
							throw new ArgumentException("Project item already exists in project!");
						project.items.Add(new FileItem(project.document, item));
					}
				}
			}
			
			protected override void OnRemove(ProjectItem item)
			{
				base.OnRemove(item);
				
				lock (project.SyncRoot) {
					if (!internalUpdating) {
						var removedFileItems = new List<FileItem>(project.items.Where(fi => fi.ProjectItem == item));
						foreach (var fileItem in removedFileItems) {
							if (fileItem.XmlElement.ParentNode != null)
								fileItem.XmlElement.ParentNode.RemoveChild(fileItem.XmlElement);
							project.items.Remove(fileItem);
						}
					}
				}
			}
		}
		
		XmlDocument document = new XmlDocument();
		List<FileGroup> groups = new List<FileGroup>();
		List<FileItem> items = new List<FileItem>();
		CppProjectItemsCollection projectItems;
		
		/// <summary>
		/// Create a new C++ project that loads the specified .vcproj file.
		/// </summary>
		public CppProject(ProjectLoadInformation info)
			: base(info)
		{
			this.Name = info.ProjectName;
			this.FileName = info.FileName;
			
			using (StreamReader r = new StreamReader(info.FileName, Encoding.Default)) {
				try {
					document.Load(r);
				} catch (Exception ex) {
					throw new ProjectLoadException(ex.Message, ex);
				}
			}
			if (document.DocumentElement.Name != "VisualStudioProject")
				throw new ProjectLoadException("The project is not a visual studio project.");
			XmlElement filesElement = document.DocumentElement["Files"];
			if (filesElement != null) {
				foreach (XmlElement filterElement in filesElement.ChildNodes.OfType<XmlElement>()) {
					if (filterElement.Name == "Filter") {
						FileGroup group = new FileGroup(this, filterElement);
						groups.Add(group);
						foreach (XmlElement fileElement in filterElement.ChildNodes.OfType<XmlElement>()) {
							if (fileElement.Name == "File" && fileElement.HasAttribute("RelativePath")) {
								items.Add(new FileItem(group, fileElement));
							}
						}
					}
				}
			}
			
			this.projectItems = new CppProjectItemsCollection(this);
		}
		
		public override string Language {
			get { return CppProjectBinding.LanguageName; }
		}
		
		public override void Save(string fileName)
		{
			lock (SyncRoot) {
				// file item types may have changed, so remove all items from their parent elements
				// and re-add them to the correct Filter elements
				foreach (FileItem item in items) {
					item.SaveChanges();
					if (item.XmlElement.ParentNode != null)
						item.XmlElement.ParentNode.RemoveChild(item.XmlElement);
				}
				foreach (FileItem item in items) {
					FileGroup group = groups.Find(fg => fg.ItemType == item.ProjectItem.ItemType);
					if (group != null) {
						group.XmlElement.AppendChild(item.XmlElement);
					} else {
						LoggingService.Warn("Couldn't find filter for item type " + item.ProjectItem.ItemType + ", the item was not saved!");
					}
				}
				
				watcher.Disable();
				using (XmlWriter writer = XmlWriter.Create(fileName, new XmlWriterSettings {
				                                           	NewLineOnAttributes = true,
				                                           	Indent = true,
				                                           	IndentChars = "\t",
				                                           	Encoding = Encoding.Default
				                                           }))
				{
					document.Save(writer);
				}
				watcher.Enable();
			}
		}
		
		/// <summary>
		/// Gets the list of available file item types. This member is thread-safe.
		/// </summary>
		public override IReadOnlyCollection<ItemType> AvailableFileItemTypes {
			get {
				lock (SyncRoot) {
					return groups.ConvertAll(fg => fg.ItemType).AsReadOnly();
				}
			}
		}
		
		/// <summary>
		/// Gets the list of items in the project. This member is thread-safe.
		/// </summary>
		public override IMutableModelCollection<ProjectItem> Items {
			get {
				lock (SyncRoot) {
					return projectItems;
				}
			}
		}
		
		public override ItemType GetDefaultItemType(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (string.Equals(extension, ".c", StringComparison.OrdinalIgnoreCase) || string.Equals(extension, ".cpp", StringComparison.OrdinalIgnoreCase))
				return ItemType.Compile;
			else if (string.Equals(extension, ".h", StringComparison.OrdinalIgnoreCase))
				return ItemType.Header;
			else
				return base.GetDefaultItemType(fileName);
		}
		
		static string GetFile(string filename)
		{
			if (string.IsNullOrEmpty(filename))
				return null;
			filename = Environment.ExpandEnvironmentVariables(filename);
			if (File.Exists(filename))
				return filename;
			else
				return null;
		}
		
		static string GetPathFromRegistry(string key, string valueName)
		{
			using (RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey(key)) {
				if (installRootKey != null) {
					object o = installRootKey.GetValue(valueName);
					if (o != null) {
						string r = o.ToString();
						if (!string.IsNullOrEmpty(r))
							return r;
					}
				}
			}
			return null;
		}
		
		public override Task<bool> BuildAsync(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor)
		{
			TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
			StartBuild(tcs, options, feedbackSink, progressMonitor);
			return tcs.Task;
		}
		
		void StartBuild(TaskCompletionSource<bool> tcs, ProjectBuildOptions options, IBuildFeedbackSink feedbackSink, IProgressMonitor progressMonitor)
		{
			string productDir = GetPathFromRegistry(@"SOFTWARE\Microsoft\VisualStudio\9.0\Setup\VC", "ProductDir");
			
			string batFile = "vcvars32.bat";
			if (options.Platform == "x64") {
				batFile = "amd64\\vcvars64.bat";
			}
			
			string commonTools =
				GetFile(productDir != null ? Path.Combine(productDir, "bin\\" + batFile) : null)
				?? GetFile("%VS90COMNTOOLS%\\" + batFile)
				??  GetFile("%VS80COMNTOOLS%\\" + batFile);
			
			Process p = new Process();
			p.StartInfo.FileName = "cmd.exe";
			p.StartInfo.Arguments = "/C";
			if (!string.IsNullOrEmpty(commonTools)) {
				p.StartInfo.Arguments += " call \"" + commonTools + "\" &&";
			}
			p.StartInfo.Arguments += " vcbuild";
			if (options.Target == BuildTarget.Build) {
				// OK
			} else if (options.Target == BuildTarget.Clean) {
				p.StartInfo.Arguments += " /clean";
			} else if (options.Target == BuildTarget.Rebuild) {
				p.StartInfo.Arguments += " /rebuild";
			}
			p.StartInfo.Arguments += " /showenv";
			p.StartInfo.Arguments += " \"" + this.FileName + "\"";
			p.StartInfo.Arguments += " \"/error:Error: \"";
			p.StartInfo.Arguments += " \"/warning:Warning: \"";
			
			p.StartInfo.WorkingDirectory = this.Directory;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.EnvironmentVariables["VCBUILD_DEFAULT_CFG"] = options.Configuration + "|" + options.Platform;
			p.StartInfo.EnvironmentVariables["SolutionPath"] = ParentSolution.FileName;
			
			p.EnableRaisingEvents = true;
			bool buildErrors = false;
			p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
				if (!string.IsNullOrEmpty(e.Data)) {
					BuildError error = ParseError(e.Data);
					if (error != null) {
						feedbackSink.ReportError(error);
						buildErrors = true;
					} else {
						feedbackSink.ReportMessage(new RichText(e.Data));
					}
				}
			};
			p.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
				if (!string.IsNullOrEmpty(e.Data)) {
					BuildError error = ParseError(e.Data);
					if (error != null)
						feedbackSink.ReportError(error);
					else
						feedbackSink.ReportError(new BuildError(null, e.Data));
					buildErrors = true;
				}
			};
			p.Exited += delegate(object sender, EventArgs e) {
				p.CancelErrorRead();
				p.CancelOutputRead();
				progressMonitor.Progress = 1;
				p.Dispose();
				tcs.SetResult(buildErrors);
			};
			
			feedbackSink.ReportMessage(new RichText("Building " + this.Name));
			feedbackSink.ReportMessage(new RichText(p.StartInfo.FileName + " " + p.StartInfo.Arguments));
			p.Start();
			p.BeginOutputReadLine();
			p.BeginErrorReadLine();
		}
		
		static readonly Regex errorRegex = new Regex(@"^Error: " +
		                                             @"((?:[^(:]|:\\)+)" + // group 1: file name
		                                             @"(?:\((\d+)\))?" + // group 2: line number
		                                             @"\s*:\s*" + // first separator
		                                             @"(?:error ([^:]+):)?" + // group 3: error code
		                                             @"\s*(.*)$" // group 4: error message
		                                            );
		
		
		static readonly Regex warningRegex = new Regex(@"^(?:\d+\>)?Warning: " +
		                                               @"((?:[^(:]|:\\)+)" + // group 1: file name
		                                               @"(?:\((\d+)\))?" + // group 2: line number
		                                               @"\s*:\s*" + // first separator
		                                               @"(?:warning ([^:]+):)?" + // group 3: error code
		                                               @"\s*(.*)$" // group 4: error message
		                                              );
		
		
		/// <summary>
		/// Parses an error or warning message and returns a BuildError object for it.
		/// </summary>
		BuildError ParseError(string text)
		{
			bool isWarning = false;
			Match match = errorRegex.Match(text);
			if (!match.Success) {
				match = warningRegex.Match(text);
				isWarning = true;
			}
			if (match.Success) {
				int line = -1;
				try {
					if (match.Groups[2].Length > 0) {
						line = int.Parse(match.Groups[2].Value);
					}
				} catch (FormatException) {
				} catch (OverflowException) {
				}
				return new BuildError(Path.Combine(Directory, match.Groups[1].Value), line, 0,
				                      match.Groups[3].Value, match.Groups[4].Value) {
					IsWarning = isWarning
				};
			} else {
				return null;
			}
		}
		
		public override IConfigurationOrPlatformNameCollection PlatformNames {
			get {
				List<string> l = new List<string>();
				foreach (XmlElement platformElement in document.DocumentElement["Platforms"]) {
					l.Add(platformElement.GetAttribute("Name"));
				}
				return new ReadOnlyConfigurationOrPlatformNameCollection(l);
			}
		}
	}
}
