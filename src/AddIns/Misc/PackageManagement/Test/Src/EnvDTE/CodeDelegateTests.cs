// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeDelegateTests
	{
		CodeDelegate codeDelegate;
		ProjectContentHelper helper;
		IClass fakeDelegate;
		
		[SetUp]
		public void Init()
		{
			helper = new ProjectContentHelper();
		}
		
		void CreatePublicDelegate(string name)
		{
			fakeDelegate = helper.AddPublicDelegateToProjectContent(name);
			CreateDelegate();
		}
		
		void CreatePrivateDelegate(string name)
		{
			fakeDelegate = helper.AddPrivateDelegateToProjectContent(name);
			CreateDelegate();
		}
		
		void CreateDelegate()
		{
			codeDelegate = new CodeDelegate(helper.ProjectContent, fakeDelegate);
		}

		[Test]
		public void Access_PublicDelegate_ReturnsPublic()
		{
			CreatePublicDelegate("MyDelegate");
			
			vsCMAccess access = codeDelegate.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateDelegate_ReturnsPrivate()
		{
			CreatePrivateDelegate("MyDelegate");
			
			vsCMAccess access = codeDelegate.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void Kind_PublicDelegate_ReturnsClass()
		{
			CreatePublicDelegate("MyDelegate");
			
			vsCMElement kind = codeDelegate.Kind;
			
			Assert.AreEqual(vsCMElement.vsCMElementDelegate, kind);
		}
	}
}
