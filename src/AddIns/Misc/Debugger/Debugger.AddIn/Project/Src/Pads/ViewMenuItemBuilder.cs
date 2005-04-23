// <file>
//     <owner name="Daneiel Grunwald" email="daniel@danielgrunwald.de"/>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class DebugViewMenuBuilder : ViewMenuBuilder
	{
		protected override string Category {
			get {
				return "Debugger";
			}
		}
	}
}
