// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.SpellChecker.Windows
{
	/// <summary>
	/// Interaction logic for SetLanguageWindow.xaml
	/// </summary>
	public partial class SetLanguageWindow : Window
	{
		public SetLanguageWindow()
		{
			InitializeComponent();
			
			this.Loaded += new RoutedEventHandler(WindowLoaded);
		}

		void WindowLoaded(object sender, RoutedEventArgs e)
		{
			this.DictionariesList.ItemsSource = CreateSource(Constants.DefaultLanguage);
		}
		
		void LinkButton_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("http://wiki.services.openoffice.org/wiki/Dictionaries");
		}
		
		void DefaultButton_Click(object sender, RoutedEventArgs e)
		{
			if (DictionariesList.SelectedItem == null)
				return;
			
			string selectedLanguage = DictionariesList.SelectedItem.ToString();		
			if (selectedLanguage.Contains("default"))
				return;
			
			this.DictionariesList.ItemsSource = CreateSource(selectedLanguage);
			Constants.DefaultLanguage = selectedLanguage;
			Constants.CreateHunspell();
		}
		
		List<string> CreateSource(string defaultLanguage)
		{
			List<string> source = new List<string>();
			foreach(var dir in new DirectoryInfo(Constants.DictionariesPath)
			        					.GetDirectories("*-*", SearchOption.TopDirectoryOnly))
			{	
				if(dir.Name == defaultLanguage)
					source.Add(dir.Name + " (default)");
				else
					source.Add(dir.Name);
			}
			
			return source;
		}
	}
}