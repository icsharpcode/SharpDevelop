// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;

namespace PackageManagement.Tests.Helpers
{
	public class FakeCodeGenerator : CodeGenerator
	{
		public FakeCodeGenerator()
		{
		}
		
		public override string GenerateCode(AbstractNode node, string indentation)
		{
			throw new NotImplementedException();
		}
		
		public DomRegion RegionPassedToInsertCodeAtEnd;
		public IRefactoringDocument DocumentPassedToInsertCodeAtEnd;
		public AbstractNode NodePassedToInsertCodeAtEnd;
		
		public override void InsertCodeAtEnd(DomRegion region, IRefactoringDocument document, params AbstractNode[] nodes)
		{
			RegionPassedToInsertCodeAtEnd = region;
			DocumentPassedToInsertCodeAtEnd = document;
			NodePassedToInsertCodeAtEnd = nodes.FirstOrDefault();
		}
	}
}
