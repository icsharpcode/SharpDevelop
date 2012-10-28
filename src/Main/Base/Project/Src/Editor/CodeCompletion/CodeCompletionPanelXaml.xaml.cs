/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 22.10.2012
 * Time: 20:11
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Editor.CodeCompletion
{
	/// <summary>
	/// Interaction logic for CodeCompletionPanelXaml.xaml
	/// </summary>
	public partial class CodeCompletionPanel : OptionPanel
	{
		public CodeCompletionPanel()
		{
			InitializeComponent();
			this.DataContext = this;
			WrapText = StringParser.Parse("${res:Dialog.Options.IDEOptions.CodeCompletion.LanguageDependend}");
		}
		
		#region overrides
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			this.EnableCodeCompletion = CodeCompletionOptions.EnableCodeCompletion;
			this.UseDataUsageCache = CodeCompletionOptions.DataUsageCacheEnabled;
			DataUsageCacheItemCount = CodeCompletionOptions.DataUsageCacheItemCount;
			UseTooltips = CodeCompletionOptions.TooltipsEnabled;
			useDebugTooltipsOnly.IsChecked =  CodeCompletionOptions.TooltipsOnlyWhenDebugging;
			completeWhenTyping.IsChecked = CodeCompletionOptions.CompleteWhenTyping;
			useKeywordCompletionCheckBox.IsChecked = CodeCompletionOptions.KeywordCompletionEnabled;
			UseInsight = CodeCompletionOptions.InsightEnabled;
			refreshInsightOnComma.IsChecked = CodeCompletionOptions.InsightRefreshOnComma;
		}
		
		
		public override bool SaveOptions()
		{
			CodeCompletionOptions.EnableCodeCompletion = this.EnableCodeCompletion;
			CodeCompletionOptions.DataUsageCacheEnabled = this.UseDataUsageCache;
			CodeCompletionOptions.DataUsageCacheItemCount =  DataUsageCacheItemCount;
			CodeCompletionOptions.TooltipsEnabled =  UseTooltips;
			CodeCompletionOptions.TooltipsOnlyWhenDebugging = (bool)useDebugTooltipsOnly.IsChecked;
			CodeCompletionOptions.CompleteWhenTyping = (bool)completeWhenTyping.IsChecked;
			CodeCompletionOptions.KeywordCompletionEnabled = (bool)useKeywordCompletionCheckBox.IsChecked;
			CodeCompletionOptions.InsightEnabled = UseInsight;
			CodeCompletionOptions.InsightRefreshOnComma = (bool)refreshInsightOnComma.IsChecked;
			return base.SaveOptions();
		}
		
		#endregion
		
		public string WrapText {get; private set;}
		
		#region Properties
		
		private bool enableCodeCompletion;
		
		public bool EnableCodeCompletion {
			get { return enableCodeCompletion; }
			set { enableCodeCompletion = value;
				base.RaisePropertyChanged(() => EnableCodeCompletion);}
		}
		
		
		private bool useDataUsageCache;
		
		public bool UseDataUsageCache {
			get { return useDataUsageCache; }
			set { useDataUsageCache = value;
				base.RaisePropertyChanged(() => UseDataUsageCache);}
		}
		
		private int dataUsageCacheItemCount;
		
		public int DataUsageCacheItemCount {
			get { return dataUsageCacheItemCount; }
			set { dataUsageCacheItemCount = value;
				base.RaisePropertyChanged(() => DataUsageCacheItemCount);}
		}
		
		
		private bool useTooltips;
		
		public bool UseTooltips {
			get { return useTooltips; }
			set { useTooltips = value;
				base.RaisePropertyChanged(() => UseTooltips);}
		}
		
		private bool useInsight;
		
		public bool UseInsight {
			get { return useInsight; }
			set { useInsight = value;
				base.RaisePropertyChanged(() => UseInsight);}
		}
		
		#endregion
		
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			ICSharpCode.SharpDevelop.Editor.CodeCompletion.CodeCompletionDataUsageCache.ResetCache();
		}
	}
}