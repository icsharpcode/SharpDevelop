// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Debugger
{
	public class GetValueException: DebuggerException
	{
		public GetValueException(string error, System.Exception inner):base(error, inner)
		{
		}
		
		public GetValueException(string errorFmt, params object[] args):base(string.Format(errorFmt, args))
		{
		}
		
		public GetValueException(string error):base(error)
		{
		}
	}
}
