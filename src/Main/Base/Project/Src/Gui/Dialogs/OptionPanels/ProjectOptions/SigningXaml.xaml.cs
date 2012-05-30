/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 26.04.2012
 * Time: 19:56
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for SigningXaml.xaml
	/// </summary>
	public partial class SigningXaml : ProjectOptionPanel
	{
//		private const string KeyFileExtensions = "*.snk;*.pfx;*.key";
	//	private MSBuildBasedProject project;
	
		public SigningXaml()
		{
			InitializeComponent();

		}
		
		public ProjectProperty<String> SignAssembly {
			get { return GetProperty("SignAssembly","false", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<string> AssemblyOriginatorKeyFile {
			get { return GetProperty("AssemblyOriginatorKeyFile","", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
	}
}