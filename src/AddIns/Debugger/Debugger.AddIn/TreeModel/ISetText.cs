// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.AddIn.TreeModel
{
	public interface ISetText
	{
		bool CanSetText { get; }
		
		bool SetText(string text);
	}
}
