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

//using System;
//using ICSharpCode.SharpDevelop.Dom;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.Helpers
//{
//	public class ReturnTypeHelper
//	{
//		public IReturnType ReturnType;
//		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
//		
//		public void CreateReturnType(string fullyQualifiedName)
//		{
//			ReturnType = MockRepository.GenerateStub<IReturnType>();
//			ReturnType.Stub(b => b.FullyQualifiedName).Return(fullyQualifiedName);
//		}
//		
//		public void AddDotNetName(string name)
//		{
//			ReturnType.Stub(t => t.DotNetName).Return(name);
//		}
//		
//		public void AddShortName(string name)
//		{
//			ReturnType.Stub(t => t.Name).Return(name);
//		}
//		
//		public void AddUnderlyingClass(IClass c)
//		{
//			ReturnType.Stub(t => t.GetUnderlyingClass()).Return(c);
//		}
//		
//		public void MakeReferenceType()
//		{
//			ReturnType.Stub(t => t.IsReferenceType).Return(true);
//		}
//	}
//}
