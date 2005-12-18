// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Ivo Kovacka" email="ivok@internet.sk"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;

namespace ICSharpCode.TextEditor.Document
{
	/// <summary>
	/// This class handles the custom lines for a buffer
	/// </summary>
	public interface ICustomLineManager
	{
		/// <value>
		/// Contains all custom lines 
		/// </value>
		ArrayList CustomLines {
			get;
		}
		
		/// <remarks>
		/// Returns the Color if the line <code>lineNr</code> has custom bg color
		/// otherwise returns <code>defaultColor</code>
		/// </remarks>
		Color GetCustomColor(int lineNr, Color defaultColor);
		
		/// <remarks>
		/// Returns true if the line <code>lineNr</code> is read only
		/// </remarks>
		bool IsReadOnly(int lineNr, bool defaultReadOnly);

		/// <remarks>
		/// Returns true if <code>selection</code> is read only
		/// </remarks>
		bool IsReadOnly(ISelection selection, bool defaultReadOnly);

		/// <remarks>
		/// Add Custom Line at the line <code>lineNr</code>
		/// </remarks>
		void AddCustomLine(int lineNr, Color customColor, bool readOnly);
		
		/// <remarks>
		/// Add Custom Lines from the line <code>startLineNr</code> to the line <code>endLineNr</code>
		/// </remarks>
		void AddCustomLine(int startLineNr,  int endLineNr, Color customColor, bool readOnly);
		
		/// <remarks>
		/// Remove Custom Line at the line <code>lineNr</code>
		/// </remarks>
		void RemoveCustomLine(int lineNr);
		
		/// <remarks>
		/// Clears all custom color lines
		/// </remarks>
		void Clear();
		
		/// <remarks>
		/// Is fired before the change
		/// </remarks>
		event EventHandler BeforeChanged;
		
		/// <remarks>
		/// Is fired after the change
		/// </remarks>
		event EventHandler Changed;
	}
}
