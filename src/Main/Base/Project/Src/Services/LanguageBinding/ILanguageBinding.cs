// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Provides access to language specific features for single files.
	/// </summary>
	public interface ILanguageBinding
	{
		/// <summary>
		/// Provides access to the formatting strategy for this language.
		/// </summary>
		IFormattingStrategy FormattingStrategy {
			get;
		}
		
		/// <summary>
		/// Provides access to the properties for this language.
		/// </summary>
		LanguageProperties Properties {
			get;
		}
		
		/// <summary>
		/// Provides access to the bracket search logic for this language.
		/// </summary>
		IBracketSearcher BracketSearcher {
			get;
		}
		
		/// <summary>
		/// Callback function for backend bindings to add services to ITextEditor.
		/// This is called when the file name of an ITextEditor changes.
		/// </summary>
		void Attach(ITextEditor editor);
		
		/// <summary>
		/// Callback function for backend bindings to remove all added services from ITextEditor.
		/// This is called when the file name of an ITextEditor changes, to unload all added
		/// features properly.
		/// </summary>
		void Detach();
	}
}
