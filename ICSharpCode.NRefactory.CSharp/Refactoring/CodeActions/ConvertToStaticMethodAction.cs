
// ConvertToStaticMethodAction.cs
//
// Author:
//      Ciprian Khlud <ciprian.mustiata@yahoo.com>
//
// Copyright (c) 2013 Ciprian Khlud <ciprian.mustiata@yahoo.com>
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
using System.Collections.Generic;
using ICSharpCode.NRefactory.CSharp.Refactoring.ExtractMethod;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Make this method static", Description = "This method doesn't use any non static members so it can be made static")]
	public class ConvertToStaticMethodAction : ICodeActionProvider
	{
	    public IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			// TODO: Invert if without else
			// ex. if (cond) DoSomething () == if (!cond) return; DoSomething ()
			// beware of loop contexts return should be continue then.
            var methodDeclaration = GetMethodDeclaration(context);
            if (methodDeclaration == null)
                yield break;

            var resolved = context.Resolve(methodDeclaration) as MemberResolveResult;
            if (resolved == null)
                yield break;
	        var isImplementingInterface = resolved.Member.ImplementedInterfaceMembers.Any();

			if (isImplementingInterface)
				yield break;
            yield return new CodeAction(context.TranslateString(string.Format("Make '{0}' static", methodDeclaration.Name)), script =>
            {
                var clonedDeclaration = (MethodDeclaration)methodDeclaration.Clone();
                clonedDeclaration.Modifiers |= Modifiers.Static;
                script.Replace(methodDeclaration, clonedDeclaration);
				var rr = context.Resolve (methodDeclaration) as MemberResolveResult;
                var method = (IMethod)rr.Member;
                //method.ImplementedInterfaceMembers.Any(m => methodGroupResolveResult.Methods.Contains((IMethod)m));
            
				script.DoGlobalOperationOn(rr.Member, (fctx, fscript, fnode) => {
					if (fnode is MemberReferenceExpression) {
						var memberReference = new MemberReferenceExpression (
							new TypeReferenceExpression (fctx.CreateShortType (rr.Member.DeclaringType)),
							rr.Member.Name
						);
						fscript.Replace (fnode, memberReference);
					} else if (fnode is InvocationExpression) {
						var invoke = (InvocationExpression)fnode;
						if (!(invoke.Target is MemberReferenceExpression))
							return;
						var memberReference = new MemberReferenceExpression (
							new TypeReferenceExpression (fctx.CreateShortType (rr.Member.DeclaringType)),
							rr.Member.Name
						);
						fscript.Replace (invoke.Target, memberReference);
					}
				});
			}, methodDeclaration);
		}

	    static MethodDeclaration GetMethodDeclaration(RefactoringContext context)
		{
            var result = context.GetNode <MethodDeclaration>();
			if (result == null)
                return null;
            //unsafe transformation for now. For all other public instances the code should 
            //replace the variable.Call(...) to ClassName.Call()
            const Modifiers ignoredModifiers = Modifiers.Override | Modifiers.Virtual | Modifiers.Static;
            if ((result.Modifiers & ignoredModifiers) != 0)
                return null;

            var usesNonStatic = StaticVisitor.UsesNotStaticMember(context, result);
            if (usesNonStatic) return null;
            return result;
        }
	}
}
