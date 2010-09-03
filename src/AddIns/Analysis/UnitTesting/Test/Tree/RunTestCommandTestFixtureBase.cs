// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests.Tree
{
	public class RunTestCommandTestFixtureBase
	{
		protected DerivedRunTestCommand runTestCommand;
		protected PadDescriptor compilerMessageViewPadDescriptor;
		protected PadDescriptor errorListPadDescriptor;
		protected MockRunTestCommandContext context;
		
		public void InitBase()
		{
			if (!PropertyService.Initialized) {
				PropertyService.InitializeService(String.Empty, String.Empty, String.Empty);
			}
			context = new MockRunTestCommandContext();
			runTestCommand = new DerivedRunTestCommand(context);
		}
		
		[TearDown]
		public void TearDownBase()
		{
			AbstractRunTestCommand.RunningTestCommand = null;
		}
	}
}
