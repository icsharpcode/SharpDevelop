/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 14.01.2006
 * Time: 14:10
 */

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
			
		}
		
		protected virtual void CopyItems(IProject sourceProject, IProject targetProject)
		{
			foreach (ProjectItem item in sourceProject.Items) {
				FileProjectItem fileItem = item as FileProjectItem;
				if (fileItem != null && FileUtility.IsBaseDirectory(sourceProject.Directory, fileItem.FileName)) {
					FileProjectItem targetItem = new FileProjectItem(targetProject, fileItem.ItemType);
					fileItem.CopyExtraPropertiesTo(targetItem);
					targetItem.Include = fileItem.Include;
					if (!Directory.Exists(Path.GetDirectoryName(targetItem.FileName))) {
						Directory.CreateDirectory(Path.GetDirectoryName(targetItem.FileName));
					}
					ConvertFile(fileItem, targetItem);
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
			while (node != null) {
				if (node is ISolutionFolderNode) {
					AddExitingProjectToSolution.AddProject((ISolutionFolderNode)node, targetProject.FileName);
					ProjectService.SaveSolution();
					break;
				}
				node = node.Parent;
			}
			FileService.NewFile("Conversion Results", "Text", conversionLog.ToString());
		}
	}
	
	public abstract class NRefactoryLanguageConverter : LanguageConverter
	{
		protected abstract void ConvertAst(CompilationUnit compilationUnit, List<ISpecial> specials);
		
		protected void ConvertFile(FileProjectItem sourceItem, FileProjectItem targetItem,
		                           string sourceExtension, string targetExtension,
		                           SupportedLanguage sourceLanguage, IOutputASTVisitor outputVisitor)
		{
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
				
				SpecialNodesInserter sni = new SpecialNodesInserter(specials,
				                                                    new SpecialOutputVisitor(outputVisitor.OutputFormatter));
				outputVisitor.NodeTracker.NodeVisiting += sni.AcceptNodeStart;
				outputVisitor.NodeTracker.NodeVisited  += sni.AcceptNodeEnd;
				outputVisitor.NodeTracker.NodeChildrenVisited += sni.AcceptNodeEnd;
				outputVisitor.Visit(p.CompilationUnit, null);
				sni.Finish();
				
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
