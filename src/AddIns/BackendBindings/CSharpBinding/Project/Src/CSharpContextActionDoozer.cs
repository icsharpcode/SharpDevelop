// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CSharpBinding.Parser;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.SharpDevelop.Refactoring;

namespace CSharpBinding
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
				return Task.Run(
					async delegate {
						var parseInfo = (await context.GetParseInformationAsync().ConfigureAwait(false)) as CSharpFullParseInformation;
						if (parseInfo == null)
							return false;
						lock (this) {
							if (!contextActionCreated) {
								contextActionCreated = true;
								contextAction = (NR5ContextAction)addIn.CreateObject(className);
							}
						}
						if (contextAction == null)
							return false;
						CSharpAstResolver resolver = await context.GetAstResolverAsync().ConfigureAwait(false);
						return true;
					}, cancellationToken);
			}
			
			public override void Execute(EditorContext context)
			{
				throw new NotImplementedException();
			}
		}
	}
}
