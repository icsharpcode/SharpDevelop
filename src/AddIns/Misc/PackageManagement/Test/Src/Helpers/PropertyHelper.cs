// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class PropertyHelper
	{
		public IProperty Property;
		public List<IAttribute> Attributes = new List<IAttribute>();
		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
		
		public void CreateProperty(string name)
		{
			Property = MockRepository.GenerateMock<IProperty, IEntity>();
			Property.Stub(p => p.Name).Return(name);
			Property.Stub(p => p.FullyQualifiedName).Return(name);
			Property.Stub(p => p.Attributes).Return(Attributes);
			Property.Stub(p => p.ProjectContent).Return(ProjectContentHelper.FakeProjectContent);
		}
		
		public void AddAttribute(string fullName, string shortName)
		{
			var attributeHelper = new AttributeHelper();
			attributeHelper.CreateAttribute(fullName, shortName);
			Attributes.Add(attributeHelper.Attribute);
		}
		
		public void AddParentClass(string className)
		{
			IClass c = ProjectContentHelper.AddClassToProjectContent(className);
			Property.Stub(p => p.DeclaringType).Return(c);
		}
		
		public void CreatePublicProperty(string name)
		{
			CreateProperty(name);
			Property.Stub(p => p.IsPublic).Return(true);
		}
		
		public void CreatePrivateProperty(string name)
		{
			CreateProperty(name);
			Property.Stub(p => p.IsPublic).Return(false);
			Property.Stub(p => p.IsPrivate).Return(true);
		}
	}
}
