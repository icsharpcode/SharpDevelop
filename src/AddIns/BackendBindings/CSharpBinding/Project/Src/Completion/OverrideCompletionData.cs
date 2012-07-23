// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using CSharpBinding.FormattingStrategy;
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
	/// Item for 'override' completion.
	/// </summary>
	class OverrideCompletionData : EntityCompletionData
	{
		readonly int declarationBegin;
		readonly CSharpTypeResolveContext contextAtCaret;
		
		public OverrideCompletionData(int declarationBegin, IMember m, CSharpTypeResolveContext contextAtCaret)
			: base(m)
		{
			this.declarationBegin = declarationBegin;
			this.contextAtCaret = contextAtCaret;
			var ambience = new CSharpAmbience();
			ambience.ConversionFlags = ConversionFlags.ShowTypeParameterList | ConversionFlags.ShowParameterList | ConversionFlags.ShowParameterNames;
			this.CompletionText = ambience.ConvertEntity(m);
		}
		
		public override void Complete(CompletionContext context)
		{
			if (declarationBegin > context.StartOffset) {
				base.Complete(context);
				return;
			}
			TypeSystemAstBuilder b = new TypeSystemAstBuilder(new CSharpResolver(contextAtCaret));
			b.ShowTypeParameterConstraints = false;
			b.GenerateBody = true;
			
			var entityDeclaration = b.ConvertEntity(this.Entity);
			entityDeclaration.Modifiers &= ~(Modifiers.Virtual | Modifiers.Abstract);
			entityDeclaration.Modifiers |= Modifiers.Override;
			
			StringWriter w = new StringWriter();
			var segmentDict = SegmentTrackingOutputFormatter.WriteNode(w, entityDeclaration, FormattingOptionsFactory.CreateSharpDevelop());
			
			context.Editor.Document.Replace(declarationBegin, context.EndOffset - declarationBegin, w.ToString().TrimEnd());
			var throwStatement = entityDeclaration.Descendants.FirstOrDefault(n => n is ThrowStatement);
			if (throwStatement != null) {
				var segment = segmentDict[throwStatement];
				context.Editor.Select(declarationBegin + segment.Offset, segment.Length);
			}
		}
	}
}
