// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.PackageManagement
{
	public interface IMessageViewCategory
	{
		void AppendLine(string text);
		void Clear();
		IOutputCategory OutputCategory { get; }
	}
}
