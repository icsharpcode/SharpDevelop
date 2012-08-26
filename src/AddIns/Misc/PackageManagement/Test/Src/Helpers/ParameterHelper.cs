// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class ParameterHelper
	{
		public IParameter Parameter;
		
		public ParameterHelper()
		{
			Parameter = MockRepository.GenerateStub<IParameter>();
		}
		
		public void MakeOptionalParameter()
		{
			Parameter.Stub(p => p.IsOptional).Return(true);
		}
		
		public void MakeOutParameter()
		{
			Parameter.Stub(p => p.IsOut).Return(true);
		}
		
		public void MakeRefParameter()
		{
			Parameter.Stub(p => p.IsRef).Return(true);
		}
		
		public void MakeParamArrayParameter()
		{
			Parameter.Stub(p => p.IsParams).Return(true);
		}
		
		public void MakeInParameter()
		{
			Parameter.Stub(p => p.Modifiers).Return(ParameterModifiers.In);
		}
		
		public void AddAttributeToParameter(string attributeTypeName)
		{
			var attributeHelper = new AttributeHelper();
			attributeHelper.CreateAttribute(attributeTypeName);
			attributeHelper.AddAttributeToParameter(Parameter);
		}
	}
}
