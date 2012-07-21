// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using Rhino.Mocks;

namespace PackageManagement.Tests.Helpers
{
	public class UsingHelper
	{
		public IUsing Using;
		public List<string> Namespaces = new List<string>();
		
		public UsingHelper()
		{
			Using = MockRepository.GenerateStub<IUsing>();
			Using.Stub(u => u.Usings).Return(Namespaces);
		}
		
		public void AddNamespace(string name)
		{
			Namespaces.Add(name);
		}
		
		public void AddNamespaceAlias(string alias, string namespaceName)
		{
			Dictionary<string, IReturnType> aliases = CreateAliasDictionary(alias, namespaceName);
			Using.Stub(u => u.HasAliases).Return(true);
			Using.Stub(u => u.Aliases).Return(aliases);
		}
		
		Dictionary<string, IReturnType> CreateAliasDictionary(string alias, string namespaceName)
		{
			var helper = new ReturnTypeHelper();
			helper.CreateReturnType(namespaceName);
			helper.AddDotNetName(namespaceName);
			var aliases = new Dictionary<string, IReturnType>();
			aliases.Add(alias, helper.ReturnType);
			return aliases;
		}
	}
}
