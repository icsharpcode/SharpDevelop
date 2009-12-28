// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.RubyBinding
{
	/// <summary>
	/// Stores the command line history for the RubyConsole.
	/// </summary>
	public class CommandLineHistory
	{
		List<string> lines = new List<string>();
		int position;
		
		public CommandLineHistory()
		{
		}
		
		/// <summary>
		/// Adds the command line to the history.
		/// </summary>
		public void Add(string line)
		{
			if (!String.IsNullOrEmpty(line)) {
				int index = lines.Count - 1;
				if (index >= 0) {
					if (lines[index] != line) {
						lines.Add(line);
					}
				} else {
					lines.Add(line);
				}
			}
			position = lines.Count;
		}
		
		/// <summary>
		/// Gets the current command line. By default this will be the last command line entered.
		/// </summary>
		public string Current {
			get { 
				if ((position >= 0) && (position < lines.Count)) {
					return lines[position];
				}
				return null;
			}
		}
		
		/// <summary>
		/// Moves to the next command line.
		/// </summary>
		/// <returns>False if the current position is at the end of the command line history.</returns>
		public bool MoveNext()
		{
			int nextPosition = position + 1;
			if (nextPosition < lines.Count) {
				++position;
			}
			return nextPosition < lines.Count;
		}
		
		/// <summary>
		/// Moves to the previous command line.
		/// </summary>
		/// <returns>False if the current position is at the start of the command line history.</returns>
		public bool MovePrevious()
		{
			if (position >= 0) {
				if (position == 0) {
					return false;
				}
				--position;
			}
			return position >= 0;
		}
	}
}
