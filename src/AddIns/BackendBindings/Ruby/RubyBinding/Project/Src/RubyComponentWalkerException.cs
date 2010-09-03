// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Exception thrown by the RubyComponentWalker class
	/// </summary>
	public class RubyComponentWalkerException : Exception
	{
		public RubyComponentWalkerException(string message) : base(message)
		{
		}
	}
}
