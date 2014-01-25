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
//using ICSharpCode.PackageManagement.EnvDTE;
//using ICSharpCode.SharpDevelop.Dom;
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//
//namespace PackageManagement.Tests.EnvDTE
//{
//	[TestFixture]
//	public class CodeDelegateTests
//	{
//		CodeDelegate codeDelegate;
//		ProjectContentHelper helper;
//		IClass fakeDelegate;
//		
//		[SetUp]
//		public void Init()
//		{
//			helper = new ProjectContentHelper();
//		}
//		
//		void CreatePublicDelegate(string name)
//		{
//			fakeDelegate = helper.AddPublicDelegateToProjectContent(name);
//			CreateDelegate();
//		}
//		
//		void CreatePrivateDelegate(string name)
//		{
//			fakeDelegate = helper.AddPrivateDelegateToProjectContent(name);
//			CreateDelegate();
//		}
//		
//		void CreateDelegate()
//		{
//			codeDelegate = new CodeDelegate(helper.ProjectContent, fakeDelegate);
//		}
//
//		[Test]
//		public void Access_PublicDelegate_ReturnsPublic()
//		{
//			CreatePublicDelegate("MyDelegate");
//			
//			global::EnvDTE.vsCMAccess access = codeDelegate.Access;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
//		}
//		
//		[Test]
//		public void Access_PrivateDelegate_ReturnsPrivate()
//		{
//			CreatePrivateDelegate("MyDelegate");
//			
//			global::EnvDTE.vsCMAccess access = codeDelegate.Access;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
//		}
//		
//		[Test]
//		public void Kind_PublicDelegate_ReturnsClass()
//		{
//			CreatePublicDelegate("MyDelegate");
//			
//			global::EnvDTE.vsCMElement kind = codeDelegate.Kind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementDelegate, kind);
//		}
//	}
//}
