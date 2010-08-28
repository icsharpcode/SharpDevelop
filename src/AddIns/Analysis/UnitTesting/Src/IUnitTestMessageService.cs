// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestMessageService
	{
		bool AskQuestion(string question, string caption);
		void ShowFormattedErrorMessage(string format, string item);
	}
}
