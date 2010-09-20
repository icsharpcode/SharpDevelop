// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Aggregates multiple ILanguageBinding instances to allow more
	/// than one language binding for a filename extension.
	/// </summary>
	sealed class AggregatedLanguageBinding : ILanguageBinding
	{
		public static readonly AggregatedLanguageBinding NullLanguageBinding = new AggregatedLanguageBinding(System.Linq.Enumerable.Empty<ILanguageBinding>());
		
		readonly IEnumerable<ILanguageBinding> allBindings;
		
		public AggregatedLanguageBinding(IEnumerable<ILanguageBinding> bindings)
		{
			if (bindings == null)
				throw new ArgumentNullException("bindings");
			this.allBindings = bindings;
		}
		
		public IFormattingStrategy FormattingStrategy {
			get {
				foreach (ILanguageBinding binding in allBindings) {
					if (binding.FormattingStrategy != null)
						return binding.FormattingStrategy;
				}
				
				return DefaultFormattingStrategy.DefaultInstance;
			}
		}
		
		public LanguageProperties Properties {
			get {
				foreach (ILanguageBinding binding in allBindings) {
					if (binding.Properties != null)
						return binding.Properties;
				}
				
				return LanguageProperties.None;
			}
		}
		
		public void Attach(ITextEditor editor)
		{
			foreach (ILanguageBinding binding in allBindings)
				binding.Attach(editor);
		}
		
		public void Detach()
		{
			foreach (ILanguageBinding binding in allBindings)
				binding.Detach();
		}
		
		public IBracketSearcher BracketSearcher {
			get {
				foreach (ILanguageBinding binding in allBindings) {
					if (binding.BracketSearcher != null)
						return binding.BracketSearcher;
				}
				
				return DefaultBracketSearcher.DefaultInstance;
			}
		}
	}
}
