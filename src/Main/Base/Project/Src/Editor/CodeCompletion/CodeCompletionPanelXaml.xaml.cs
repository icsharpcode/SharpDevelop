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
