// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Refactoring
{
	using NR5ContextAction = ICSharpCode.NRefactory.CSharp.Refactoring.IContextAction;
	
	/// <summary>
	/// Doozer for C# context actions.
	/// Expects a 'class' referencing an NR5 context action and provides an SD IContextActionsProvider.
	/// </summary>
	public class CSharpContextActionDoozer : IDoozer
	{
		public bool HandleConditions {
			get { return false; }
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			return new CSharpContextActionWrapper(args.AddIn, args.Codon.Properties);
		}
		
		sealed class CSharpContextActionWrapper : ContextAction
		{
			readonly AddIn addIn;
			readonly string className;
			readonly string displayName;
			
			public CSharpContextActionWrapper(AddIn addIn, Properties properties)
			{
				this.addIn = addIn;
				this.className = properties["class"];
				this.displayName = properties["displayName"];
			}
			
			bool contextActionCreated;
			NR5ContextAction contextAction;
			
			public override string ID {
				get { return className; }
			}
			
			public override string DisplayName {
				get { return StringParser.Parse(displayName); }
			}
			
			public override Task<bool> IsAvailableAsync(EditorContext context, CancellationToken cancellationToken)
			{
				if (!string.Equals(Path.GetExtension(context.FileName), ".cs", StringComparison.OrdinalIgnoreCase))
					return Task.FromResult(false);
				ITextEditor editor = context.Editor;
				// grab SelectionStart/SelectionLength while we're still on the main thread
				int selectionStart = editor.SelectionStart;
				int selectionLength = editor.SelectionLength;
				return Task.Run(
					async delegate {
						CreateContextAction();
						if (contextAction == null)
							return false;
						CSharpAstResolver resolver = await context.GetAstResolverAsync().ConfigureAwait(false);
						var refactoringContext = new SDRefactoringContext(context.TextSource, resolver, context.CaretLocation, selectionStart, selectionLength, cancellationToken);
						return contextAction.IsValid(refactoringContext);
					}, cancellationToken);
			}
			
			void CreateContextAction()
			{
				lock (this) {
					if (!contextActionCreated) {
						contextActionCreated = true;
						contextAction = (NR5ContextAction)addIn.CreateObject(className);
					}
				}
			}
			
			public override async Task ExecuteAsync(EditorContext context)
			{
				var resolver = await context.GetAstResolverAsync();
				var refactoringContext = new SDRefactoringContext(context.Editor, resolver, context.CaretLocation);
				CreateContextAction();
				contextAction.Run(refactoringContext);
			}
		}
	}
}
