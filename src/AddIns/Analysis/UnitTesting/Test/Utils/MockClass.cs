// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using System;
using System.Collections.Generic;

namespace UnitTesting.Tests.Utils
{
	public class MockClass : DefaultClass
	{
		string dotNetName;
		IClass compoundClass;
		
		public MockClass()
			: this(String.Empty)
		{
		}
		
		public MockClass(IProjectContent projectContent)
			: this(projectContent, String.Empty)
		{
		}
		
		public MockClass(IProjectContent projectContent, string fullyQualifiedName)
			: this(projectContent, fullyQualifiedName, null)
		{
		}
		
		public MockClass(string fullyQualifiedName)
			: this(fullyQualifiedName, fullyQualifiedName)
		{
		}
		
		public MockClass(string fullyQualifiedName, string dotNetName)
			: this(fullyQualifiedName, dotNetName, null)
		{
		}
		
		public MockClass(string fullyQualifiedName, string dotNetName, IClass declaringType)
			: this(new MockProjectContent(), fullyQualifiedName, dotNetName, declaringType)
		{
		}
		
		public MockClass(IProjectContent projectContent, string fullyQualifiedName, IClass declaringType)
			: this(projectContent, fullyQualifiedName, fullyQualifiedName, declaringType)
		{
		}
		
		public MockClass(IProjectContent projectContent, string fullyQualifiedName, string dotNetName, IClass declaringType)
			: base(new DefaultCompilationUnit(projectContent), declaringType)
		{
			this.FullyQualifiedName = fullyQualifiedName;
			this.dotNetName = dotNetName;
			
			if (declaringType != null) {
				declaringType.InnerClasses.Add(this);
			}
		}
		
		public override string DotNetName {
			get { return dotNetName; }
		}
		
		public void SetDotNetName(string name)
		{
			dotNetName = name;
		}
		
		public MockProjectContent MockProjectContent {
			get { return ProjectContent as MockProjectContent; }
		}
		
		public IProject Project {
			get { return ProjectContent.Project as IProject; }
		}
		
		public static MockClass CreateMockClassWithoutAnyAttributes()
		{
			return CreateMockClassWithAttributes(new MockAttribute[0]);
		}
		
		public static MockClass CreateMockClassWithAttributes(IList<MockAttribute> attributes)
		{
			MockClass mockClass = new MockClass();
			mockClass.MockProjectContent.Project = new MockCSharpProject();
			mockClass.MockProjectContent.Classes.Add(mockClass);
			
			foreach (MockAttribute attribute in attributes) {
				mockClass.Attributes.Add(attribute);
			}
			return mockClass;
		}
		
		public static MockClass CreateMockClassWithAttribute(MockAttribute attribute)
		{
			List<MockAttribute> attributes = new List<MockAttribute>();
			attributes.Add(attribute);
			
			return CreateMockClassWithAttributes(attributes);
		}
		
		public static MockClass CreateClassWithBaseType(string baseTypeName)
		{
			MockClass baseType = MockClass.CreateMockClassWithoutAnyAttributes();
			baseType.FullyQualifiedName = baseTypeName;
			MockClass c = MockClass.CreateMockClassWithoutAnyAttributes();
			c.BaseTypes.Add(new DefaultReturnType(baseType));
			return c;
		}
		
		public void SetCompoundClass(IClass c)
		{
			this.compoundClass = c;
		}
		
		protected override IReturnType CreateDefaultReturnType()
		{
			if (compoundClass != null) {
				return new DefaultReturnType(compoundClass);
			}
			return base.CreateDefaultReturnType();
		}
		
		public void AddBaseClass(IClass baseClass)
		{
			DefaultReturnType returnType = new DefaultReturnType(baseClass);
			BaseTypes.Add(returnType);
		}
	}
}
