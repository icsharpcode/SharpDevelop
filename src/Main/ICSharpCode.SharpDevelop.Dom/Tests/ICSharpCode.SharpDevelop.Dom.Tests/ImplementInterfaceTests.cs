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
