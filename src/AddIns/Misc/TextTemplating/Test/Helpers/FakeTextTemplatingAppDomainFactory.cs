// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace TextTemplating.Tests.Helpers
{
	public class FakeTextTemplatingAppDomainFactory : ITextTemplatingAppDomainFactory
	{
		public FakeTextTemplatingAppDomain FakeTextTemplatingAppDomain = new FakeTextTemplatingAppDomain();
		public int CreateTextTemplatingAppDomainCallCount;
		public string ApplicationBasePassedToCreateTextTemplatingAppDomain;
		
		public ITextTemplatingAppDomain CreateTextTemplatingAppDomain(string applicationBase)
		{
			ApplicationBasePassedToCreateTextTemplatingAppDomain = applicationBase;
			CreateTextTemplatingAppDomainCallCount++;
			return FakeTextTemplatingAppDomain;
		}
	}
}
