// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Description of AddInLoadException.
	/// </summary>
	public class AddInLoadException : CoreException
	{
		public AddInLoadException(string message) : base(message)
		{
		}
	}
}
