// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using NUnit.Framework;
using Rhino.Mocks;

namespace AspNet.Mvc.Tests
{
	public abstract class MvcTestsBase
	{
		[SetUp]
		public void Init()
		{
			SD.InitializeForUnitTests();
		}
		
		[TearDown]
		public void TearDown()
		{
			SD.TearDownForUnitTests();
		}
	}
}
