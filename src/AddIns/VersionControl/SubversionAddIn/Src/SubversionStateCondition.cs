// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.Svn.Commands;
using SharpSvn;

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
				return RegisterEventsCommand.CanBeVersionControlledFile(node.FileName);
			}
			DirectoryNode dir = ProjectBrowserPad.Instance.SelectedNode as DirectoryNode;
			if (dir != null) {
				return Commands.RegisterEventsCommand.CanBeVersionControlledDirectory(dir.Directory);
			}
			SolutionNode sol = ProjectBrowserPad.Instance.SelectedNode as SolutionNode;
			if (sol != null) {
				return Commands.RegisterEventsCommand.CanBeVersionControlledDirectory(sol.Solution.Directory);
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
				return Test(condition, node.FileName, false);
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
				return Test(condition, dir.Directory, true);
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
				return Test(condition, sol.Solution.Directory, true);
			}
			return false;
		}
		
		bool Test(Condition condition, string fileName, bool isDirectory)
		{
			string[] allowedStatus = condition.Properties["state"].Split(';');
			if (allowedStatus.Length == 0 || (allowedStatus.Length == 1 && allowedStatus[0].Length == 0)) {
				return true;
			}
			string status;
			if (isDirectory ? RegisterEventsCommand.CanBeVersionControlledDirectory(fileName)
			    : RegisterEventsCommand.CanBeVersionControlledFile(fileName))
			{
				status = OverlayIconManager.GetStatus(fileName).ToString();
			} else {
				status = "Unversioned";
			}
			/*if (status == "Unversioned") {
				PropertyDictionary pd = SvnClient.Instance.Client.PropGet("svn:ignore", Path.GetDirectoryName(fileName), Revision.Working, Recurse.None);
				if (pd != null) {
					string shortFileName = Path.GetFileName(fileName);
					foreach (Property p in pd.Values) {
						using (StreamReader r = new StreamReader(new MemoryStream(p.Data))) {
							string line;
							while ((line = r.ReadLine()) != null) {
								if (string.Equals(line, shortFileName, StringComparison.OrdinalIgnoreCase)) {
									status = "Ignored";
									break;
								}
							}
						}
					}
				}
			}*/
			//LoggingService.Debug("Status of " + fileName + " is " + status);
			return Array.IndexOf(allowedStatus, status) >= 0;
		}
	}
}
