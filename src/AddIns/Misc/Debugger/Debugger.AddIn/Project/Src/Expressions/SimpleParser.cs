// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Debugger;

namespace Debugger.Expressions
{
	public static class SimpleParser
	{
		public static Expression Parse(string code)
		{
			int dotIndex = code.LastIndexOf('.');
			if (dotIndex == -1) {
				return new SimpleIdentifierExpression(code);
			} else {
				string member = code.Substring(dotIndex + 1);
				string target = code.Substring(0, dotIndex);
				return new SimpleMemberReferenceExpression(Parse(target), member);
			}
		}
	}
}
