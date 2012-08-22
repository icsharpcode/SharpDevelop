// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class UsingScopeHelper
	{
		public IUsingScope UsingScope;
		public List<IUsing> Usings = new List<IUsing>();
		
		public UsingScopeHelper()
		{
			UsingScope = MockRepository.GenerateStub<IUsingScope>();
			UsingScope.Stub(scope => scope.Usings).Return(Usings);
		}
		
		public void AddNamespace(string name)
		{
			var usingHelper = new UsingHelper();
			usingHelper.AddNamespace(name);
			Usings.Add(usingHelper.Using);
		}
		
		IUsing CreateUsingWithNamespaces(params string[] names)
		{
			return null;
		}
		
		public void AddNamespaceAlias(string alias, string namespaceName)
		{
			var usingHelper = new UsingHelper();
			usingHelper.AddNamespaceAlias(alias, namespaceName);
			Usings.Add(usingHelper.Using);
		}
		
		public void SetNamespaceName(string namespaceName)
		{
			UsingScope.Stub(u => u.NamespaceName).Return(namespaceName);
		}
	}
}
