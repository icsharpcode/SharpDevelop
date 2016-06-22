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

//using ICSharpCode.NRefactory;
//using ICSharpCode.SharpDevelop.Dom;
//using ICSharpCode.SharpDevelop.Gui;
//using ICSharpCode.WixBinding;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.IO;
//using System.Xml;
//
//namespace WixBinding.Tests.Document
//{
//	/// <summary>
//	/// Tests the WixDocument class reads two dialogs from the Wix file using
//	/// the WixDocument.GetDialogIds method.
//	/// </summary>
//	[TestFixture]
//	public class GetTwoDialogIdsFromWixDocumentTestFixture
//	{
//		ReadOnlyCollection<string> dialogIds;
//		string welcomeDialogId;
//		string progressDialogId;
//		Location welcomeDialogLocation;
//		Location missingDialogLocation;
//		
//		[TestFixtureSetUpAttribute]
//		public void SetUpFixture()
//		{
//			WixDocumentReader wixReader = new WixDocumentReader(GetWixXml());
//			dialogIds = wixReader.GetDialogIds();
//			welcomeDialogId = dialogIds[0];
//			progressDialogId = dialogIds[1];
//			
//			wixReader = new WixDocumentReader(GetWixXml());
//			welcomeDialogLocation = wixReader.GetStartElementLocation("Dialog", welcomeDialogId);
//		
//			wixReader = new WixDocumentReader(GetWixXml());
//			missingDialogLocation = wixReader.GetStartElementLocation("Dialog", "missingDialogId");
//		}
//		
//		[Test]
//		public void HasTwoDialogs()
//		{
//			Assert.AreEqual(2, dialogIds.Count);
//		}
//		
//		[Test]
//		public void WelcomeDialogId()
//		{
//			Assert.AreEqual("WelcomeDialog", welcomeDialogId);
//		}
//		
//		[Test]
//		public void ProgressDialogId()
//		{
//			Assert.AreEqual("ProgressDialog", progressDialogId);
//		}
//		
//		[Test]
//		public void WelcomeDialogElementLine()
//		{
//			Assert.AreEqual(9, welcomeDialogLocation.Y);
//		}
//		
//		[Test]
//		public void WelcomeDialogElementColumn()
//		{
//			Assert.AreEqual(4, welcomeDialogLocation.X);
//		}
//		
//		[Test]
//		public void MissingDialogId()
//		{
//			Assert.IsTrue(missingDialogLocation.IsEmpty);
//		}
//
//		string GetWixXml()
//		{
//			return "<Wix xmlns='http://schemas.microsoft.com/wix/2006/wi'>\r\n" +
//				"\t<Product Name='DialogTest' \r\n" +
//				"\t         Version='1.0' \r\n" +
//				"\t         Language='1013' \r\n" +
//				"\t         Manufacturer='#develop' \r\n" +
//				"\t         Id='????????-????-????-????-????????????'>\r\n" +
//				"\t\t<Package/>\r\n" +
//				"\t\t<UI>\r\n" +
//				"\t\t\t<Dialog Id='WelcomeDialog' Height='100' Width='200'/>\r\n" +
//				"\t\t\t<Dialog Id='ProgressDialog' Height='150' Width='250'/>\r\n" +
//				"\t\t</UI>\r\n" +
//				"\t</Product>\r\n" +
//				"</Wix>";
//		}
//	}
//}
