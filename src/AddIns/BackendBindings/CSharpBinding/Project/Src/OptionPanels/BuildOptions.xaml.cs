// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
