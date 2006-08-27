// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Svn.Commands;
using NSvn.Core;
using NSvn.Common;

namespace ICSharpCode.Svn
{
	/// <summary>
	/// Gets if a folder is under version control
	/// </summary>
	public class SubversionIsControlledCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			FileNode node = ProjectBrowserPad.Instance.SelectedNode as FileNode;
			if (node != null) {
				return RegisterEventsCommand.CanBeVersionControlled(node.FileName);
			}
			DirectoryNode dir = ProjectBrowserPad.Instance.SelectedNode as DirectoryNode;
			if (dir != null) {
				return Directory.Exists(Path.Combine(dir.Directory, ".svn"));
			}
			SolutionNode sol = ProjectBrowserPad.Instance.SelectedNode as SolutionNode;
			if (sol != null) {
				return Directory.Exists(Path.Combine(sol.Solution.Directory, ".svn"));
			}
			return false;
		}
	}
	
	public class SubversionStateCondition : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			FileNode node = ProjectBrowserPad.Instance.SelectedNode as FileNode;
			if (node != null) {
				if (condition.Properties["item"] == "Folder") {
					return false;
				}
				return Test(condition, node.FileName);
			}
			DirectoryNode dir = ProjectBrowserPad.Instance.SelectedNode as DirectoryNode;
			if (dir != null) {
				if (condition.Properties["item"] == "File") {
					return false;
				}
				if (condition.Properties["state"].Contains("Modified")) {
					// Directories are not checked recursively yet.
					return true;
				}
				return Test(condition, dir.Directory);
			}
			SolutionNode sol = ProjectBrowserPad.Instance.SelectedNode as SolutionNode;
			if (sol != null) {
				if (condition.Properties["item"] == "File") {
					return false;
				}
				if (condition.Properties["state"].Contains("Modified")) {
					// Directories are not checked recursively yet.
					return true;
				}
				return Test(condition, sol.Solution.Directory);
			}
			return false;
		}
		
		static string lastTestFileName;
		static string lastTestStatus;
		
		internal static void ResetCache()
		{
			lastTestFileName = null;
		}
		
		bool Test(Condition condition, string fileName)
		{
			string[] allowedStatus = condition.Properties["state"].Split(';');
			if (allowedStatus.Length == 0) {
				return true;
			}
			string status;
			if (fileName == lastTestFileName) {
				status = lastTestStatus;
			} else {
				status = SvnClient.Instance.Client.SingleStatus(fileName).TextStatus.ToString();
				if (status == "Unversioned") {
					PropertyDictionary pd = SvnClient.Instance.Client.PropGet("svn:ignore", Path.GetDirectoryName(fileName), Revision.Working, Recurse.None);
					if (pd != null) {
						string shortFileName = Path.GetFileName(fileName);
						foreach (Property p in pd.Values) {
							using (StreamReader r = new StreamReader(new MemoryStream(p.Data))) {
								string line;
								while ((line = r.ReadLine()) != null) {
									if (string.Equals(line, shortFileName, StringComparison.InvariantCultureIgnoreCase)) {
										status = "Ignored";
										break;
									}
								}
							}
						}
					}
				}
				LoggingService.Debug("Status of " + fileName + " is " + status);
				lastTestFileName = fileName;
				lastTestStatus = status;
			}
			return Array.IndexOf(allowedStatus, status) >= 0;
		}
	}
}
