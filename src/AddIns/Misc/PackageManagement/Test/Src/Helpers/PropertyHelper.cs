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
		
		public void CreateProperty(string name)
		{
			Property = MockRepository.GenerateMock<IProperty, IEntity>();
			Property.Stub(p => p.Attributes).Return(Attributes);
		}
		
		public void AddAttribute(string fullName, string shortName)
		{
			var attributeHelper = new AttributeHelper();
			attributeHelper.CreateAttribute(fullName, shortName);
			Attributes.Add(attributeHelper.Attribute);
		}
	}
}
