// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.PythonBinding
{
	public class PythonTestRunnerResponseFile : IDisposable
	{
		TextWriter writer;
		
		public PythonTestRunnerResponseFile(string fileName)
			: this(new StreamWriter(fileName, false, Encoding.UTF8))
		{
		}
		
		public PythonTestRunnerResponseFile(TextWriter writer)
		{
			this.writer = writer;
		}
		
		public void WriteTest(string testName)
		{
			writer.WriteLine(testName);
		}
		
		public void WritePaths(string[] paths)
		{
			foreach (string path in paths) {
				WritePath(path);
			}
		}
		
		public void WritePathIfNotEmpty(string path)
		{
			if (!String.IsNullOrEmpty(path)) {
				WritePath(path);
			}
		}
		
		public void WritePath(string path)
		{
			WriteQuotedArgument("p", path);
		}
		
		void WriteQuotedArgument(string option, string value)
		{
			writer.WriteLine("/{0}:\"{1}\"", option, value);
		}
		
		public void WriteResultsFileName(string fileName)
		{
			WriteQuotedArgument("r", fileName);
		}
		
		public void Dispose()
		{
			writer.Dispose();
		}
		
		public void WriteTests(SelectedTests selectedTests)
		{
			WritePathsForReferencedProjects(selectedTests.Project);
			
			if (selectedTests.Member != null) {
				WriteTest(selectedTests.Member.FullyQualifiedName);
			} else if (selectedTests.Class != null) {
				WriteTest(selectedTests.Class.FullyQualifiedName);
			} else if (!String.IsNullOrEmpty(selectedTests.NamespaceFilter)) {
				WriteTest(selectedTests.NamespaceFilter);
			} else {
				WriteProjectTests(selectedTests.Project);
			}
			
		}
		
		void WriteProjectTests(IProject project)
		{
			if (project != null) {
				WriteProjectFileItems(project.Items);
			}
		}
		
		void WritePathsForReferencedProjects(IProject project)
		{
			if (project != null) {
				foreach (ProjectItem item in project.Items) {
					ProjectReferenceProjectItem projectRef = item as ProjectReferenceProjectItem;
					if (projectRef != null) {
						string directory = Path.GetDirectoryName(projectRef.FileName);
						WritePathIfNotEmpty(directory);
					}
				}
			}
		}
		
		void WriteProjectFileItems(ReadOnlyCollection<ProjectItem> items)
		{
			foreach (ProjectItem item in items) {
				FileProjectItem fileItem = item as FileProjectItem;
				if (fileItem != null) {
					WriteFileNameWithoutExtension(fileItem.FileName);
				}
			}
		}
		
		void WriteFileNameWithoutExtension(string fileName)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			WriteTest(fileNameWithoutExtension);
		}
	}
}
