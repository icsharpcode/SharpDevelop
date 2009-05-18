// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents a form or user control in the designer. Used to generate
	/// Python code after the form has been changed in the designer.
	/// </summary>
	public class PythonControl
	{
		PythonCodeBuilder codeBuilder;
		string indentString = String.Empty;
		
		public PythonControl() 
			: this("\t")
		{
		}
		
		public PythonControl(string indentString)
		{
			this.indentString = indentString;
		}

		/// <summary>
		/// Generates python code for the InitializeComponent method based on the controls added to the form.
		/// </summary>
		public string GenerateInitializeComponentMethod(Control control)
		{
			codeBuilder = new PythonCodeBuilder();
			codeBuilder.IndentString = indentString;
			
			codeBuilder.AppendIndentedLine("def InitializeComponent(self):");
			codeBuilder.IncreaseIndent();
			
			GenerateInitializeComponentMethodBodyInternal(control);
			
			return codeBuilder.ToString();
		}
		
		/// <summary>
		/// Generates the InitializeComponent method body.
		/// </summary>
		public string GenerateInitializeComponentMethodBody(Control control, int initialIndent)
		{
			codeBuilder = new PythonCodeBuilder();
			codeBuilder.IndentString = indentString;
			
			for (int i = 0; i < initialIndent; ++i) {
				codeBuilder.IncreaseIndent();
			}
			GenerateInitializeComponentMethodBodyInternal(control);
			
			return codeBuilder.ToString();
		}				
						
		void GenerateInitializeComponentMethodBodyInternal(Control control)
		{
			PythonDesignerRootComponent rootDesignerComponent = PythonDesignerComponentFactory.CreateDesignerRootComponent(control);
			if (rootDesignerComponent.HasNonVisualChildComponents()) {
				rootDesignerComponent.AppendCreateComponentsContainer(codeBuilder);
				rootDesignerComponent.AppendCreateNonVisualComponents(codeBuilder);
				rootDesignerComponent.AppendNonVisualComponentsBeginInit(codeBuilder);
			}
			rootDesignerComponent.AppendCreateChildComponents(codeBuilder);
			rootDesignerComponent.AppendChildComponentsSuspendLayout(codeBuilder);
			rootDesignerComponent.AppendSuspendLayout(codeBuilder);
			rootDesignerComponent.AppendNonVisualComponents(codeBuilder);
			rootDesignerComponent.AppendComponent(codeBuilder);
			rootDesignerComponent.AppendChildComponentsResumeLayout(codeBuilder);
			if (rootDesignerComponent.HasNonVisualChildComponents()) {
				rootDesignerComponent.AppendNonVisualComponentsEndInit(codeBuilder);
			}
			rootDesignerComponent.AppendResumeLayout(codeBuilder);
		}
	}
}
