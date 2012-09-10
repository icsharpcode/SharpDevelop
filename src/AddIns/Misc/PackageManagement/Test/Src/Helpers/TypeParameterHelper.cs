// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class TypeParameterHelper
	{
		public ITypeParameter TypeParameter;
		
		public TypeParameterHelper()
		{
			TypeParameter = MockRepository.GenerateMock<ITypeParameter>();
		}
		
		public void SetName(string name)
		{
			TypeParameter.Stub(tp => tp.Name);
		}
		
		public List<ITypeParameter> TypeParameterToList()
		{
			var parameters = new List<ITypeParameter>();
			parameters.Add(TypeParameter);
			return parameters;
		}
	}
}
