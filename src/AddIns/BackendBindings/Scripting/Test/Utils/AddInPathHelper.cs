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

//using System;
//using System.IO;
//using ICSharpCode.Core;
//
//namespace ICSharpCode.Scripting.Tests.Utils
//{
//	public class AddInPathHelper
//	{
//		string addInName;
//		string identity;
//		string fileName;
//		
//		public AddInPathHelper(string addInName)
//		{
//			this.addInName = addInName;
//			identity = String.Format("ICSharpCode.{0}", addInName);
//			fileName = String.Format(@"C:\SharpDevelop\AddIns\{0}\{0}.addin", addInName);
//		}
//		
//		public AddIn CreateDummyAddInInsideAddInTree()
//		{
//			RemoveOldAddIn();
//			AddIn addin = CreateAddIn();
//			AddInTree.InsertAddIn(addin);
//			return addin;
//		}
//		
//		void RemoveOldAddIn()
//		{
//			AddIn oldAddin = FindOldAddIn();
//			if (oldAddin != null) {
//				AddInTree.RemoveAddIn(oldAddin);
//			}
//		}
//		
//		AddIn FindOldAddIn()
//		{
//			foreach (AddIn addin in AddInTree.AddIns) {
//				if (addin.Manifest.PrimaryIdentity == identity) {
//					return addin;
//				}
//			}
//			return null;
//		}
//		
//		AddIn CreateAddIn()
//		{
//			string xml = GetAddInXml();
//			AddIn addin = AddIn.Load(new StringReader(xml));
//			addin.FileName = fileName;
//			return addin;
//		}
//		
//		string GetAddInXml()
//		{
//			string format =
//				"<AddIn name='{0}' author= '' copyright='' description=''>\r\n" +
//				"    <Manifest>\r\n" +
//				"        <Identity name='{1}'/>\r\n" +
//				"    </Manifest>\r\n" +
//				"</AddIn>";
//
//			return String.Format(format, addInName, identity);
//		}
//	}
//}
