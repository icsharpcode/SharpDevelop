// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq;
using System.Threading;

using CSharpBinding.Refactoring;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.CSharp.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;

namespace CSharpBinding.Completion
{
	/// <summary>
	/// Completion item that introduces a using declaration.
	/// </summary>
	class ImportCompletionData : EntityCompletionData
	{
		string insertUsing;
		string insertionText;
		
		public ImportCompletionData(ITypeDefinition typeDef, CSharpResolver contextAtCaret, bool useFullName)
			: base(typeDef)
		{
			this.Description = "using " + typeDef.Namespace + ";";
			if (useFullName) {
				var astBuilder = new TypeSystemAstBuilder(contextAtCaret);
				insertionText = astBuilder.ConvertType(typeDef).ToString();
			} else {
				insertionText = typeDef.Name;
				insertUsing = typeDef.Namespace;
			}
		}
		
		public override void Complete(CompletionContext context)
		{
			context.Editor.Document.Replace(context.StartOffset, context.Length, insertionText);
			context.EndOffset = context.StartOffset + insertionText.Length;
			if (insertUsing != null) {
				SD.Log.Debug("Insert using '" + insertUsing + "'");
				var refactoringContext = SDRefactoringContext.Create(context.Editor, CancellationToken.None);
				using (var script = refactoringContext.StartScript()) {
					UsingHelper.InsertUsing(refactoringContext, script, new UsingDeclaration(insertUsing));
				}
			}
		}
	}
}
