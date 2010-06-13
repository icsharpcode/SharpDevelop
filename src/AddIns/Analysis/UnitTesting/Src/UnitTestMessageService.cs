// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestMessageService : IUnitTestMessageService
	{
		public bool AskQuestion(string question, string caption)
		{
			return MessageService.AskQuestion(question, caption);
		}
	}
}
