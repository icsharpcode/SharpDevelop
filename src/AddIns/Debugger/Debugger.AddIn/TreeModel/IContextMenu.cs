// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System.Windows.Forms;

namespace Debugger.AddIn.TreeModel
{
	public interface IContextMenu
	{
		ContextMenuStrip GetContextMenu();
	}
}
