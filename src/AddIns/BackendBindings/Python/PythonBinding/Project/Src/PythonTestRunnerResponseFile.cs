// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
