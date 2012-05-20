// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class ProjectContentHelper
	{
		public IProjectContent FakeProjectContent;
		
		public ProjectContentHelper()
		{
			FakeProjectContent = MockRepository.GenerateStub<IProjectContent>();
		}
		
		public IClass AddClassToProjectContent(string className)
		{
			IClass fakeClass = AddClassToProjectContentCommon(className);
			fakeClass.Stub(c => c.ClassType).Return(ClassType.Class);
			
			return fakeClass;
		}

		IClass AddClassToProjectContentCommon(string className)
		{
			IClass fakeClass = MockRepository.GenerateMock<IClass, IEntity>();
			FakeProjectContent.Stub(pc => pc.GetClass(className, 0)).Return(fakeClass);
			fakeClass.Stub(c => c.FullyQualifiedName).Return(className);
			return fakeClass;
		}
		
		public IClass AddInterfaceToProjectContent(string interfaceName)
		{
			IClass fakeClass = AddClassToProjectContentCommon(interfaceName);
			fakeClass.Stub(c => c.ClassType).Return(ClassType.Interface);
			return fakeClass;
		}
	}
}
