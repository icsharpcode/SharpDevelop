// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Exception thrown by the PythonComponentWalker class.
	/// </summary>
	public class PythonComponentWalkerException : Exception
	{
		public PythonComponentWalkerException(string message) : base(message)
		{
		}
	}
}
