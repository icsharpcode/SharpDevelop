// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.TextEditor.Actions;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class GoToDefinition : AbstractEditActionMenuCommand
	{
		public override IEditAction EditAction {
			get {
				// TODO: use click position instead of cursor position
				return new ICSharpCode.SharpDevelop.DefaultEditor.Actions.GoToDefinition();
			}
		}
	}
}
