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
//using System.Collections.Generic;
//using ICSharpCode.SharpDevelop.Dom;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.Helpers
//{
//	public class AttributeHelper
//	{
//		public IAttribute Attribute;
//		public IReturnType AttributeType;
//		public List<object> PositionalArguments = new List<object>();
//		public Dictionary<string, object> NamedArguments = new Dictionary<string, object>();
//		
//		public void CreateAttribute(string fullName)
//		{
//			CreateAttribute(fullName, fullName);
//		}
//		
//		public void CreateAttribute(string fullName, string shortName)
//		{
//			var returnTypeHelper = new ReturnTypeHelper();
//			returnTypeHelper.CreateReturnType(fullName);
//			returnTypeHelper.AddShortName(shortName);
//			AttributeType = returnTypeHelper.ReturnType;
//			
//			Attribute = MockRepository.GenerateStub<IAttribute>();
//			Attribute.Stub(a => a.AttributeType).Return(AttributeType);
//			Attribute.Stub(a => a.PositionalArguments).Return(PositionalArguments);
//			Attribute.Stub(a => a.NamedArguments).Return(NamedArguments);
//		}
//		
//		public void AddPositionalArguments(params object[] args)
//		{
//			PositionalArguments.AddRange(args);
//		}
//		
//		public void AddNamedArgument(string name, object value)
//		{
//			NamedArguments.Add(name, value);
//		}
//		
//		public void AddAttributeToClass(IClass c)
//		{
//			var attributes = new List<IAttribute>();
//			attributes.Add(Attribute);
//			c.Stub(item => item.Attributes).Return(attributes);
//		}
//		
//		public void AddAttributeToMethod(IMethod method)
//		{
//			var attributes = new List<IAttribute>();
//			attributes.Add(Attribute);
//			method.Stub(m => m.Attributes).Return(attributes);
//		}
//		
//		public void AddAttributeToParameter(IParameter parameter)
//		{
//			var attributes = new List<IAttribute>();
//			attributes.Add(Attribute);
//			parameter.Stub(p => p.Attributes).Return(attributes);
//		}
//	}
//}
