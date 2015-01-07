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
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// View model for assembly info
	/// </summary>
	public class AssemblyInfoViewModel : ViewModelBase
	{
		private const string NONE_LANGUAGE_CODE = "NONE";

		private readonly AssemblyInfo assemblyInfo;

		public AssemblyInfoViewModel(AssemblyInfo assemblyInfo)
		{
			this.assemblyInfo = assemblyInfo;
			NewGuidCommand = new RelayCommand(() => Guid = System.Guid.NewGuid());

			var cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

			Languages = new Dictionary<string, string>();
			Languages.Add(
				NONE_LANGUAGE_CODE,
				string.Format("({0})", StringParser.Parse("${res:Dialog.ProjectOptions.AssemblyInfo.None}")));

			Languages.AddRange(cultures.ToDictionary(x => x.Name, x => x.DisplayName).Distinct().OrderBy(x => x.Value));
		}

		public string Title
		{
			get { return assemblyInfo.Title; }
			set { assemblyInfo.Title = value; OnPropertyChanged(); }
		}

		public string Description
		{
			get { return assemblyInfo.Description; }
			set { assemblyInfo.Description = value; OnPropertyChanged(); }
		}

		public string Company
		{
			get { return assemblyInfo.Company; }
			set { assemblyInfo.Company = value; OnPropertyChanged(); }
		}

		public string Product
		{
			get { return assemblyInfo.Product; }
			set { assemblyInfo.Product = value; OnPropertyChanged(); }
		}

		public string Copyright
		{
			get { return assemblyInfo.Copyright; }
			set { assemblyInfo.Copyright = value; OnPropertyChanged(); }
		}

		public string Trademark
		{
			get { return assemblyInfo.Trademark; }
			set { assemblyInfo.Trademark = value; OnPropertyChanged(); }
		}

		public string DefaultAlias
		{
			get { return assemblyInfo.DefaultAlias; }
			set { assemblyInfo.DefaultAlias = value; OnPropertyChanged(); }
		}

		public string AssemblyVersion
		{
			get { return assemblyInfo.AssemblyVersion; }
			set { assemblyInfo.AssemblyVersion = value; OnPropertyChanged(); }
		}

		public string AssemblyFileVersion
		{
			get { return assemblyInfo.AssemblyFileVersion; }
			set { assemblyInfo.AssemblyFileVersion = value; OnPropertyChanged(); }
		}

		public string InformationalVersion
		{
			get { return assemblyInfo.InformationalVersion; }
			set { assemblyInfo.InformationalVersion = value; OnPropertyChanged(); }
		}

		public Guid? Guid
		{
			get { return assemblyInfo.Guid; }
			set { assemblyInfo.Guid = value; OnPropertyChanged(); }
		}

		public string NeutralLanguage
		{
			get
			{
				return assemblyInfo.NeutralLanguage ?? NONE_LANGUAGE_CODE;
			}

			set
			{
				assemblyInfo.NeutralLanguage = value == NONE_LANGUAGE_CODE ? null : value;
				OnPropertyChanged();
			}
		}

		public bool ComVisible
		{
			get { return assemblyInfo.ComVisible; }
			set { assemblyInfo.ComVisible = value; OnPropertyChanged(); }
		}

		public bool ClsCompliant
		{
			get { return assemblyInfo.ClsCompliant; }
			set { assemblyInfo.ClsCompliant = value; OnPropertyChanged(); }
		}

		public bool JitOptimization
		{
			get { return assemblyInfo.JitOptimization; }
			set { assemblyInfo.JitOptimization = value; OnPropertyChanged(); }
		}

		public bool JitTracking
		{
			get { return assemblyInfo.JitTracking; }
			set { assemblyInfo.JitTracking = value; OnPropertyChanged(); }
		}

		public ICommand NewGuidCommand { get; private set; }

		public Dictionary<string, string> Languages { get; private set; } 
	}
}