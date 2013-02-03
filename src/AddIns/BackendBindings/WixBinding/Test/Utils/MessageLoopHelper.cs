// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using Rhino.Mocks;

namespace WixBinding.Tests.Utils
{
	public static class MessageLoopHelper
	{
		public static void InitializeForUnitTests()
		{
			IMessageLoop messageLoop = MockRepository.GenerateStub<IMessageLoop>();
			SD.Services.RemoveService(typeof(IMessageLoop));
			SD.Services.AddService(typeof(IMessageLoop), messageLoop);
		}
	}
}
