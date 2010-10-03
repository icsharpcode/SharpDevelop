// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.Tests.Ast
{
	public class LocationAssignmentCheckVisitor : NodeTrackingAstVisitor
	{
		protected override void BeginVisit(INode node)
		{
			Assert.IsFalse(node.StartLocation.IsEmpty);
			Assert.IsFalse(node.EndLocation.IsEmpty);
		}
	}
}
