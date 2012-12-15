// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project.Commands;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// Converts projects from one language to another, for example C# &lt;-&gt; VB
	/// </summary>
	public abstract class LanguageConverter : AbstractMenuCommand
	{
		protected virtual void AfterConversion(IProject targetProject) {}
		
		public abstract string TargetLanguageName { get; }
		
		protected virtual IProject CreateProject(string targetProjectDirectory, IProject sourceProject)
		{
			ProjectCreateInformation info = new ProjectCreateInformation();
			info.Solution = sourceProject.ParentSolution;
			info.ProjectBasePath = targetProjectDirectory;
			info.ProjectName = sourceProject.Name + ".Converted";
			info.RootNamespace = sourceProject.RootNamespace;
			
			ProjectBindingDescriptor descriptor = ProjectBindingService.GetCodonPerLanguageName(TargetLanguageName);
			if (descriptor == null || descriptor.Binding == null)
				throw new InvalidOperationException("Cannot get Language Binding for " + TargetLanguageName);
			
			info.OutputProjectFileName = FileUtility.NormalizePath(Path.Combine(targetProjectDirectory, info.ProjectName + descriptor.ProjectFileExtension));
			
			return descriptor.Binding.CreateProject(info);
		}
		
		protected virtual void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			if (!File.Exists(targetItem.FileName)) {
				File.Copy(sourceItem.FileName, targetItem.FileName);
			}
		}
		
		protected virtual double GetRequiredWork(ProjectItem item)
		{
			if (item.ItemType == ItemType.Compile) {
				return 50;
			} else if (ItemType.DefaultFileItems.Contains(item.ItemType)) {
				return 4;
			} else {
				return 1;
			}
		}
		
		protected virtual void CopyProperties(IProject sourceProject, IProject targetProject)
		{
			MSBuildBasedProject sp = sourceProject as MSBuildBasedProject;
			MSBuildBasedProject tp = targetProject as MSBuildBasedProject;
			if (sp != null && tp != null) {
				lock (sp.SyncRoot) {
					lock (tp.SyncRoot) {
						// Remove all PropertyGroups in target project:
						foreach (ProjectPropertyGroupElement tpg in tp.MSBuildProjectFile.PropertyGroups) {
							tp.MSBuildProjectFile.RemoveChild(tpg);
						}
						// Copy all PropertyGroups from source project to target project:
						foreach (ProjectPropertyGroupElement spg in sp.MSBuildProjectFile.PropertyGroups) {
							ProjectPropertyGroupElement tpg = tp.MSBuildProjectFile.AddPropertyGroup();
							tpg.Condition = spg.Condition;
							foreach (ProjectPropertyElement sprop in spg.Properties) {
								ProjectPropertyElement tprop = tpg.AddProperty(sprop.Name, sprop.Value);
								tprop.Condition = sprop.Condition;
							}
						}
						
						// use the newly created IdGuid instead of the copied one
						tp.SetProperty(MSBuildBasedProject.ProjectGuidPropertyName, tp.IdGuid);
					}
				}
			}
		}
		
		protected virtual void FixExtensionOfExtraProperties(FileProjectItem item, string sourceExtension, string targetExtension)
		{
			List<KeyValuePair<string, string>> replacements = new List<KeyValuePair<string, string>>();
			foreach (string metadataName in item.MetadataNames) {
				if ("Include".Equals(metadataName, StringComparison.OrdinalIgnoreCase))
					continue;
				string value = item.GetMetadata(metadataName);
				if (value.EndsWith(sourceExtension, StringComparison.OrdinalIgnoreCase)) {
					replacements.Add(new KeyValuePair<string, string>(metadataName, value));
				}
			}
			foreach (KeyValuePair<string, string> pair in replacements) {
				item.SetMetadata(pair.Key, Path.ChangeExtension(pair.Value, targetExtension));
			}
		}
		
		protected virtual void CopyItems(IProject sourceProject, IProject targetProject, IProgressMonitor monitor)
		{
			if (sourceProject == null)
				throw new ArgumentNullException("sourceProject");
			if (targetProject == null)
				throw new ArgumentNullException("targetProject");
			IProjectItemListProvider targetProjectItems = targetProject as IProjectItemListProvider;
			if (targetProjectItems == null)
				throw new ArgumentNullException("targetProjectItems");
			
			ICollection<ProjectItem> sourceItems = sourceProject.Items;
			double totalWork = 0;
			foreach (ProjectItem item in sourceItems) {
				totalWork += GetRequiredWork(item);
			}
			
			foreach (ProjectItem item in sourceItems) {
				FileProjectItem fileItem = item as FileProjectItem;
				if (fileItem != null && FileUtility.IsBaseDirectory(sourceProject.Directory, fileItem.FileName)) {
					FileProjectItem targetItem = new FileProjectItem(targetProject, fileItem.ItemType);
					fileItem.CopyMetadataTo(targetItem);
					targetItem.Include = fileItem.Include;
					if (File.Exists(fileItem.FileName)) {
						if (!Directory.Exists(Path.GetDirectoryName(targetItem.FileName))) {
							Directory.CreateDirectory(Path.GetDirectoryName(targetItem.FileName));
						}
						try {
							ConvertFile(fileItem, targetItem);
						} catch (Exception ex) {
							throw new ConversionException("Error converting " + fileItem.FileName, ex);
						}
					}
					targetProjectItems.AddProjectItem(targetItem);
				} else {
					targetProjectItems.AddProjectItem(item.CloneFor(targetProject));
				}
				monitor.CancellationToken.ThrowIfCancellationRequested();
				monitor.Progress += GetRequiredWork(item) / totalWork;
			}
		}
		
		protected StringBuilder conversionLog;
		
		public override void Run()
		{
			conversionLog = new StringBuilder();
			string translatedTitle = ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.Convert.ProjectConverter");
			conversionLog.AppendLine(translatedTitle);
			conversionLog.Append('=', translatedTitle.Length);
			conversionLog.AppendLine();
			conversionLog.AppendLine();
			MSBuildBasedProject sourceProject = ProjectService.CurrentProject as MSBuildBasedProject;
			string targetProjectDirectory = sourceProject.Directory + ".ConvertedTo" + TargetLanguageName;
			if (Directory.Exists(targetProjectDirectory)) {
				MessageService.ShowMessageFormatted(translatedTitle, "${res:ICSharpCode.SharpDevelop.Commands.Convert.TargetAlreadyExists}", targetProjectDirectory);
				return;
			}
			conversionLog.Append(ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.Convert.SourceDirectory")).Append(": ");
			conversionLog.AppendLine(sourceProject.Directory);
			conversionLog.Append(ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.Convert.TargetDirectory")).Append(": ");
			conversionLog.AppendLine(targetProjectDirectory);
			
			try {
				PerformConversion(translatedTitle, sourceProject, targetProjectDirectory);
			} catch (OperationCanceledException) {
				// ignore
			}
		}
		
		void PerformConversion(string translatedTitle, MSBuildBasedProject sourceProject, string targetProjectDirectory)
		{
			IProject targetProject;
			using (AsynchronousWaitDialog monitor = AsynchronousWaitDialog.ShowWaitDialog(translatedTitle, "Converting", true)) {
				Directory.CreateDirectory(targetProjectDirectory);
				targetProject = CreateProject(targetProjectDirectory, sourceProject);
				CopyProperties(sourceProject, targetProject);
				conversionLog.AppendLine();
				CopyItems(sourceProject, targetProject, monitor);
				monitor.CancellationToken.ThrowIfCancellationRequested();
				conversionLog.AppendLine();
				AfterConversion(targetProject);
				conversionLog.AppendLine(ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.Convert.ConversionComplete"));
				targetProject.Save();
				targetProject.Dispose();
			}
			
			TreeNode node = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				node = ProjectBrowserPad.Instance.SolutionNode;
			}
			while (node != null) {
				if (node is ISolutionFolderNode) {
					AddExistingProjectToSolution.AddProject((ISolutionFolderNode)node, targetProject.FileName);
					ProjectService.SaveSolution();
					break;
				}
				node = node.Parent;
			}
			IViewContent newFileWindow;
			newFileWindow = FileService.NewFile(ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.Convert.ConversionResults"), conversionLog.ToString());
			if (newFileWindow != null) {
				newFileWindow.PrimaryFile.IsDirty = false;
			}
		}
	}

	public abstract class NRefactoryLanguageConverter : LanguageConverter
	{
		protected abstract void ConvertAst(CompilationUnit compilationUnit, List<ISpecial> specials,
		                                   FileProjectItem sourceItem);
		
		protected void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem,
		                           string sourceExtension, string targetExtension,
		                           SupportedLanguage sourceLanguage, IOutputAstVisitor outputVisitor)
		{
			FixExtensionOfExtraProperties(targetItem, sourceExtension, targetExtension);
			if (sourceExtension.Equals(Path.GetExtension(sourceItem.FileName), StringComparison.OrdinalIgnoreCase)) {
				ITextBuffer code = ParserService.GetParseableFileContent(sourceItem.FileName);
				var p = ParserFactory.CreateParser(sourceLanguage, code.CreateReader());
				p.Parse();
				if (p.Errors.Count > 0) {
					conversionLog.AppendLine();
					conversionLog.AppendLine(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.Convert.IsNotConverted}",
					                                            new StringTagPair("FileName", sourceItem.FileName)));
					conversionLog.AppendLine(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.Convert.ParserErrorCount}",
					                                            new StringTagPair("ErrorCount", p.Errors.Count.ToString())));
					conversionLog.AppendLine(p.Errors.ErrorOutput);
					base.ConvertFile(sourceItem, targetItem);
					return;
				}
				
				List<ISpecial> specials = p.Lexer.SpecialTracker.CurrentSpecials;
				
				ConvertAst(p.CompilationUnit, specials, sourceItem);
				
				using (SpecialNodesInserter.Install(specials, outputVisitor)) {
					outputVisitor.VisitCompilationUnit(p.CompilationUnit, null);
				}
				
				p.Dispose();
				
				if (outputVisitor.Errors.Count > 0) {
					conversionLog.AppendLine();
					conversionLog.AppendLine(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.Convert.ConverterErrorCount}",
					                                            new StringTagPair("FileName", sourceItem.FileName),
					                                            new StringTagPair("ErrorCount", outputVisitor.Errors.Count.ToString())));
					conversionLog.AppendLine(outputVisitor.Errors.ErrorOutput);
				}
				
				targetItem.Include = Path.ChangeExtension(targetItem.Include, targetExtension);
				File.WriteAllText(targetItem.FileName, outputVisitor.Text);
			} else {
				base.ConvertFile(sourceItem, targetItem);
			}
		}
		
		protected override void CopyProperties(IProject sourceProject, IProject targetProject)
		{
			base.CopyProperties(sourceProject, targetProject);
			
			// WarningsAsErrors/NoWarn refers to warning numbers that are completely different for
			// the two compilers.
			MSBuildBasedProject p = (MSBuildBasedProject)targetProject;
			p.SetProperty("WarningsAsErrors", null);
			p.SetProperty("NoWarn", null);
		}
	}
	
	/// <summary>
	/// Exception used when converting a file fails.
	/// </summary>
	[Serializable]
	public class ConversionException : Exception
	{
		public ConversionException() : base()
		{
		}
		
		public ConversionException(string message) : base(message)
		{
		}
		
		public ConversionException(string message, Exception innerException) : base(message, innerException)
		{
		}
		
		protected ConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
