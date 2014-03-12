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
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using CSharpBinding.FormattingStrategy;

namespace CSharpBinding.OptionPanels
{
	/// <summary>
	/// Option panel for global formatting settings.
	/// </summary>
	internal class CSharpGlobalFormattingOptionPanel : CSharpFormattingOptionPanel
	{
		public CSharpGlobalFormattingOptionPanel()
			: base(CSharpFormattingOptionsPersistence.GlobalOptions, true)
		{
		}
	}
	
	/// <summary>
	/// Option panel for solution-wide formatting settings.
	/// </summary>
	internal class CSharpSolutionFormattingOptionPanel : CSharpFormattingOptionPanel
	{
		public CSharpSolutionFormattingOptionPanel()
			: base(CSharpFormattingOptionsPersistence.SolutionOptions, true)
		{
		}
	}
	
	/// <summary>
	/// Interaction logic for CSharpFormattingOptionPanel.xaml
	/// </summary>
	internal partial class CSharpFormattingOptionPanel : OptionPanel
	{
		readonly CSharpFormattingOptionsPersistence persistenceHelper;
		
		public CSharpFormattingOptionPanel(CSharpFormattingOptionsPersistence persistenceHelper, bool allowPresets)
		{
			if (persistenceHelper == null)
				throw new ArgumentNullException("persistenceHelper");
			
			this.persistenceHelper = persistenceHelper;
			InitializeComponent();			
			
			formattingEditor.AllowPresets = allowPresets;
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			persistenceHelper.Load();
			formattingEditor.OptionsContainer = persistenceHelper.StartEditing(); 
		}
		
		public override bool SaveOptions()
		{
			return persistenceHelper.Save();
		}
	}
}