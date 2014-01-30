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
