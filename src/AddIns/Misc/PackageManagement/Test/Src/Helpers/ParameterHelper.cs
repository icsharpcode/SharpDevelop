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
//	public class ParameterHelper
//	{
//		public IParameter Parameter;
//		
//		public ParameterHelper()
//		{
//			Parameter = MockRepository.GenerateStub<IParameter>();
//		}
//		
//		public void MakeOptionalParameter()
//		{
//			Parameter.Stub(p => p.IsOptional).Return(true);
//		}
//		
//		public void MakeOutParameter()
//		{
//			Parameter.Stub(p => p.IsOut).Return(true);
//		}
//		
//		public void MakeRefParameter()
//		{
//			Parameter.Stub(p => p.IsRef).Return(true);
//		}
//		
//		public void MakeParamArrayParameter()
//		{
//			Parameter.Stub(p => p.IsParams).Return(true);
//		}
//		
//		public void MakeInParameter()
//		{
//			Parameter.Stub(p => p.Modifiers).Return(ParameterModifiers.In);
//		}
//		
//		public void AddAttributeToParameter(string attributeTypeName)
//		{
//			var attributeHelper = new AttributeHelper();
//			attributeHelper.CreateAttribute(attributeTypeName);
//			attributeHelper.AddAttributeToParameter(Parameter);
//		}
//	}
//}
