// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.TextTemplating
{
	public class CompilerErrorTask : Task
	{
		public CompilerErrorTask(CompilerError error)
			: base(
				GetFileName(error.FileName),
				error.ErrorText,
				error.Column,
				error.Line,
				GetTaskType(error.IsWarning))
		{
		}
		
		static TaskType GetTaskType(bool warning)
		{
			if (warning) {
				return TaskType.Warning;
			}
			return TaskType.Error;
		}
		
		static FileName GetFileName(string fileName)
		{
			if (!String.IsNullOrEmpty(fileName)) {
				return new FileName(fileName);
			}
			return null;
		}
	}
}
