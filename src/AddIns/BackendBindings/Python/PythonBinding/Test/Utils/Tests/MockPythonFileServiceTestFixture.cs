// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Text;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class MockPythonFileServiceTestFixture
	{
		MockPythonFileService fileService;
		
		[SetUp]
		public void Init()
		{
			fileService = new MockPythonFileService();
		}
		
		[Test]
		public void GetTempFileNameReturnsReturnsTemporaryFileName()
		{
			string expectedFileName = @"c:\temp\tmp1.tmp";
			fileService.SetTempFileName(expectedFileName);
			string tempFileName = fileService.GetTempFileName();
			Assert.AreEqual(expectedFileName, tempFileName);
		}
		
		[Test]
		public void TextWriterReturnedFromCreateTextWriter()
		{
			using (StringWriter stringWriter = new StringWriter(new StringBuilder())) {
				fileService.SetTextWriter(stringWriter);
				CreateTextWriterInfo info = new CreateTextWriterInfo(@"test.tmp", Encoding.UTF8, true);
				Assert.AreEqual(stringWriter, fileService.CreateTextWriter(info));
			}
		}
		
		[Test]
		public void CreateTextWriterInfoIsSavedWhenCreateTextWriterMethodIsCalled()
		{
			fileService.CreateTextWriterInfoPassedToCreateTextWriter = null;
			CreateTextWriterInfo info =  new CreateTextWriterInfo("test.txt", Encoding.UTF8, true);
			fileService.CreateTextWriter(info);
			Assert.AreEqual(info, fileService.CreateTextWriterInfoPassedToCreateTextWriter);
		}
		
		[Test]
		public void DeleteFileSavesFileNameDeleted()
		{
			fileService.FileNameDeleted = null;
			string expectedFileName = @"c:\temp\tmp66.tmp";
			fileService.DeleteFile(expectedFileName);

			Assert.AreEqual(expectedFileName, fileService.FileNameDeleted);
		}
	}
}
