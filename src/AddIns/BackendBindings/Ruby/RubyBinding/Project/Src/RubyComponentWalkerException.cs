// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
