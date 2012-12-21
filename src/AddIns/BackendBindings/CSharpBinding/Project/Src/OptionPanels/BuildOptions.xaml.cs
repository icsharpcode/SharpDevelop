/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 05/02/2012
 * Time: 19:54
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.Converter;
using ICSharpCode.SharpDevelop.Widgets;

namespace CSharpBinding.OptionPanels
{
	/// <summary>
	/// Interaction logic for BuildOptions.xaml
	/// </summary>
	
	public partial class BuildOptions : ProjectOptionPanel
	{

		public BuildOptions()
		{
			InitializeComponent();
			this.DataContext = this;
		}
		
		
		#region properties
		
		public ProjectProperty<string> DefineConstants {
			get {return GetProperty("DefineConstants", "",
			                        TextBoxEditMode.EditRawProperty,PropertyStorageLocations.ConfigurationSpecific); }
		}
		
		
		public ProjectProperty<bool> Optimize {
			get { return GetProperty("Optimize", false, PropertyStorageLocations.ConfigurationSpecific); }
		}
		
		
		public ProjectProperty<bool> AllowUnsafeBlocks {
			get { return GetProperty("AllowUnsafeBlocks", false); }
		}
		
		
		public ProjectProperty<bool> CheckForOverflowUnderflow {
			get { return GetProperty("CheckForOverflowUnderflow", false, PropertyStorageLocations.ConfigurationSpecific); }
		}
		
		public ProjectProperty<bool> NoStdLib {
			get { return GetProperty("NoStdLib", false); }
		}
		
	
		#endregion
		
		#region overrides
		
		protected override void Initialize()
		{
			base.Initialize();
			buildOutput.Initialize(this);
			this.buildAdvanced.Initialize(this);
			this.errorsAndWarnings.Initialize(this);
			this.treatErrorsAndWarnings.Initialize(this);
		}
		
		#endregion
		
	}
}