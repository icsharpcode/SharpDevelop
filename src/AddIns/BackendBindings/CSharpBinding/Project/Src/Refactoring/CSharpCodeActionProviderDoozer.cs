// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding.Refactoring
{
	/// <summary>
	/// Doozer for C# context actions.
	/// Expects a 'class' referencing an NR5 context action and provides an SD IContextActionsProvider.
	/// </summary>
	public class CSharpCodeActionProviderDoozer : IDoozer
	{
		public bool HandleConditions {
			get { return false; }
		}
		
		public object BuildItem(BuildItemArgs args)
		{
			Type providerType = args.AddIn.FindType(args.Codon.Properties["class"]);
			if (providerType == null)
				return null;
			var attributes = providerType.GetCustomAttributes(typeof(ContextActionAttribute), true);
			if (attributes.Length == 0) {
				LoggingService.Error("[ContextAction] attribute is missing on " + providerType.FullName);
				return null;
			}
			if (!typeof(CodeActionProvider).IsAssignableFrom(providerType)) {
				LoggingService.Error(providerType.FullName + " does not implement CodeActionProvider");
				return null;
			}
			return new CSharpContextActionProviderWrapper((ContextActionAttribute)attributes[0], providerType);
		}
		
		sealed class CSharpContextActionProviderWrapper : IContextActionProvider
		{
			readonly ContextActionAttribute attribute;
			readonly Type type;
			
			public CSharpContextActionProviderWrapper(ContextActionAttribute attribute, Type type)
			{
				this.attribute = attribute;
				this.type = type;
			}
			
			CodeActionProvider codeActionProvider;
			
			bool CreateCodeActionProvider()
			{
				lock (this) {
					if (codeActionProvider == null) {
						codeActionProvider = (CodeActionProvider)Activator.CreateInstance(type);
					}
					return true;
				}
			}
			
			public string ID {
				get { return type.FullName; }
			}
			
			public string DisplayName {
				get { return attribute.Title; }
			}
			
			public string Category {
				get { return attribute.Category ?? string.Empty; }
			}
			
			public bool AllowHiding {
				get { return true; }
			}
			
			public bool IsVisible { get; set; }
			
			public Task<IContextAction[]> GetAvailableActionsAsync(EditorRefactoringContext context, CancellationToken cancellationToken)
			{
				ITextEditor editor = context.Editor;
				// grab SelectionStart/SelectionLength while we're still on the main thread
				int selectionStart = editor.SelectionStart;
				int selectionLength = editor.SelectionLength;
				return Task.Run(
					async delegate {
						if (!CreateCodeActionProvider())
							return new IContextAction[0];
						CSharpAstResolver resolver = await context.GetAstResolverAsync().ConfigureAwait(false);
						var refactoringContext = new SDRefactoringContext(context.TextSource, resolver, context.CaretLocation, selectionStart, selectionLength, cancellationToken);
						return codeActionProvider.GetActions(refactoringContext).Select(Wrap).ToArray();
					}, cancellationToken);
			}
			
			IContextAction Wrap(CodeAction actionToWrap, int index)
			{
				// Take care not to capture 'actionToWrap' in the lambda
				string description = actionToWrap.Description;
				return new CSharpContextActionWrapper(
					this, actionToWrap,
					context => {
						var actions = codeActionProvider.GetActions(context).ToList();
						if (index < actions.Count) {
							var action = actions[index];
							if (action.Description == description)
								return action;
						}
						return null;
					});
			}
		}
	}
}
