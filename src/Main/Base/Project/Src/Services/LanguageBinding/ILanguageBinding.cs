// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

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
		
		#region routines for single file compilation
		/// <summary>
		/// Is used to determine, if this language binding is able to compile a specific file.
		/// </summary>
		/// <param name="fileName">The file name of the file to compile.</param>
		/// <returns>True, if this language binding can compile the given file.</returns>
		bool CanCompile(string fileName);
		
		/// <summary>
		/// Compiles a single file.
		/// </summary>
		/// <param name="fileName">The file name of the file to compile.</param>
		/// <returns>The compiler results.</returns>
		CompilerResults CompileFile(string fileName);
		
		/// <summary>
		/// This function executes a file, the filename is given by filename,
		/// the file was compiled by the compiler object before.
		/// </summary>
		void Execute(string fileName, bool debug);
		
		/// <summary>
		/// Returns the name of the file output. (need only to work for CanCompile == true files)
		/// </summary>
		/// <param name="fileName">The file name of the file to compile.</param>
		/// <returns>The compiled assembly name (full path).</returns>
		string GetCompiledOutputName(string fileName);
		#endregion
		
		
		IProject LoadProject(string fileName, string projectName);
		
		/// <summary>
		/// Creates a IProject out of the given ProjetCreateInformation object.
		/// Each language binding must provide a representation of the project
		/// it 'controls'.
		/// </summary>
		IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions);
	}
}
