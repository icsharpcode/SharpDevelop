// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakeMessageReporter : IMessageReporter
	{
		public string MessagePassedToShowErrorMessage;
		public bool IsClearMessageCalled;
		
		public void ShowErrorMessage(string message)
		{
			MessagePassedToShowErrorMessage = message;
		}
		
		public void ClearMessage()
		{
			IsClearMessageCalled = true;
		}
	}
}
