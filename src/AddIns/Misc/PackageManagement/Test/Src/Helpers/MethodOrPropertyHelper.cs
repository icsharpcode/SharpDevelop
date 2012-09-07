// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class MethodOrPropertyHelper
	{
		public List<IParameter> parameters = new List<IParameter>();
		
		public void AddParameter(string name)
		{
			AddParameter("System.String", name);
		}
		
		public void AddParameter(string type, string name)
		{
			IParameter parameter = MockRepository.GenerateStub<IParameter>();
			parameter.Stub(p => p.Name).Return(name);
			
			var returnTypeHelper = new ReturnTypeHelper();
			returnTypeHelper.CreateReturnType("System.String");
			returnTypeHelper.AddDotNetName("System.String");
			parameter.ReturnType = returnTypeHelper.ReturnType;
			
			parameters.Add(parameter);
		}
	}
}
