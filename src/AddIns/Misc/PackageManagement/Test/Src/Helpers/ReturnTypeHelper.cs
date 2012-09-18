// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class ReturnTypeHelper
	{
		public IReturnType ReturnType;
		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
		
		public void CreateReturnType(string fullyQualifiedName)
		{
			ReturnType = MockRepository.GenerateStub<IReturnType>();
			ReturnType.Stub(b => b.FullyQualifiedName).Return(fullyQualifiedName);
		}
		
		public void AddDotNetName(string name)
		{
			ReturnType.Stub(t => t.DotNetName).Return(name);
		}
		
		public void AddShortName(string name)
		{
			ReturnType.Stub(t => t.Name).Return(name);
		}
		
		public void AddUnderlyingClass(IClass c)
		{
			ReturnType.Stub(t => t.GetUnderlyingClass()).Return(c);
		}
		
		public void MakeReferenceType()
		{
			ReturnType.Stub(t => t.IsReferenceType).Return(true);
		}
	}
}
