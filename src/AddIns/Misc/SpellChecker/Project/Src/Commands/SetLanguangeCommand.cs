// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using ICSharpCode.Core;
using ICSharpCode.SpellChecker.Windows;

namespace ICSharpCode.SpellChecker.Commands
{
	/// <summary>
	/// SetLanguange command
	/// </summary>
	public class SetLanguangeCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			SetLanguageWindow window = new SetLanguageWindow();
			window.ShowDialog();
		}
	}
}
