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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.ProjectOptions
{
	/// <summary>
	/// Interaction logic for VersionEditor.xaml
	/// </summary>
	public partial class VersionEditor
	{
		public static DependencyProperty VersionProperty =
			DependencyProperty.Register("Version", typeof(Version), typeof(VersionEditor), new PropertyMetadata(OnVersionChanged));

		public VersionEditor()
		{
			InitializeComponent();
		}

		public Version Version
		{
			get { return GetValue(VersionProperty) as Version; }
			set { SetValue(VersionProperty, value); }
		}

		private void OnTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Block any non-character input
			if (!e.Text.All(char.IsDigit))
			{
				e.Handled = true;
			}
		}

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			// Get new Version class instance from textboxes 
			string majorPart = majorTextBox.Text;
			string minorPart = minorTextBox.Text;
			string buildPart = buildTextBox.Text;
			string revisionPart = revisionTextBox.Text;

			int majorVersion = 0, minorVersion = 0, build = 0, revision = 0;

			var majorVersionWasSet = !string.IsNullOrEmpty(majorPart) && int.TryParse(majorPart, out majorVersion);
			var minorVersionWasSet = !string.IsNullOrEmpty(minorPart) && int.TryParse(minorPart, out minorVersion);
			var buildWasSet = !string.IsNullOrEmpty(buildPart) && int.TryParse(buildPart, out build);
			var revisionWasSet = !string.IsNullOrEmpty(revisionPart) && int.TryParse(revisionPart, out revision);

			Version newVersion;

			if (revisionWasSet)
				newVersion = new Version(majorVersion, minorVersion, build, revision);
			else if (buildWasSet)
				newVersion = new Version(majorVersion, minorVersion, build);
			else if (majorVersionWasSet || minorVersionWasSet)
				newVersion = new Version(majorVersion, minorVersion);
			else
				newVersion = new Version();

			if (!newVersion.Equals(Version))
				Version = newVersion;
		}

		private static void OnVersionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var versionEditor = d as VersionEditor;
			var newVersion = e.NewValue as Version;
			if (versionEditor != null && newVersion != null)
			{
				// Update textboxes values when version property changes
				versionEditor.majorTextBox.Text = newVersion.Major >= 0 ? newVersion.Major.ToString() : string.Empty;
				versionEditor.minorTextBox.Text = newVersion.Minor >= 0 ? newVersion.Minor.ToString() : string.Empty;
				versionEditor.buildTextBox.Text = newVersion.Build >= 0 ? newVersion.Build.ToString() : string.Empty;
				versionEditor.revisionTextBox.Text = newVersion.Revision >= 0 ? newVersion.Revision.ToString() : string.Empty;
			}
		}
	}
}
