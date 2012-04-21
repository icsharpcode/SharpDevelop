/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 21.04.2012
 * Time: 20:14
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding.Project
{
	/// <summary>
	/// Interaction logic for LinkerOptionsXaml.xaml
	/// </summary>
	public partial class LinkerOptionsXaml :  ProjectOptionPanel
	{
		public LinkerOptionsXaml()
		{
			InitializeComponent();
		}
	}
}