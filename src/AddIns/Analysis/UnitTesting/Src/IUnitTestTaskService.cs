// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestTaskService
	{
		MessageViewCategory BuildMessageViewCategory { get; }
		bool InUpdate { get; set; }
		void ClearExceptCommentTasks();
		void Add(Task task);
		bool SomethingWentWrong { get; }
		bool HasCriticalErrors(bool treatWarningsAsErrors);
	}
}
