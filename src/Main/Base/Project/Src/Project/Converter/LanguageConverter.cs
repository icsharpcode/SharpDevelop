// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project.Commands;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace ICSharpCode.SharpDevelop.Project.Converter
{
	/// <summary>
	/// Converts projects from one language to another, for example C# &lt;-&gt; VB
	/// </summary>
	public abstract class LanguageConverter : AbstractMenuCommand
	{
		protected abstract IProject CreateProject(string targetProjectDirectory, IProject sourceProject);
		protected virtual void AfterConversion(IProject targetProject) {}
		
		public abstract string TargetLanguageName { get; }
		
		protected virtual void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem)
		{
			if (!File.Exists(targetItem.FileName)) {
				File.Copy(sourceItem.FileName, targetItem.FileName);
			}
		}
		
		protected virtual void CopyProperties(IProject sourceProject, IProject targetProject)
		{
			AbstractProject sp = sourceProject as AbstractProject;
			AbstractProject tp = targetProject as AbstractProject;
			if (sp != null && tp != null) {
				tp.Configurations.Clear();
				tp.UserConfigurations.Clear();
				foreach (KeyValuePair<string, PropertyGroup> pair in sp.Configurations) {
					tp.Configurations.Add(pair.Key, pair.Value.Clone());
				}
				foreach (KeyValuePair<string, PropertyGroup> pair in sp.UserConfigurations) {
					tp.UserConfigurations.Add(pair.Key, pair.Value.Clone());
				}
				tp.BaseConfiguration.Merge(sp.BaseConfiguration);
				tp.UserBaseConfiguration.Merge(sp.UserBaseConfiguration);
			}
		}
		
		/// <summary>
		/// Changes a property in the <paramref name="project"/> by applying a method to its value.
		/// </summary>
		protected void FixProperty(AbstractProject project, string propertyName, Converter<string, string> method)
		{
			if (project.BaseConfiguration.IsSet(propertyName))
				project.BaseConfiguration[propertyName] = method(project.BaseConfiguration[propertyName]);
			if (project.UserBaseConfiguration.IsSet(propertyName))
				project.UserBaseConfiguration[propertyName] = method(project.UserBaseConfiguration[propertyName]);
			foreach (PropertyGroup pg in project.Configurations.Values) {
				if (pg.IsSet(propertyName))
					pg[propertyName] = method(pg[propertyName]);
			}
			foreach (PropertyGroup pg in project.UserConfigurations.Values) {
				if (pg.IsSet(propertyName))
					pg[propertyName] = method(pg[propertyName]);
			}
		}
		
		protected virtual void FixExtensionOfExtraProperties(FileProjectItem item, string sourceExtension, string targetExtension)
		{
			sourceExtension = sourceExtension.ToLowerInvariant();
			
			List<KeyValuePair<string, string>> replacements = new List<KeyValuePair<string, string>>();
			foreach (KeyValuePair<string, string> pair in item.Properties) {
				if ("Include".Equals(pair.Key, StringComparison.OrdinalIgnoreCase))
					continue;
				if (pair.Value.ToLowerInvariant().EndsWith(sourceExtension)) {
					replacements.Add(pair);
				}
			}
			foreach (KeyValuePair<string, string> pair in replacements) {
				item.Properties[pair.Key] = Path.ChangeExtension(pair.Value, targetExtension);
			}
		}
		
		protected virtual void CopyItems(IProject sourceProject, IProject targetProject)
		{
			foreach (ProjectItem item in sourceProject.Items) {
				FileProjectItem fileItem = item as FileProjectItem;
				if (fileItem != null && FileUtility.IsBaseDirectory(sourceProject.Directory, fileItem.FileName)) {
					FileProjectItem targetItem = new FileProjectItem(targetProject, fileItem.ItemType);
					fileItem.CopyExtraPropertiesTo(targetItem);
					targetItem.Include = fileItem.Include;
					if (File.Exists(fileItem.FileName)) {
						if (!Directory.Exists(Path.GetDirectoryName(targetItem.FileName))) {
							Directory.CreateDirectory(Path.GetDirectoryName(targetItem.FileName));
						}
						ConvertFile(fileItem, targetItem);
					}
					targetProject.Items.Add(targetItem);
				} else {
					// Adding the same item to two projects is only allowed because we will save and reload
					// the target project.
					targetProject.Items.Add(item);
				}
			}
		}
		
		protected StringBuilder conversionLog;
		
		public override void Run()
		{
			conversionLog = new StringBuilder();
			conversionLog.AppendLine("SharpDevelop Project Converter");
			conversionLog.AppendLine("==============================");
			conversionLog.AppendLine("");
			IProject sourceProject = ProjectService.CurrentProject;
			string targetProjectDirectory = sourceProject.Directory + ".ConvertedTo" + TargetLanguageName;
			if (Directory.Exists(targetProjectDirectory)) {
				MessageService.ShowMessage(targetProjectDirectory + " already exists, cannot convert.");
				return;
			}
			conversionLog.AppendLine("Source: " + sourceProject.Directory);
			conversionLog.AppendLine("Target: " + targetProjectDirectory);
			
			Directory.CreateDirectory(targetProjectDirectory);
			IProject targetProject = CreateProject(targetProjectDirectory, sourceProject);
			CopyProperties(sourceProject, targetProject);
			conversionLog.AppendLine();
			CopyItems(sourceProject, targetProject);
			conversionLog.AppendLine();
			AfterConversion(targetProject);
			conversionLog.AppendLine("Conversion complete.");
			targetProject.Save();
			targetProject.Dispose();
			TreeNode node = ProjectBrowserPad.Instance.SelectedNode;
			if (node == null) {
				node = ProjectBrowserPad.Instance.SolutionNode;
			}
			while (node != null) {
				if (node is ISolutionFolderNode) {
					AddExitingProjectToSolution.AddProject((ISolutionFolderNode)node, targetProject.FileName);
					ProjectService.SaveSolution();
					break;
				}
				node = node.Parent;
			}
			ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow newFileWindow;
			newFileWindow = FileService.NewFile("Conversion Results", "Text", conversionLog.ToString());
			if (newFileWindow != null) {
				newFileWindow.ViewContent.IsDirty = false;
			}
		}
	}
	
	public abstract class NRefactoryLanguageConverter : LanguageConverter
	{
		protected abstract void ConvertAst(CompilationUnit compilationUnit, List<ISpecial> specials);
		
		protected void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem,
		                           string sourceExtension, string targetExtension,
		                           SupportedLanguage sourceLanguage, IOutputASTVisitor outputVisitor)
		{
			FixExtensionOfExtraProperties(targetItem, sourceExtension, targetExtension);
			if (sourceExtension.Equals(Path.GetExtension(sourceItem.FileName), StringComparison.OrdinalIgnoreCase)) {
				string code = ParserService.GetParseableFileContent(sourceItem.FileName);
				IParser p = ParserFactory.CreateParser(sourceLanguage, new StringReader(code));
				p.Parse();
				if (p.Errors.count > 0) {
					conversionLog.AppendLine();
					conversionLog.AppendLine(sourceItem.FileName + " is not converted:");
					conversionLog.AppendLine("Parser found " + p.Errors.count + " error(s)");
					conversionLog.AppendLine(p.Errors.ErrorOutput);
					base.ConvertFile(sourceItem, targetItem);
					return;
				}
				
				List<ISpecial> specials = p.Lexer.SpecialTracker.CurrentSpecials;
				
				ConvertAst(p.CompilationUnit, specials);
				
				using (SpecialNodesInserter.Install(specials, outputVisitor)) {
					outputVisitor.Visit(p.CompilationUnit, null);
				}
				
				p.Dispose();
				
				if (outputVisitor.Errors.count > 0) {
					conversionLog.AppendLine();
					conversionLog.AppendLine(outputVisitor.Errors.count + " error(s) converting " + sourceItem.FileName + ":");
					conversionLog.AppendLine(outputVisitor.Errors.ErrorOutput);
				}
				
				targetItem.Include = Path.ChangeExtension(targetItem.Include, targetExtension);
				File.WriteAllText(targetItem.FileName, outputVisitor.Text);
			} else {
				base.ConvertFile(sourceItem, targetItem);
			}
		}
	}
}
