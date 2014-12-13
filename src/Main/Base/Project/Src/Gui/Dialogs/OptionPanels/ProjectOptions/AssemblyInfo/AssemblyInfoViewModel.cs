using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
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
				string.Format("({0})", StringParser.Parse("${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}")));

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

		public Version AssemblyVersion
		{
			get { return assemblyInfo.AssemblyVersion; }
			set { assemblyInfo.AssemblyVersion = value; OnPropertyChanged(); }
		}

		public Version AssemblyFileVersion
		{
			get { return assemblyInfo.AssemblyFileVersion; }
			set { assemblyInfo.AssemblyFileVersion = value; OnPropertyChanged(); }
		}

		public Version InformationalVersion
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