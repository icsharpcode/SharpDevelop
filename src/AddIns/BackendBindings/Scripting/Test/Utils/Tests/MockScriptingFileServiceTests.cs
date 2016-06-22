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
using System.IO;
using System.Text;

using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Utils.Tests
{
	[TestFixture]
	public class MockScriptingFileServiceTests
	{
		MockScriptingFileService fileService;
		
		[SetUp]
		public void Init()
		{
			fileService = new MockScriptingFileService();
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
