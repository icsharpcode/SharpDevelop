//
// CS1105ExtensionMethodMustBeDeclaredStaticAction.cs
//
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc. (http://xamarin.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using ICSharpCode.NRefactory.TypeSystem;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Extension methods must be declared static")]
	public class CS1105ExtensionMethodMustBeDeclaredStaticAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var method = context.GetNode<MethodDeclaration>();
			if (method == null || !method.NameToken.Contains(context.Location))
				yield break;

			if (method.HasModifier(Modifiers.Static))
				yield break;
			var param = method.Parameters.FirstOrDefault();
			if (param == null || param.ParameterModifier != ParameterModifier.This)
				yield break;
			yield return new CodeAction(
				context.TranslateString("Convert method to static"),
				script => script.ChangeModifier(method, method.Modifiers | Modifiers.Static), 
				method) {
				Severity = ICSharpCode.NRefactory.Refactoring.Severity.Error
			};
		}
	}
}

