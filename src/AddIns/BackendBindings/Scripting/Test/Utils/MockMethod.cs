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
//
//namespace ICSharpCode.Scripting.Tests.Utils
//{
//	public class MockMethod : DefaultMethod
//	{
//		public MockMethod()
//			: this(String.Empty)
//		{
//		}
//		
//		public MockMethod(string name)
//			: this(null, name)
//		{
//		}
//		
//		public MockMethod(IClass declaringType)
//			: this(declaringType, String.Empty)
//		{
//		}
//		
//		public MockMethod(IClass declaringType, string name)
//			: base(declaringType, name)
//		{
//		}
//		
//		public static MockMethod CreateMockMethodWithoutAnyAttributes()
//		{
//			return CreateMockMethodWithAttributes(new MockAttribute[0]);
//		}
//		
//		public static MockMethod CreateMockMethodWithAttributes(IList<MockAttribute> attributes)
//		{
//			MockClass mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
//			MockMethod mockMethod = new MockMethod(mockClass);
//			
//			foreach (MockAttribute attribute in attributes) {
//				mockMethod.Attributes.Add(attribute);
//			}
//			
//			return mockMethod;
//		}
//		
//		public static MockMethod CreateMockMethodWithAttribute(MockAttribute attribute)
//		{
//			List<MockAttribute> attributes = new List<MockAttribute>();
//			attributes.Add(attribute);
//			
//			return CreateMockMethodWithAttributes(attributes);
//		}
//		
//		public MockClass MockDeclaringType {
//			get { return DeclaringType as MockClass; }
//		}
//	}
//}
