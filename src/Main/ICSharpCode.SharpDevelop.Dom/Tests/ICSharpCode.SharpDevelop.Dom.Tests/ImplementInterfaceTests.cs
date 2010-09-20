// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Dom.Tests
{
	[TestFixture]
	public class ImplementInterfaceTests
	{
		[Test]
		public void NestedInterfaceInGenericClass()
		{
			// See SD2-1626
			DefaultProjectContent pc = new DefaultProjectContent();
			pc.ReferencedContents.Add(SharedProjectContentRegistryForTests.Instance.Mscorlib);
			
			DefaultCompilationUnit cu = new DefaultCompilationUnit(pc);
			DefaultClass container = new DefaultClass(cu, "TestClass");
			container.TypeParameters.Add(new DefaultTypeParameter(container, "T", 0));
			
			DefaultClass innerClass = new DefaultClass(cu, container);
			innerClass.FullyQualifiedName = "TestClass.INestedInterface";
			innerClass.ClassType = ClassType.Interface;
			innerClass.TypeParameters.Add(new DefaultTypeParameter(innerClass, "T", 0));
			innerClass.Properties.Add(new DefaultProperty(innerClass, "P") {
			                          	ReturnType = new GenericReturnType(innerClass.TypeParameters[0]),
			                          	CanGet = true
			                          });
			container.InnerClasses.Add(innerClass);
			pc.AddClassToNamespaceList(container);
			
			DefaultClass targetClass = new DefaultClass(cu, "TargetClass");
			List<AbstractNode> nodes = new List<AbstractNode>();
			
			IReturnType interf = new SearchClassReturnType(pc, targetClass, 0, 0, "TestClass.INestedInterface", 1);
			interf = new ConstructedReturnType(interf, new IReturnType[] { SharedProjectContentRegistryForTests.Instance.Mscorlib.GetClass("System.String", 0).DefaultReturnType });
			
			CSharpCodeGenerator codeGen = new CSharpCodeGenerator();
			codeGen.ImplementInterface(nodes, interf, true, targetClass);
			
			Assert.AreEqual(1, nodes.Count);
			CSharpOutputVisitor output = new CSharpOutputVisitor();
			output.Options.IndentationChar = ' ';
			output.Options.IndentSize = 2;
			nodes[0].AcceptVisitor(output, null);
			Assert.AreEqual("string TestClass<string>.INestedInterface.P {\n  get {\n    throw new NotImplementedException();\n  }\n}", output.Text.Replace("\r", "").Trim());
		}
	}
}
