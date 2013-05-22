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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp
{
	[TestFixture]
	public class DepthFirstVisitorTests
	{
		sealed class CollectNodesVisitor : DepthFirstAstVisitor
		{
			internal List<AstNode> nodes = new List<AstNode>();
			
			protected override void VisitChildren(AstNode node)
			{
				nodes.Add(node);
				base.VisitChildren(node);
			}
		}
		
		[Test]
		public void TestCollectNodes()
		{
			var cu = new CSharpParser().Parse("using System;\nclass Test {\n  int field; // comment\n}");
			CollectNodesVisitor v = new CollectNodesVisitor();
			cu.AcceptVisitor(v);
			Assert.AreEqual(cu.DescendantsAndSelf.ToList(), v.nodes);
		}
	}
}
