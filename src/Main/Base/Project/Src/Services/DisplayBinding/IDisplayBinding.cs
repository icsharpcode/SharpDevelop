// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.SharpDevelop.Project;

using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This class defines the SharpDevelop display binding interface, it is a factory
	/// structure, which creates IViewContents.
	/// </summary>
	public interface IDisplayBinding
	{
		/// <remarks>
		/// This function determines, if this display binding is able to create
		/// an IViewContent for the file given by fileName.
		/// </remarks>
		/// <returns>
		/// true, if this display binding is able to create
		/// an IViewContent for the file given by fileName.
		/// false otherwise
		/// </returns>
		bool CanCreateContentForFile(string fileName);
		
		/// <remarks>
		/// Creates a new IViewContent object for the file fileName
		/// </remarks>
		/// <returns>
		/// A newly created IViewContent object.
		/// </returns>
		IViewContent CreateContentForFile(string fileName);
		
		/// <remarks>
		/// This function determines, if this display binding is able to create
		/// an IViewContent for the language given by languageName.
		/// </remarks>
		/// <returns>
		/// true, if this display binding is able to create
		/// an IViewContent for the language given by languageName.
		/// false otherwise
		/// </returns>
		bool CanCreateContentForLanguage(string languageName);
		
		/// <remarks>
		/// Creates a new IViewContent object for the language given by 
		/// languageName with the content given by content
		/// </remarks>
		/// <returns>
		/// A newly created IViewContent object.
		/// </returns>
		IViewContent CreateContentForLanguage(string languageName, string content);
	}
}
