// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeAssemblyParserService : IAssemblyParserService
	{
		public FakeReflectionProjectContent FakeReflectionProjectContent = new FakeReflectionProjectContent();
		public ReferenceProjectItem ItemPassedToGetReflectionProjectContentForReference;
		public bool IsGetReflectionProjectContentForReferenceCalled;
		
		public IReflectionProjectContent GetReflectionProjectContentForReference(ReferenceProjectItem item)
		{
			IsGetReflectionProjectContentForReferenceCalled = true;
			ItemPassedToGetReflectionProjectContentForReference = item;
			return FakeReflectionProjectContent;
		}
	}
}
