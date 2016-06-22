/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.02.2014
 * Time: 19:51
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.Reporting.Addin.Views;

namespace ICSharpCode.Reporting.Addin.DesignerBinding
{
	/// <summary>
	/// Description of IDesignerGenerator.
	/// </summary>
	public interface IDesignerGenerator
	{
		System.CodeDom.Compiler.CodeDomProvider CodeDomProvider {
			get;
		}
		void Attach(DesignerView viewContent);
		void Detach();
		DesignerView ViewContent { get; }
		/// <summary>
		/// Gets the collection of OpenedFiles that contain code which belongs
		/// to the designed form, not including resource files.
		/// </summary>
		/// <param name="designerCodeFile">Receives the file which contains the code to be modified by the forms designer.</param>
		/// <returns>A collection of OpenedFiles that contain code which belongs to the designed form.</returns>
		/// <remarks>The returned collection must include the <paramref name="designerCodeFile"/>.</remarks>
		IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile);
		void MergeFormChanges(CodeCompileUnit unit);
		bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position);
	}
}
