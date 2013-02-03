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
			InitializeMessageLoop();
		}
		
		void InitializeMessageLoop()
		{
			IMessageLoop messageLoop = MockRepository.GenerateStub<IMessageLoop>();
			SD.Services.RemoveService(typeof(IMessageLoop));
			SD.Services.AddService(typeof(IMessageLoop), messageLoop);
		}
		
		[TearDown]
		public void TearDown()
		{
			SD.TearDownForUnitTests();
		}
	}
}
