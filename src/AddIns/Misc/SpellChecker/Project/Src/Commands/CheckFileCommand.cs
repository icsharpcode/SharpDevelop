// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SpellChecker.Engine;

namespace ICSharpCode.SpellChecker.Commands
{
	/// <summary>
	/// CheckFile Command
	/// </summary>
	public class CheckFileCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			CheckFile.Check();			
		}
		
		
	}
}
