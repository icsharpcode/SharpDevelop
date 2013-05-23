// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Dom
{
	[TestFixture]
	public class ProjectEntityModelContextTests
	{
		IUnresolvedTypeDefinition CreateMockTypeDefinition(string fileName)
		{
			var typeDefinition = MockRepository.GenerateStrictMock<IUnresolvedTypeDefinition>();
			var file = MockRepository.GenerateStrictMock<IUnresolvedFile>();
			file.Stub(f => f.FileName).Return(fileName);
			typeDefinition.Stub(td => td.UnresolvedFile).Return(file);
			return typeDefinition;
		}
		
		[Test]
		public void XamlCodeBehindIsBetterThanXaml()
		{
			var context = new ProjectEntityModelContext(MockRepository.GenerateStrictMock<IProject>(), ".cs");
			Assert.IsTrue(context.IsBetterPart(CreateMockTypeDefinition("Window.xaml.cs"), CreateMockTypeDefinition("Window.xaml")));
			Assert.IsTrue(context.IsBetterPart(CreateMockTypeDefinition("Window.cs"), CreateMockTypeDefinition("Window.xaml")));
			Assert.IsFalse(context.IsBetterPart(CreateMockTypeDefinition("Window.xaml"), CreateMockTypeDefinition("Window.xaml.cs")));
			Assert.IsFalse(context.IsBetterPart(CreateMockTypeDefinition("Window.xaml"), CreateMockTypeDefinition("Window.cs")));
		}
		
		[Test]
		public void MainPartIsBetterThanDesigner()
		{
			var context = new ProjectEntityModelContext(MockRepository.GenerateStrictMock<IProject>(), ".cs");
			Assert.IsTrue(context.IsBetterPart(CreateMockTypeDefinition("Form.cs"), CreateMockTypeDefinition("Form.Designer.cs")));
			Assert.IsFalse(context.IsBetterPart(CreateMockTypeDefinition("Form.Designer.cs"), CreateMockTypeDefinition("Form.cs")));
		}
	}
}
