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
using ICSharpCode.WixBinding;

namespace WixBinding.Tests.Utils
{
	public class MockWixPackageFilesControl : IWixPackageFilesControl
	{
		bool disposed;
		bool saveMethodCalled;
		string addElementNameParameter;
		bool removeSelectedElementMethodCalled;
		bool addFilesMethodCalled;
		bool addDirectoryMethodCalled;
		bool calculateDiffMethodCalled;
		WixProject showFilesProjectParameter;
		ITextFileReader showFilesFileReaderParameter;
		IWixDocumentWriter showFilesDocumentWriterParameter;
				
		public MockWixPackageFilesControl()
		{
		}
		
		public void Dispose()
		{
			disposed = true;
		}
		
		public bool IsDisposed {
			get { return disposed; }
		}
		
		public event EventHandler DirtyChanged;

		public void RaiseDirtyChangedEvent()
		{
			DirtyChanged(this, new EventArgs());
		}
		
		public bool IsDirty { get; set; }
		
		public bool SaveMethodCalled {
			get { return saveMethodCalled; }
		}

		public void Save()
		{
			saveMethodCalled = true;
		}
		
		public string AddElementNameParameter {
			get { return addElementNameParameter; }
		}

		public void AddElement(string name)
		{
			addElementNameParameter = name;
		}
		
		public bool RemoveSelectedElementMethodCalled {
			get { return removeSelectedElementMethodCalled; }
		}
		
		public void RemoveSelectedElement()
		{
			removeSelectedElementMethodCalled = true;
		}
		
		public bool AddFilesMethodCalled {
			get { return addFilesMethodCalled; }
		}
		
		public void AddFiles()
		{
			addFilesMethodCalled = true;
		}
		
		public bool AddDirectoryMethodCalled {
			get { return addDirectoryMethodCalled; }
		}
		
		public void AddDirectory()
		{
			addDirectoryMethodCalled = true;
		}
		
		public bool CalculateDiffMethodCalled {
			get { return calculateDiffMethodCalled; }
		}
		
		public void CalculateDiff()
		{
			calculateDiffMethodCalled = true;
		}
		
		public bool IsDiffVisible { get; set; }
		
		public void ShowFiles(WixProject project, ITextFileReader fileReader, IWixDocumentWriter documentWriter)
		{
			showFilesProjectParameter = project;
			showFilesFileReaderParameter = fileReader;
			showFilesDocumentWriterParameter = documentWriter;
		}
		
		public WixProject ShowFilesMethodProjectParameter {
			get { return showFilesProjectParameter; }
		}
		
		public ITextFileReader ShowFilesMethodFileReaderParameter {
			get { return showFilesFileReaderParameter; }
		}
		
		public IWixDocumentWriter ShowFilesMethodDocumentWriterParameter {
			get { return showFilesDocumentWriterParameter; }
		}
		
		public WixDocument Document { get; set; }
	}
}
