// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Debugger
{
	class NewStringEval: Eval
	{
		string textToCreate;
		
		public NewStringEval(Process process,
		                     IExpirable[] expireDependencies,
		                     IMutable[] mutateDependencies,
		                     string textToCreate)
			:base(process, expireDependencies, mutateDependencies)
		{
			this.textToCreate = textToCreate;
		}
		
		protected override void StartEvaluation()
		{
			corEval.NewString(textToCreate);
		}
	}
}
