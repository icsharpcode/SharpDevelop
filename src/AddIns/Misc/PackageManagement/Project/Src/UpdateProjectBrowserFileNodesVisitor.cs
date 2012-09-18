// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using DTE = ICSharpCode.PackageManagement.EnvDTE;

namespace ICSharpCode.PackageManagement
{
	public class UpdateProjectBrowserFileNodesVisitor : ProjectBrowserTreeNodeVisitor
	{
		ProjectItemEventArgs projectItemEventArgs;
		FileProjectItem newFileAddedToProject;
		string directoryForNewFileAddedToProject;
		
		public UpdateProjectBrowserFileNodesVisitor(ProjectItemEventArgs projectItemEventArgs)
		{
			this.projectItemEventArgs = projectItemEventArgs;
			this.newFileAddedToProject = projectItemEventArgs.ProjectItem as FileProjectItem;
		}
		
		string DirectoryForNewFileAddedToProject {
			get {
				if (directoryForNewFileAddedToProject == null) {
					directoryForNewFileAddedToProject = Path.GetDirectoryName(newFileAddedToProject.FileName);
				}
				return directoryForNewFileAddedToProject;
			}
		}
		
		public override object Visit(ProjectNode projectNode, object data)
		{
			if (IsFileAddedInProject(projectNode)) {
				return Visit((DirectoryNode)projectNode, data);
			}
			return null;
		}
		
		public override object Visit(DirectoryNode directoryNode, object data)
		{
			if (!ShouldVisitDirectoryNode(directoryNode))
				return null;
			
			if (IsImmediateParentForNewFile(directoryNode)) {
				if (IsNewFileIsDependentUponAnotherFile()) {
					base.Visit(directoryNode, data);
				} else if (IsChildFileNodeMissingForNewFile(directoryNode)) {
					AddFileOrDirectoryNodeTo(directoryNode);
				}
			} else if (IsChildDirectoryNodeMissingForNewFile(directoryNode)) {
				AddChildDirectoryNodeForNewFileTo(directoryNode);
			} else {
				return base.Visit(directoryNode, data);
			}
			return null;
		}
		
		bool ShouldVisitDirectoryNode(DirectoryNode directoryNode)
		{
			return directoryNode.IsInitialized && IsNewFileInsideDirectory(directoryNode);
		}
		
		bool IsNewFileInsideDirectory(DirectoryNode directoryNode)
		{
			return FileUtility.IsBaseDirectory(directoryNode.Directory, DirectoryForNewFileAddedToProject);
		}
		
		bool IsFileAddedInProject(ProjectNode projectNode)
		{
			return projectNode.Project == newFileAddedToProject.Project;
		}
		
		bool IsImmediateParentForNewFile(DirectoryNode directoryNode)
		{
			return FileUtility.IsBaseDirectory(DirectoryForNewFileAddedToProject, directoryNode.Directory);
		}
		
		bool IsNewFileIsDependentUponAnotherFile()
		{
			return !String.IsNullOrEmpty(newFileAddedToProject.DependentUpon);
		}
		
		string GetDirectoryForFileAddedToProject()
		{
			return Path.GetDirectoryName(newFileAddedToProject.FileName);
		}
		
		void AddChildDirectoryNodeForNewFileTo(DirectoryNode parentNode)
		{
			string childDirectory = GetMissingChildDirectory(parentNode.Directory);
			AddDirectoryNodeTo(parentNode, childDirectory);
		}
		
		string GetMissingChildDirectory(string parentDirectory)
		{
			string relativeDirectoryForNewFile = GetRelativeDirectoryForNewFile(parentDirectory);
			string childDirectoryName = GetFirstChildDirectoryName(relativeDirectoryForNewFile);
			return Path.Combine(parentDirectory, childDirectoryName);
		}
		
		string GetRelativeDirectoryForNewFile(string baseDirectory)
		{
			return FileUtility.GetRelativePath(baseDirectory, DirectoryForNewFileAddedToProject);
		}
		
		string GetFirstChildDirectoryName(string fullSubFolderPath)
		{
			return fullSubFolderPath.Split('\\').First();
		}

		void AddDirectoryNodeTo(TreeNode parentNode, string directory)
		{
			var directoryNode = new DirectoryNode(directory, FileNodeStatus.InProject);
			directoryNode.InsertSorted(parentNode);
		}

		void AddFileOrDirectoryNodeTo(DirectoryNode directoryNode)
		{
			if (newFileAddedToProject.ItemType == ItemType.Folder) {
				AddDirectoryNodeTo(directoryNode, newFileAddedToProject.FileName);
			} else {
				AddFileNodeTo(directoryNode);
			}
		}
		
		void AddFileNodeTo(TreeNode node, FileNodeStatus status = FileNodeStatus.InProject)
		{
			var fileNode = new FileNode(newFileAddedToProject.FileName, status);
			fileNode.InsertSorted(node);
		}
		
		bool IsChildFileNodeMissingForNewFile(DirectoryNode parentDirectoryNode)
		{
			return !IsChildFileNodeAlreadyAddedForNewFile(parentDirectoryNode);
		}
		
		bool IsChildFileNodeAlreadyAddedForNewFile(DirectoryNode parentDirectoryNode)
		{
			return GetChildFileNodes(parentDirectoryNode)
				.Any(childFileNode => FileNodeMatchesNewFileAdded(childFileNode));
		}
		
		bool FileNodeMatchesNewFileAdded(FileNode fileNode)
		{
			return FileUtility.IsEqualFileName(fileNode.FileName, newFileAddedToProject.FileName);
		}
		
		bool IsChildDirectoryNodeMissingForNewFile(DirectoryNode parentDirectoryNode)
		{
			return !IsChildDirectoryNodeAlreadyAddedForNewFile(parentDirectoryNode);
		}
		
		bool IsChildDirectoryNodeAlreadyAddedForNewFile(DirectoryNode parentDirectoryNode)
		{
			return GetChildDirectoryNodes(parentDirectoryNode)
				.Any(childDirectoryNode => DirectoryOfNewFileStartsWith(childDirectoryNode));
		}
		
		bool DirectoryOfNewFileStartsWith(DirectoryNode directoryNode)
		{
			return FileUtility.IsBaseDirectory(directoryNode.Directory, DirectoryForNewFileAddedToProject);
		}
		
		IEnumerable<FileNode> GetChildFileNodes(ExtTreeNode parentNode)
		{
			return parentNode.AllNodes.OfType<FileNode>();
		}
		
		IEnumerable<DirectoryNode> GetChildDirectoryNodes(ExtTreeNode parentNode)
		{
			return parentNode.AllNodes.OfType<DirectoryNode>();
		}
		
		public override object Visit(FileNode fileNode, object data)
		{
			if (IsNewFileIsDependentUponAnotherFile()) {
				if (IsImmediateParentForNewFile(fileNode)) {
					AddFileNodeTo(fileNode, FileNodeStatus.BehindFile);
					return null;
				}
			}
			return base.Visit(fileNode, data);
		}
		
		bool IsImmediateParentForNewFile(FileNode fileNode)
		{
			return DTE.FileProjectItemExtensions.IsDependentUponFileName(newFileAddedToProject, fileNode.FileName);
		}
	}
}
