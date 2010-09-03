// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace UnitTesting.Tests.Utils
{
	public class MockMethod : DefaultMethod
	{
		public MockMethod()
			: this(String.Empty)
		{
		}
		
		public MockMethod(string name)
			: this(null, name)
		{
		}
		
		public MockMethod(IClass declaringType)
			: this(declaringType, String.Empty)
		{
		}
		
		public MockMethod(IClass declaringType, string name)
			: base(declaringType, name)
		{
		}
		
		public static MockMethod CreateMockMethodWithoutAnyAttributes()
		{
			return CreateMockMethodWithAttributes(new MockAttribute[0]);
		}
		
		public static MockMethod CreateMockMethodWithAttributes(IList<MockAttribute> attributes)
		{
			MockClass mockClass = MockClass.CreateMockClassWithoutAnyAttributes();
			MockMethod mockMethod = new MockMethod(mockClass);
			
			foreach (MockAttribute attribute in attributes) {
				mockMethod.Attributes.Add(attribute);
			}
			
			return mockMethod;
		}
		
		public static MockMethod CreateMockMethodWithAttribute(MockAttribute attribute)
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(attribute);
			
			return CreateMockMethodWithAttributes(attributes);
		}
		
		public MockClass MockDeclaringType {
			get { return DeclaringType as MockClass; }
		}
	}
}
