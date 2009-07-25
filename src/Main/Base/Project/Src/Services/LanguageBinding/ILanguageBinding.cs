// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
