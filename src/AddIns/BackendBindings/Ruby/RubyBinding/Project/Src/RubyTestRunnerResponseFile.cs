// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.RubyBinding
{
	public class RubyTestRunnerResponseFile : IDisposable
	{
		TextWriter writer;
		
		public RubyTestRunnerResponseFile(string fileName)
			: this(new StreamWriter(fileName, false, Encoding.ASCII))
		{
		}
		
		public RubyTestRunnerResponseFile(TextWriter writer)
		{
			this.writer = writer;
		}
		
		public void Dispose()
		{
			writer.Dispose();
		}
		
		public void WriteTests(SelectedTests selectedTests)
		{
			if (selectedTests.Member != null) {
				WriteTestFileNameForMethod(selectedTests.Member);
			} else if (selectedTests.Class != null) {
				WriteTestFileNameForClass(selectedTests.Class);
			} else if (selectedTests.Project != null) {
				WriteTestsForProject(selectedTests.Project);
			}
		}
		
		void WriteTestFileNameForMethod(IMember method)
		{
			WriteTestFileNameForCompilationUnit(method.CompilationUnit);
		}
		
		void WriteTestFileNameForClass(IClass c)
		{
			WriteTestFileNameForCompilationUnit(c.CompilationUnit);
		}
		
		void WriteTestFileNameForCompilationUnit(ICompilationUnit unit)
		{
			WriteTestFileName(unit.FileName);
		}
		
		void WriteTestsForProject(IProject project)
		{
			WriteTestsForProjectFileItems(project.Items);
		}
		
		void WriteTestsForProjectFileItems(ReadOnlyCollection<ProjectItem> items)
		{
			foreach (ProjectItem item in items) {
				FileProjectItem fileItem = item as FileProjectItem;
				if (fileItem != null) {
					WriteTestFileName(fileItem.FileName);
				}
			}
		}
		
		void WriteTestFileName(string testFileName)
		{
			writer.WriteLine(testFileName);
		}
	}
}
