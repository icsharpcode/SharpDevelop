// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.WixBinding;
using NUnit.Framework;

namespace WixBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockWixPackageFilesControlTests
	{
		MockWixPackageFilesControl control;
		
		[SetUp]
		public void Init()
		{
			control = new MockWixPackageFilesControl();
		}
		
		[Test]
		public void CallingDisposeSetsIsDisposedToTrue()
		{
			control.Dispose();
			Assert.IsTrue(control.IsDisposed);
		}
		
		[Test]
		public void InitialValueOfIsDisposedIsFalse()
		{
			Assert.IsFalse(control.IsDisposed);
		}
		
		[Test]
		public void RaiseDirtyChangedMethodFiresDirtyChangedEvent()
		{
			bool dirtyChangedEventFired = false;
			control.DirtyChanged += delegate (object source, EventArgs e)
				{ dirtyChangedEventFired = true; };
			
			control.RaiseDirtyChangedEvent();
			
			Assert.IsTrue(dirtyChangedEventFired);
		}
		
		[Test]
		public void IsDirtyPropertyCanBeSetAndRetrieved()
		{
			control.IsDirty = true;
			Assert.IsTrue(control.IsDirty);
		}
		
		[Test]
		public void IsDirtyIsInitiallyFalse()
		{
			Assert.IsFalse(control.IsDirty);
		}
		
		[Test]
		public void SaveMethodBeingCalledIsRecorded()
		{
			control.Save();
			Assert.IsTrue(control.SaveMethodCalled);
		}
		
		[Test]
		public void SaveMethodCalledIsInitiallyFalse()
		{
			Assert.IsFalse(control.SaveMethodCalled);
		}
		
		[Test]
		public void AddElementMethodRecordsNameParameter()
		{
			control.AddElement("name");
			Assert.AreEqual("name", control.AddElementNameParameter);
		}
		
		[Test]
		public void RemoveSelectedElementMethodBeingCalledIsRecorded()
		{
			control.RemoveSelectedElement();
			Assert.IsTrue(control.RemoveSelectedElementMethodCalled);
		}
		
		[Test]
		public void RemoveSelectedElementMethodCalledIsInitiallyFalse()
		{
			Assert.IsFalse(control.RemoveSelectedElementMethodCalled);
		}
		
		
		[Test]
		public void AddFilesMethodBeingCalledIsRecorded()
		{
			control.AddFiles();
			Assert.IsTrue(control.AddFilesMethodCalled);
		}
		
		[Test]
		public void AddFilesMethodCalledIsInitiallyFalse()
		{
			Assert.IsFalse(control.AddFilesMethodCalled);
		}
		
		[Test]
		public void AddDirectoyMethodBeingCalledIsRecorded()
		{
			control.AddDirectory();
			Assert.IsTrue(control.AddDirectoryMethodCalled);
		}
		
		[Test]
		public void AddDirectoryMethodCalledIsInitiallyFalse()
		{
			Assert.IsFalse(control.AddDirectoryMethodCalled);
		}

		[Test]
		public void CalculateDiffMethodBeingCalledIsRecorded()
		{
			control.CalculateDiff();
			Assert.IsTrue(control.CalculateDiffMethodCalled);
		}
		
		[Test]
		public void CalculateDiffMethodCalledIsInitiallyFalse()
		{
			Assert.IsFalse(control.CalculateDiffMethodCalled);
		}
		
		[Test]
		public void CanSetIsDiffVisibleToTrue()
		{
			control.IsDiffVisible = true;
			Assert.IsTrue(control.IsDiffVisible);
		}
		
		[Test]
		public void CanSetIsDiffVisibleToFalse()
		{
			control.IsDiffVisible = false;
			Assert.IsFalse(control.IsDiffVisible);
		}
		
		[Test]
		public void ShowFilesMethodProjectParameterSaved()
		{
			WixProject project = WixBindingTestsHelper.CreateEmptyWixProject();
			control.ShowFiles(project, null, null);
			Assert.AreSame(project, control.ShowFilesMethodProjectParameter);
		}
		
		[Test]
		public void ShowFilesMethodFileReaderParameterSaved()
		{
			WorkbenchTextFileReader fileReader = new WorkbenchTextFileReader();
			control.ShowFiles(null, fileReader, null);
			Assert.AreSame(fileReader, control.ShowFilesMethodFileReaderParameter);
		}
		
		[Test]
		public void ShowFilesMethodDocumentWriterParameterSaved()
		{
			MockWixDocumentWriter writer = new MockWixDocumentWriter();
			control.ShowFiles(null, null, writer);
			Assert.AreSame(writer, control.ShowFilesMethodDocumentWriterParameter);
		}
		
		[Test]
		public void WixDocumentCanBeRetrieved()
		{
			WixDocument doc = new WixDocument();
			control.Document = doc;
			Assert.AreSame(doc, control.Document);
		}
	}
}
