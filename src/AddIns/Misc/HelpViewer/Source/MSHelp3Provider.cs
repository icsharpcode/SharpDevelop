// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using MSHelpSystem.Core;

namespace MSHelpSystem
{
	public class MSHelp3Provider : HelpProvider
	{
		public override bool TryShowHelp(string fullTypeName)
		{
			if (string.IsNullOrEmpty(fullTypeName)) {
				throw new ArgumentNullException("fullTypeName");
			}
			LoggingService.Info(string.Format("Help 3.0: Calling \"TryShowHelp\" with {0}", fullTypeName));
			return DisplayHelp.ContextualHelp(fullTypeName);
		}

		public override bool TryShowHelpByKeyword(string keyword)
		{
			if (string.IsNullOrEmpty(keyword)) {
				throw new ArgumentNullException("keyword");
			}
			LoggingService.Info(string.Format("Help 3.0: Calling \"TryShowHelpByKeyword\" with {0}", keyword));
			DisplayHelp.Keywords(keyword);
			return true;
		}
	}
}
