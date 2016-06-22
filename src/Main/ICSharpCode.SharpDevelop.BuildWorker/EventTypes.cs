// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
