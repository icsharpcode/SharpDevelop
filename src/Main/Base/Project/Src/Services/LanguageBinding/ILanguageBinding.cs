// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using System.Xml;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// The <code>ILanguageBinding</code> interface is the base interface
	/// of all language bindings avaiable.
	/// </summary>
	public interface ILanguageBinding
	{
		/// <returns>
		/// The language for this language binding.
		/// </returns>
		string Language {
			get;
		}
		
		IProject LoadProject(string fileName, string projectName);
		
		/// <summary>
		/// Creates a IProject out of the given ProjetCreateInformation object.
		/// Each language binding must provide a representation of the project
		/// it 'controls'.
		/// </summary>
		IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions);
	}
}
