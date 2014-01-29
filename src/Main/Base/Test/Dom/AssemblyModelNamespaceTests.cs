// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom
{
	[TestFixture]
	public class AssemblyModelNamespaceTests : CSharpModelTestBase
	{
		[Test]
		public void AddUnnamedNamespace()
		{
			var topLevelChangeEventArgs = ListenForChanges(assemblyModel.TopLevelTypeDefinitions);
			var namespaceChangeEventArgs = ListenForChanges(assemblyModel.Namespaces);
			
			AddCodeFile("test.cs", "class Test {}");
			Assert.AreEqual(1, assemblyModel.TopLevelTypeDefinitions.Count);
			Assert.AreEqual(1, assemblyModel.Namespaces.Count);
			var simpleClass = assemblyModel.TopLevelTypeDefinitions.Single();
			var unnamedNamespace = assemblyModel.Namespaces.Single();
			Assert.IsNull(unnamedNamespace.ParentNamespace);
			Assert.IsEmpty(unnamedNamespace.Name);
			Assert.IsEmpty(topLevelChangeEventArgs.Single().OldItems);
			Assert.IsEmpty(namespaceChangeEventArgs.Single().OldItems);
			Assert.AreEqual(new[] { simpleClass }, topLevelChangeEventArgs.Single().NewItems);
			Assert.AreEqual(new[] { unnamedNamespace }, namespaceChangeEventArgs.Single().NewItems);
		}
		
		[Test]
		public void MoveFromUnnamedNamespaceToFirstLevel()
		{
			AddUnnamedNamespace();
			MoveToNamespace("test.cs", "FirstLevel");
		}
		
		[Test]
		public void MoveFromUnnamedNamespaceToSecondLevel()
		{
			AddUnnamedNamespace();
			MoveToNamespace("test.cs", "FirstLevel.SecondLevel");
		}
		
		[Test]
		public void MoveFromUnnamedNamespaceToThirdLevelSteps()
		{
			AddUnnamedNamespace();
			MoveToNamespace("test.cs", "FirstLevel");
			MoveToNamespace("test.cs", "FirstLevel.SecondLevel");
			MoveToNamespace("test.cs", "FirstLevel.SecondLevel.ThirdLevel");
		}
		
		[Test]
		public void MoveFromUnnamedNamespaceToSecondLevelAndOther()
		{
			AddUnnamedNamespace();
			MoveToNamespace("test.cs", "FirstLevel.SecondLevel");
			MoveToNamespace("test.cs", "FirstLevel.Nested");
		}
		
		void MoveToNamespace(string filename, string newNamespace)
		{
			var topLevelChangeEventArgs = ListenForChanges(assemblyModel.TopLevelTypeDefinitions);
			var namespaceChangeEventArgs = ListenForChanges(assemblyModel.Namespaces);
			var oldClass = assemblyModel.TopLevelTypeDefinitions.Single();
			var oldNamespace = assemblyModel.Namespaces.Single();
			UpdateCodeFile(filename, "namespace " + newNamespace + " { class Test {} }");
			Assert.AreEqual(1, assemblyModel.TopLevelTypeDefinitions.Count);
			Assert.AreEqual(1, assemblyModel.Namespaces.Count);
			var simpleClass = assemblyModel.TopLevelTypeDefinitions.Single();
			
			string[] parts = newNamespace.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
			var simpleNamespace = assemblyModel.Namespaces.Single();
			
			Assert.AreEqual(new[] {
			                	simpleNamespace
			                }, namespaceChangeEventArgs.Single().NewItems);
			Assert.AreEqual(new[] {
			                	oldNamespace
			                }, namespaceChangeEventArgs.Single().OldItems);
			
			Assert.AreEqual(newNamespace, simpleNamespace.FullName);
			
			for (int i = parts.Length - 1; i <= 0; i++) {
				Assert.IsNotNull(simpleNamespace.ParentNamespace);
				Assert.AreEqual(parts[i], simpleNamespace.Name);
				if (i == 0) {
					Assert.IsEmpty(simpleNamespace.ParentNamespace.Name);
					Assert.IsNull(simpleNamespace.ParentNamespace.ParentNamespace);
				} else {
					Assert.AreEqual(new[] { simpleNamespace }, simpleNamespace.ParentNamespace.ChildNamespaces);
				}
			}

			Assert.AreEqual(new[] {
			                	oldClass
			                }, topLevelChangeEventArgs.Single().OldItems);
			Assert.AreEqual(new[] {
			                	simpleClass
			                }, topLevelChangeEventArgs.Single().NewItems);
		}
	}
}
