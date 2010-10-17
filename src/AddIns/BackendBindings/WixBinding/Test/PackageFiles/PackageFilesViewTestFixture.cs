// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class PackageFilesViewTestFixture
	{
		PackageFilesView packageFilesView;
		WixProject project;
		MockWorkbench mockWorkbench;
		MockWixPackageFilesControl mockPackageFilesControl;
		WixDocument document;
		MockXmlTextWriter xmlTextWriter;
		MockTextEditorOptions textEditorOptions;
		FileNameEventArgs fileUtilityFileSavedEventArgs;
		
		[SetUp]
		public void Init()
		{
			project = WixBindingTestsHelper.CreateEmptyWixProject();
			mockWorkbench = new MockWorkbench();
			mockPackageFilesControl = new MockWixPackageFilesControl();
			textEditorOptions = new MockTextEditorOptions();
			textEditorOptions.ConvertTabsToSpaces = false;
			
			xmlTextWriter = new MockXmlTextWriter(textEditorOptions);
			packageFilesView = new PackageFilesView(project, mockWorkbench, mockPackageFilesControl, xmlTextWriter);
			
			mockWorkbench.ActiveContent = packageFilesView;
			
			document = new WixDocument(project, new DefaultFileLoader());
		}
		
		[TearDown]
		public void TearDown()
		{
			packageFilesView.Dispose();
		}
		
		[Test]
		public void WixProjectPassedToPackageFilesViewConstructorSoPackageFilesViewIsForProjectReturnsTrue()
		{
			Assert.IsTrue(packageFilesView.IsForProject(project));
		}
		
		[Test]
		public void PackageFilesViewTitleNameIsSet()
		{
			Assert.AreEqual("${res:ICSharpCode.WixBinding.PackageFilesView.Title}", packageFilesView.TitleName);
		}
		
		[Test]
		public void PackageFilesViewControlIsWixPackageFilesControl()
		{
			Assert.AreSame(mockPackageFilesControl, packageFilesView.Control);
		}
		
		[Test]
		public void DisposingPackageFilesViewDisposesWixPackageFilesControl()
		{
			packageFilesView.Dispose();
			
			Assert.IsTrue(mockPackageFilesControl.IsDisposed);
		}
		
		[Test]
		public void RaisingDirtyChangedEventOnWixPackageFilesControlFiresPackageFilesViewIsDirtyChangedEvent()
		{
			bool dirtyChangedEventFired = false;
			packageFilesView.IsDirtyChanged += delegate (object source, EventArgs e)
				{ dirtyChangedEventFired = true; };
			
			mockPackageFilesControl.RaiseDirtyChangedEvent();
			
			Assert.IsTrue(dirtyChangedEventFired);
		}
		
		[Test]
		public void PackageFilesViewIsDirtyReturnsWixPackageFilesControlIsDirtyValue()
		{
			mockPackageFilesControl.IsDirty = true;
			
			Assert.IsTrue(packageFilesView.IsDirty);
		}
		
		[Test]
		public void PackageFilesViewIsDirtyIsInitiallyFalse()
		{
			Assert.IsFalse(packageFilesView.IsDirty);
		}
		
		[Test]
		public void SaveMethodCallsWixPackageFilesControlSaveMethod()
		{
			packageFilesView.Save();
			
			Assert.IsTrue(mockPackageFilesControl.SaveMethodCalled);
		}
		
		[Test]
		public void PackageFilesViewWriteMethodFiresFileUtilityFileSavedEvent()
		{
			try {
				fileUtilityFileSavedEventArgs = null;
				FileUtility.FileSaved += FileUtilityFileSaved;
				
				string fileName = @"d:\projects\test\setup.wxs";
				WixDocument document = new WixDocument();
				document.FileName = fileName;
				packageFilesView.Write(document);
				
				Assert.AreEqual(fileName, fileUtilityFileSavedEventArgs.FileName.ToString());
			} finally {
				FileUtility.FileSaved -= FileUtilityFileSaved;
			}
		}
		
		void FileUtilityFileSaved(object sender, FileNameEventArgs e)
		{
			fileUtilityFileSavedEventArgs = e;
		}
		
		[Test]
		public void AddElementCallsWixPackageFilesAddElementMethod()
		{
			DerivedAddElementCommand command = new DerivedAddElementCommand("name", mockWorkbench);
			command.CallOnClick();
			Assert.AreEqual("name", mockPackageFilesControl.AddElementNameParameter);
		}
		
		[Test]
		public void AddElementCommandDoesNotThrowNullReferenceExceptionWhenWhenNoActivePackageFilesView()
		{
			mockWorkbench.ActiveContent = null;
			DerivedAddElementCommand command = new DerivedAddElementCommand("name", mockWorkbench);
			
			Assert.DoesNotThrow(delegate { command.CallOnClick(); });
		}
		
		[Test]
		public void RemoveSelectedElementCallsWixPackageFilesRemoveSelectedElementMethod()
		{
			RemoveElementCommand command = new RemoveElementCommand(mockWorkbench);
			command.Run();
			Assert.IsTrue(mockPackageFilesControl.RemoveSelectedElementMethodCalled);
		}
		
		[Test]
		public void AddFilesCallsWixPackageFilesAddFilesMethod()
		{
			AddFilesCommand command = new AddFilesCommand(mockWorkbench);
			command.Run();
			Assert.IsTrue(mockPackageFilesControl.AddFilesMethodCalled);
		}
		
		[Test]
		public void AddDirectoryCallsWixPackageFilesAddDirectoryMethod()
		{
			AddDirectoryCommand command = new AddDirectoryCommand(mockWorkbench);
			command.Run();
			Assert.IsTrue(mockPackageFilesControl.AddDirectoryMethodCalled);
		}
		
		[Test]
		public void CalculateDiffCallsWixPackageFilesShowDiffMethod()
		{
			ShowDiffCommand command = new ShowDiffCommand(mockWorkbench);
			command.Run();
			Assert.IsTrue(mockPackageFilesControl.CalculateDiffMethodCalled);
		}
		
		[Test]
		public void ShowDiffCommandDoesNotThrowNullReferenceExceptionWhenNoActivePackageFilesView()
		{
			mockWorkbench.ActiveContent = null;
			ShowDiffCommand command = new ShowDiffCommand(mockWorkbench);
			Assert.DoesNotThrow(delegate { command.Run(); });
		}
		
		[Test]
		public void HideDiffSetsWixPackageFilesIsDiffVisibleToFalse()
		{
			mockPackageFilesControl.IsDiffVisible = true;
			
			HideDiffCommand command = new HideDiffCommand(mockWorkbench);
			command.Run();
			
			Assert.IsFalse(mockPackageFilesControl.IsDiffVisible);
		}
		
		[Test]
		public void ShowFilesMethodProjectParameterIsProjectPassedToPackageFilesViewOnCreation()
		{
			packageFilesView.ShowFiles();
			Assert.AreSame(project, mockPackageFilesControl.ShowFilesMethodProjectParameter);
		}
		
		[Test]
		public void ShowFilesMethodWixDocumentWriterParameterIsPackageFilesViewObject()
		{
			packageFilesView.ShowFiles();
			Assert.AreSame(packageFilesView, mockPackageFilesControl.ShowFilesMethodDocumentWriterParameter);
		}
		
		[Test]
		public void ShowFilesMethodTextFileReaderParameterIsNotNull()
		{
			packageFilesView.ShowFiles();
			Assert.IsNotNull(mockPackageFilesControl.ShowFilesMethodFileReaderParameter);
		}
		
		[Test]
		public void WriteWixDocumentSetsPackageFilesControlIsDirtyToFalse()
		{
			mockPackageFilesControl.IsDirty = true;
			packageFilesView.Write(document);
			Assert.IsFalse(mockPackageFilesControl.IsDirty);
		}
		
		[Test]
		public void WriteWixDocumentSavesDocumentToDiskUsingTextEditorProperties()
		{
			packageFilesView.Write(document);
			
			XmlWriterSettings expectedSettings = new XmlWriterSettings();
			expectedSettings.CloseOutput = true;
			expectedSettings.Indent = true;
			expectedSettings.IndentChars = "\t";
			expectedSettings.NewLineChars = "\r\n";
			expectedSettings.OmitXmlDeclaration = true;
			
			XmlWriterSettingsComparison comparison = new XmlWriterSettingsComparison();
			Assert.IsTrue(comparison.AreEqual(expectedSettings, xmlTextWriter.XmlWriterSettingsPassedToCreateMethod), 
				comparison.ToString());
		}
		
		[Test]
		public void WriteWixDocumentSavesDocumentToDiskUsingTextEditorPropertiesWhenConvertTabsToSpacesIsTrue()
		{
			textEditorOptions.ConvertTabsToSpaces = true;
			textEditorOptions.IndentationSize = 4;
			
			packageFilesView.Write(document);
			
			XmlWriterSettings expectedSettings = new XmlWriterSettings();
			expectedSettings.CloseOutput = true;
			expectedSettings.Indent = true;
			expectedSettings.IndentChars = "    ";
			expectedSettings.NewLineChars = "\r\n";
			expectedSettings.OmitXmlDeclaration = true;
			
			XmlWriterSettingsComparison comparison = new XmlWriterSettingsComparison();
			Assert.IsTrue(comparison.AreEqual(expectedSettings, xmlTextWriter.XmlWriterSettingsPassedToCreateMethod), 
				comparison.ToString());
		}
	}
}
