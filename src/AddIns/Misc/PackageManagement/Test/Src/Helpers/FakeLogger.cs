// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;

namespace PackageManagement.Tests.Helpers
{
	public class FakeLogger : ILogger
	{
		public List<string> FormattedMessagesLogged = new List<string>();
		
		public string FirstFormattedMessageLogged {
			get { return FormattedMessagesLogged[0]; }
		}
		
		public string LastFormattedMessageLogged {
			get {
				int index = FormattedMessagesLogged.Count - 1;
				return FormattedMessagesLogged[index];
			}
		}
		
		public string NextToLastFormattedMessageLogged {
			get {
				int index = FormattedMessagesLogged.Count - 2;
				return FormattedMessagesLogged[index];
			}
		}
		
		public string SecondFormattedMessageLogged {
			get { return FormattedMessagesLogged[1]; }
		}
		
		public void Log(MessageLevel level, string message, params object[] args)
		{
			string formattedMessage = String.Format(message, args);
			FormattedMessagesLogged.Add(formattedMessage);
		}
		
		public bool FormattedMessagesLoggedContainsText(string text)
		{
			return FormattedMessagesLogged.Any(message => message.Contains(text));
		}
	}
}
