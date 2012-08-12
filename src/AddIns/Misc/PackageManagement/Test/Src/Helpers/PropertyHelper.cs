// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class PropertyHelper : MethodOrPropertyHelper
	{
		public IProperty Property;
		public ProjectContentHelper ProjectContentHelper = new ProjectContentHelper();
		public ReturnTypeHelper ReturnTypeHelper = new ReturnTypeHelper();
		
		List<IAttribute> attributes = new List<IAttribute>();
		
		/// <summary>
		/// Property name should include class prefix (e.g. "Class1.MyProperty")
		/// </summary>
		public void CreateProperty(string fullyQualifiedName)
		{
			Property = MockRepository.GenerateMock<IProperty, IEntity>();
			Property.Stub(p => p.FullyQualifiedName).Return(fullyQualifiedName);
			Property.Stub(p => p.Attributes).Return(attributes);
			Property.Stub(p => p.Parameters).Return(parameters);
			Property.Stub(p => p.ProjectContent).Return(ProjectContentHelper.ProjectContent);
		}
		
		public void AddAttribute(string fullName, string shortName)
		{
			var attributeHelper = new AttributeHelper();
			attributeHelper.CreateAttribute(fullName, shortName);
			attributes.Add(attributeHelper.Attribute);
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
		
		public void HasGetterAndSetter()
		{
			HasGetter = true;
			HasSetter = true;
		}
		
		public bool HasSetter {
			set { Property.Stub(p => p.CanSet).Return(value); }
		}
		
		public bool HasGetter {
			set { Property.Stub(p => p.CanGet).Return(value); }
		}
		
		public void HasGetterOnly()
		{
			HasGetter = true;
			HasSetter = false;
		}
		
		public void HasSetterOnly()
		{
			HasGetter = false;
			HasSetter = true;
		}
		
		public void GetterModifierIsPrivate()
		{
			GetterModifier = ModifierEnum.Private;
		}
		
		ModifierEnum GetterModifier {
			set { Property.Stub(p => p.GetterModifiers).Return(value); }
		}
			
		public void GetterModifierIsNone()
		{
			GetterModifier = ModifierEnum.None;
		}
		
		public void SetterModifierIsPrivate()
		{
			SetterModifier = ModifierEnum.Private;
		}
		
		ModifierEnum SetterModifier {
			set { Property.Stub(p => p.SetterModifiers).Return(value); }
		}
			
		public void SetterModifierIsNone()
		{
			GetterModifier = ModifierEnum.None;
		}
		
		public void SetPropertyReturnType(string fullName)
		{
			ReturnTypeHelper.CreateReturnType(fullName);
			ReturnTypeHelper.AddDotNetName(fullName);
			Property.Stub(p => p.ReturnType).Return(ReturnTypeHelper.ReturnType);
		}
		
		public object Project {
			get { return ProjectContentHelper.ProjectContent.Project; }
		}
		
		public void CreateProjectForProjectContent()
		{
			ProjectContentHelper.SetProjectForProjectContent(ProjectHelper.CreateTestProject());
		}
	}
}
