// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

//using System;
//using System.Collections.Generic;
//using ICSharpCode.SharpDevelop.Dom;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.Helpers
//{
//	public class UsingHelper
//	{
//		public IUsing Using;
//		public List<string> Namespaces = new List<string>();
//		
//		public UsingHelper()
//		{
//			Using = MockRepository.GenerateStub<IUsing>();
//			Using.Stub(u => u.Usings).Return(Namespaces);
//		}
//		
//		public void AddNamespace(string name)
//		{
//			Namespaces.Add(name);
//		}
//		
//		public void AddNamespaceAlias(string alias, string namespaceName)
//		{
//			Dictionary<string, IReturnType> aliases = CreateAliasDictionary(alias, namespaceName);
//			Using.Stub(u => u.HasAliases).Return(true);
//			Using.Stub(u => u.Aliases).Return(aliases);
//		}
//		
//		Dictionary<string, IReturnType> CreateAliasDictionary(string alias, string namespaceName)
//		{
//			var helper = new ReturnTypeHelper();
//			helper.CreateReturnType(namespaceName);
//			helper.AddDotNetName(namespaceName);
//			var aliases = new Dictionary<string, IReturnType>();
//			aliases.Add(alias, helper.ReturnType);
//			return aliases;
//		}
//	}
//}
