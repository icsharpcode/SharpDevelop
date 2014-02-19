// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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
using System.Reflection;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp
{
	[TestFixture]
	public class AstStructureTests
	{
		[Test]
		public void RolesAreStaticReadOnly ()
		{
			foreach (Type type in typeof(AstNode).Assembly.GetExportedTypes()) {
				if (type.IsSubclassOf (typeof(AstNode))) {
					foreach (FieldInfo field in type.GetFields()) {
						if (field.FieldType.IsSubclassOf(typeof(Role))) {
							Assert.IsTrue(field.IsPublic, field + " should be public");
							Assert.IsTrue(field.IsStatic, field + " should be static");
							Assert.IsTrue(field.IsInitOnly, field + " should be readonly");
							Assert.That(field.Name, Is.StringEnding("Role"));
							Assert.IsNotNull(field.GetValue(null), field + " should not have null value");
						}
					}
				}
			}
		}
		
		[Test]
		public void AstNodesDoNotDeriveFromEachOther()
		{
			// Ast nodes should derive only from abstract classes; not from concrete types.
			// For example, we want to avoid that an AST consumer doing "if (node is PropertyDeclaration)"
			// unknowingly also handles IndexerDeclarations.
			foreach (Type type in typeof(AstNode).Assembly.GetExportedTypes()) {
				if (type == typeof(CSharpModifierToken)) // CSharpModifierToken is the exception (though I'm not too happy about that)
					continue;
				if (typeof(PreProcessorDirective).IsAssignableFrom (type)) // another exception - is it useful or not ?
					continue;
				if (type.IsSubclassOf(typeof(AstNode))) {
					Assert.IsTrue(type.BaseType.IsAbstract, type.FullName);
				}
			}
		}
		
		[Test]
		public void NullNodesCallVisitNullNode()
		{
			foreach (Type type in typeof(AstNode).Assembly.GetExportedTypes()) {
				if (type.IsSubclassOf(typeof(AstNode))) {
					var nullField = type.GetField("Null");
					if (nullField != null) {
						AstNode nullNode = (AstNode)nullField.GetValue(null);
						Assert.IsTrue(nullNode.IsNull, nullNode.GetType().Name + " should be a null node");
						var v1 = new VisitNullNodeTest();
						var v2 = new VisitNullNodeTest<string>();
						var v3 = new VisitNullNodeTest<string, string>();
						nullNode.AcceptVisitor(v1);
						nullNode.AcceptVisitor(v2);
						nullNode.AcceptVisitor(v3, null);
						Assert.IsTrue(v1.called, nullNode.GetType().Name + " should call 'void VisitNullNode();'");
						Assert.IsTrue(v2.called, nullNode.GetType().Name + " should call 'T VisitNullNode();'");
						Assert.IsTrue(v3.called, nullNode.GetType().Name + " should call 'S VisitNullNode(T data);'");
					}
				}
			}
		}
		
		class VisitNullNodeTest : DepthFirstAstVisitor 
		{
			internal bool called;
			public override void VisitNullNode(AstNode nullNode)
			{
				called = true;
			}
		}
		
		class VisitNullNodeTest<T> : DepthFirstAstVisitor<T> 
		{
			internal bool called;
			public override T VisitNullNode(AstNode nullNode)
			{
				called = true;
				return base.VisitNullNode(nullNode);
			}
		}
		
		class VisitNullNodeTest<T, S> : DepthFirstAstVisitor<T, S> 
		{
			internal bool called;
			public override S VisitNullNode(AstNode nullNode, T data)
			{
				called = true;
				return base.VisitNullNode(nullNode, data);
			}
		}
	}
}
