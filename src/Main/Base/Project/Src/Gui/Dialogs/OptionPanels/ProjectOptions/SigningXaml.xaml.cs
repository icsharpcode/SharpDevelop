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
		
		private void Initialize()
		{
			Cmd = new RelayCommand<bool>(CheckExecute);
		}
		
		
		
		public ProjectProperty<String> SignAssembly {
			get { return GetProperty("SignAssembly","", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<string> AssemblyOriginatorKeyFile {
			get { return GetProperty("AssemblyOriginatorKeyFile","", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		#region overrides
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			Console.WriteLine("sign {0}",SignAssembly.Value.ToString());
			//this.project = project;
			Console.WriteLine("sign {0}",SignAssembly.Value.ToString());
			Initialize();				
			string prop  = GetProperty("SignAssembly", "", TextBoxEditMode.EditRawProperty).Value.ToString();
		}
		#endregion
		
		public RelayCommand<bool> Cmd {get;set;}
		
		
		private void CheckExecute(bool isChecked)
		{
			Console.WriteLine(" Checkbox ischecked {0}",isChecked);
			IsDirty = true;
			SignAssembly.Value = "False";
		}
			
	}
}