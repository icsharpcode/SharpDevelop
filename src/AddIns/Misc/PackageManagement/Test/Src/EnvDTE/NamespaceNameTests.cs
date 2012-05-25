// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class NamespaceNameTests
	{
		NamespaceName namespaceName;
		
		void CreateNamespaceName(string parent, string name)
		{
			namespaceName = new NamespaceName(parent, name);
		}
		
		[Test]
		public void QualifiedName_ParentNamespaceIsTest_ReturnsTestPrefix()
		{
			CreateNamespaceName("Test", "Child");
			
			string name = namespaceName.QualifiedName;
			
			Assert.AreEqual("Test.Child", name);
		}
		
		[Test]
		public void QualifiedName_ParentNamespaceIsEmptyString_ReturnsJustChildNamespaceName()
		{
			CreateNamespaceName(String.Empty, "Child");
			
			string name = namespaceName.QualifiedName;
			
			Assert.AreEqual("Child", name);
		}
	}
}
