// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestTaskService
	{
		IOutputCategory BuildMessageViewCategory { get; }
		void ClearExceptCommentTasks();
		void Add(SDTask task);
		bool SomethingWentWrong { get; }
		bool HasCriticalErrors(bool treatWarningsAsErrors);
	}
}
