// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>


namespace Debugger
{
	public enum ExceptionType
	{
		FirstChance = 1,
		UserFirstChance = 2,
		CatchHandlerFound = 3,
		Unhandled = 4,
	}
}
