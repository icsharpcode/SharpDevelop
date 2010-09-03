// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.Build.Framework;

namespace ICSharpCode.SharpDevelop.BuildWorker
{
	[Flags]
	enum EventTypes
	{
		None            = 0,
		Message         = 0x0001,
		Error           = 0x0002,
		Warning         = 0x0004,
		BuildStarted    = 0x0008,
		BuildFinished   = 0x0010,
		ProjectStarted  = 0x0020,
		ProjectFinished = 0x0040,
		TargetStarted   = 0x0080,
		TargetFinished  = 0x0100,
		TaskStarted     = 0x0200,
		TaskFinished    = 0x0400,
		Custom          = 0x0800,
		Unknown         = 0x1000,
		All             = 0x1fff
	}
}
