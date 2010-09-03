// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
