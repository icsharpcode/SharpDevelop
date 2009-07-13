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
using System.Globalization;
using System.Reflection;
using System.Resources;
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
		IResourceService resourceService;
		
		public PythonControl() 
			: this("\t")
		{
		}
		
		public PythonControl(string indentString)
			: this(indentString, null)
		{
			this.indentString = indentString;
		}
		
		public PythonControl(string indentString, IResourceService resourceService)
		{
			this.indentString = indentString;
			this.resourceService = resourceService;
		}

		/// <summary>
		/// Generates python code for the InitializeComponent method based on the controls added to the form.
		/// </summary>
		public string GenerateInitializeComponentMethod(Control control)
		{
			return GenerateInitializeComponentMethod(control, String.Empty);
		}
		
		/// <summary>
		/// Generates python code for the InitializeComponent method based on the controls added to the form.
		/// </summary>
		public string GenerateInitializeComponentMethod(Control control, string rootNamespace)
		{
			PythonCodeBuilder methodCodeBuilder = new PythonCodeBuilder();
			methodCodeBuilder.IndentString = indentString;
			
			methodCodeBuilder.AppendIndentedLine("def InitializeComponent(self):");
						
			codeBuilder = new PythonCodeBuilder();
			codeBuilder.IndentString = indentString;
			codeBuilder.IncreaseIndent();
			
			PythonDesignerRootComponent rootComponent = GenerateInitializeComponentMethodBodyInternal(control, rootNamespace);
			rootComponent.GenerateResources(resourceService);
			
			methodCodeBuilder.Append(codeBuilder.ToString());
			return methodCodeBuilder.ToString();
		}

		/// <summary>
		/// Generates the InitializeComponent method body.
		/// </summary>
		public string GenerateInitializeComponentMethodBody(Control control, int initialIndent)
		{
			return GenerateInitializeComponentMethodBody(control, String.Empty, initialIndent);
		}
		
		/// <summary>
		/// Generates the InitializeComponent method body.
		/// </summary>
		public string GenerateInitializeComponentMethodBody(Control control, string rootNamespace, int initialIndent)
		{
			codeBuilder = new PythonCodeBuilder();
			codeBuilder.IndentString = indentString;
			
			for (int i = 0; i < initialIndent; ++i) {
				codeBuilder.IncreaseIndent();
			}
			PythonDesignerRootComponent rootComponent = GenerateInitializeComponentMethodBodyInternal(control, rootNamespace);
			rootComponent.GenerateResources(resourceService);
			
			return codeBuilder.ToString();
		}

		PythonDesignerRootComponent GenerateInitializeComponentMethodBodyInternal(Control control, string rootNamespace)
		{
			PythonDesignerRootComponent rootDesignerComponent = PythonDesignerComponentFactory.CreateDesignerRootComponent(control, rootNamespace);
			rootDesignerComponent.AppendCreateContainerComponents(codeBuilder);
			rootDesignerComponent.AppendSupportInitializeComponentsBeginInit(codeBuilder);
			rootDesignerComponent.AppendChildComponentsSuspendLayout(codeBuilder);
			rootDesignerComponent.AppendSuspendLayout(codeBuilder);
			rootDesignerComponent.AppendComponent(codeBuilder);
			rootDesignerComponent.AppendChildComponentsResumeLayout(codeBuilder);
			rootDesignerComponent.AppendSupportInitializeComponentsEndInit(codeBuilder);
			rootDesignerComponent.AppendResumeLayout(codeBuilder);
			return rootDesignerComponent;
		}
	}
}
