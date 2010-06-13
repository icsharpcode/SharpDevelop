// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
