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
