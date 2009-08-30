// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
	class AggregatedLanguageBinding : ILanguageBinding
	{
		IEnumerable<ILanguageBinding> allBindings;
		
		public AggregatedLanguageBinding(IEnumerable<ILanguageBinding> bindings)
		{
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
