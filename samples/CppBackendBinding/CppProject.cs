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
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using Microsoft.Win32;

namespace CppBackendBinding
{
	/// <summary>
	/// C++ project class. Handlings project loading and saving.
	/// </summary>
	public class CppProject : AbstractProject, IProjectItemListProvider, IProjectAllowChangeConfigurations
	{
		XmlDocument document = new XmlDocument();
		List<FileGroup> groups = new List<FileGroup>();
		List<FileItem> items = new List<FileItem>();
		
		/// <summary>
		/// Create a new C++ project that loads the specified .vcproj file.
		/// </summary>
		public CppProject(ProjectLoadInformation info)
		{
			this.Name = info.ProjectName;
			this.FileName = info.FileName;
			this.TypeGuid = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
			
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
				using (XmlWriter writer = XmlWriter.Create(fileName, new XmlWriterSettings {
				                                           	NewLineOnAttributes = true,
				                                           	Indent = true,
				                                           	IndentChars = "\t",
				                                           	Encoding = Encoding.Default
				                                           }))
				{
					document.Save(writer);
				}
			}
		}
		
		/// <summary>
		/// Gets the list of available file item types. This member is thread-safe.
		/// </summary>
		public override ICollection<ItemType> AvailableFileItemTypes {
			get {
				lock (SyncRoot) {
					return groups.ConvertAll(fg => fg.ItemType).AsReadOnly();
				}
			}
		}
		
		/// <summary>
		/// Gets the list of items in the project. This member is thread-safe.
		/// The returned collection is guaranteed not to change - adding new items or removing existing items
		/// will create a new collection.
		/// </summary>
		public override ReadOnlyCollection<ProjectItem> Items {
			get {
				lock (SyncRoot) {
					return new ReadOnlyCollection<ProjectItem>(items.ConvertAll(fi => fi.ProjectItem));
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
		
		#region IProjectAllowChangeConfigurations
		// TODO: Configuration/Platform handling
		bool IProjectAllowChangeConfigurations.RenameProjectConfiguration(string oldName, string newName)
		{
			throw new NotImplementedException();
		}
		
		bool IProjectAllowChangeConfigurations.RenameProjectPlatform(string oldName, string newName)
		{
			throw new NotImplementedException();
		}
		
		bool IProjectAllowChangeConfigurations.AddProjectConfiguration(string newName, string copyFrom)
		{
			throw new NotImplementedException();
		}
		
		bool IProjectAllowChangeConfigurations.AddProjectPlatform(string newName, string copyFrom)
		{
			throw new NotImplementedException();
		}
		
		bool IProjectAllowChangeConfigurations.RemoveProjectConfiguration(string name)
		{
			throw new NotImplementedException();
		}
		
		bool IProjectAllowChangeConfigurations.RemoveProjectPlatform(string name)
		{
			throw new NotImplementedException();
		}
		#endregion
		
		#region IProjectItemListProvider
		void IProjectItemListProvider.AddProjectItem(ProjectItem item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			lock (SyncRoot) {
				if (items.Exists(fi => fi.ProjectItem == item))
					throw new ArgumentException("Project item already exists in project!");
				items.Add(new FileItem(document, item));
			}
		}
		
		bool IProjectItemListProvider.RemoveProjectItem(ProjectItem item)
		{
			lock (SyncRoot) {
				return items.RemoveAll(fi => fi.ProjectItem == item) > 0;
			}
		}
		#endregion
		
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
		
		public override void StartBuild(ProjectBuildOptions options, IBuildFeedbackSink feedbackSink)
		{
			string productDir = GetPathFromRegistry(@"SOFTWARE\Microsoft\VisualStudio\9.0\Setup\VC", "ProductDir");
			
			string batFile = "vcvars32.bat";
			if (options.Platform == "x64") {
				batFile = "vcvars64.bat";
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
			p.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e) {
				if (!string.IsNullOrEmpty(e.Data)) {
					BuildError error = ParseError(e.Data);
					if (error != null)
						feedbackSink.ReportError(error);
					else
						feedbackSink.ReportMessage(e.Data);
				}
			};
			p.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e) {
				if (!string.IsNullOrEmpty(e.Data)) {
					BuildError error = ParseError(e.Data);
					if (error != null)
						feedbackSink.ReportError(error);
					else
						feedbackSink.ReportError(new BuildError(null, e.Data));
				}
			};
			p.Exited += delegate(object sender, EventArgs e) {
				p.CancelErrorRead();
				p.CancelOutputRead();
				feedbackSink.Done(p.ExitCode == 0);
				p.Dispose();
			};
			
			feedbackSink.ReportMessage("Building " + this.Name);
			feedbackSink.ReportMessage(p.StartInfo.FileName + " " + p.StartInfo.Arguments);
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
		
		public override ICollection<string> PlatformNames {
			get {
				List<string> l = new List<string>();
				foreach (XmlElement platformElement in document.DocumentElement["Platforms"]) {
					l.Add(platformElement.GetAttribute("Name"));
				}
				return l.AsReadOnly();
			}
		}
	}
}
