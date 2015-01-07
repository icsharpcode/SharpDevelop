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
			DependencyProperty.Register("Version", typeof(string), typeof(VersionEditor), new PropertyMetadata(OnVersionChanged));
		public static DependencyProperty TypeProperty =
			DependencyProperty.Register("Type", typeof(VersionType), typeof(VersionEditor));

		public VersionEditor()
		{
			InitializeComponent();
		}

		public string Version
		{
			get { return GetValue(VersionProperty) as string; }
			set { SetValue(VersionProperty, value); }
		}

		public VersionType Type
		{
			get { return (VersionType)GetValue(TypeProperty); }
			set { SetValue(TypeProperty, value); }
		}

		public enum VersionType
		{
			Assembly,
			File,
			Info,
		}

		enum AllowedType
		{
			Integer,
			Star,
			Empty,
			String,
		}

		static readonly AllowedType[][] assemblyVersionTypes =
		{
			new [] { AllowedType.Integer, AllowedType.Integer, AllowedType.Integer, AllowedType.Integer },
			new [] { AllowedType.Integer, AllowedType.Integer, AllowedType.Integer, AllowedType.Empty },
			new [] { AllowedType.Integer, AllowedType.Integer, AllowedType.Integer, AllowedType.Star },
			new [] { AllowedType.Integer, AllowedType.Integer, AllowedType.Empty, AllowedType.Empty },
			new [] { AllowedType.Integer, AllowedType.Integer, AllowedType.Star, AllowedType.Empty },
			new [] { AllowedType.Integer, AllowedType.Empty, AllowedType.Empty, AllowedType.Empty },
			new [] { AllowedType.Integer, AllowedType.Star, AllowedType.Empty, AllowedType.Empty },
		};
		static readonly AllowedType[][] fileVersionTypes =
		{
			new [] { AllowedType.Integer, AllowedType.Integer, AllowedType.Integer, AllowedType.Integer },
			new [] { AllowedType.Integer, AllowedType.Integer, AllowedType.Integer, AllowedType.Empty },
			new [] { AllowedType.Integer, AllowedType.Integer, AllowedType.Empty, AllowedType.Empty },
			new [] { AllowedType.Integer, AllowedType.Empty, AllowedType.Empty, AllowedType.Empty },
		};
		static readonly AllowedType[][] infoVersionTypes =
		{
			new [] { AllowedType.String, AllowedType.String, AllowedType.String, AllowedType.String },
			new [] { AllowedType.String, AllowedType.String, AllowedType.String, AllowedType.Empty },
			new [] { AllowedType.String, AllowedType.String, AllowedType.Empty, AllowedType.Empty },
			new [] { AllowedType.String, AllowedType.Empty, AllowedType.Empty, AllowedType.Empty },
		};
		static readonly int[] assemblyVersionRange = { 0, 65534 };
		static readonly int[] fileVersionRange = { 0, 65535 };

		AllowedType[] GetAllowedType(string[] parts)
		{
			AllowedType[][] types = GetAllowedTypes();
			int[] range = GetAllowedRange(); 
			bool allowed = false;
			foreach (var element in types)
			{
				allowed = true;
				for (int i = 0; i < parts.Length; i++)
				{
					int t;
					switch (element[i])
					{
						case AllowedType.Integer:
							if (!int.TryParse(parts[i], out t))
								allowed = false;
							if (range != null && (t < range[0] || t > range[1]))
								allowed = false;
							break;
						case AllowedType.Star:
							if (parts[i] != "*")
								allowed = false;
							break;
						case AllowedType.Empty:
							if (!string.IsNullOrEmpty(parts[i]))
								allowed = false;
							break;
						case AllowedType.String:
							if (string.IsNullOrEmpty(parts[i]))
								allowed = false;
							break;
					}

					if (!allowed)
						break;
				}

				if (allowed)
					return element;
			}

			return null;
		}

		AllowedType[][] GetAllowedTypes()
		{
			if (this.Type == VersionType.Assembly)
				return assemblyVersionTypes;
			if (this.Type == VersionType.File)
				return fileVersionTypes;
			return infoVersionTypes;
		}

		int[] GetAllowedRange()
		{
			if (this.Type == VersionType.Assembly)
				return assemblyVersionRange;
			if (this.Type == VersionType.File)
				return fileVersionRange;
			return null;
		}

		private void OnTextChanged(object sender, TextChangedEventArgs e)
		{
			// Get new Version class instance from textboxes 
			string majorPart = majorTextBox.Text;
			string minorPart = minorTextBox.Text;
			string buildPart = buildTextBox.Text;
			string revisionPart = revisionTextBox.Text;

			AllowedType[] type = GetAllowedType(new string[] { majorPart, minorPart, buildPart, revisionPart });
			if (type == null)
				type = GetAllowedType(new string[] { majorPart, minorPart, buildPart, "" });
			if (type == null)
				type = GetAllowedType(new string[] { majorPart, minorPart, "", "" });
			if (type == null)
				type = GetAllowedType(new string[] { majorPart, "", "", "" });

			string newVersion;
			if (type != null)
			{
				if (type[3] != AllowedType.Empty)
					newVersion = majorPart + "." + minorPart + "." + buildPart + "." + revisionPart;
				else if (type[2] != AllowedType.Empty)
					newVersion = majorPart + "." + minorPart + "." + buildPart;
				else if (type[1] != AllowedType.Empty)
					newVersion = majorPart + "." + minorPart;
				else if (type[0] != AllowedType.Empty)
					newVersion = majorPart;
				else
					newVersion = "";
			}
			else
				newVersion = "";

			if (!newVersion.Equals(Version))
				Version = newVersion;
		}

		static string[] SplitNParts(string s, char c, int count)
		{
			var parts = new List<string>();
			int pos = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (s[i] == c)
				{
					if (parts.Count + 1 == count)
						break;

					parts.Add(s.Substring(pos, i - pos));
					pos = i + 1;
				}
			}

			parts.Add(s.Substring(pos));
			return parts.ToArray();
		}

		private static void OnVersionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var versionEditor = d as VersionEditor;
			var newVersion = e.NewValue as string;
			if (versionEditor != null && newVersion != null)
			{
				var parts = SplitNParts(newVersion, '.', 4);
				// Update textboxes values when version property changes
				versionEditor.majorTextBox.Text = parts.Length > 0 ? parts[0] : string.Empty;
				versionEditor.minorTextBox.Text = parts.Length > 1 ? parts[1] : string.Empty;
				versionEditor.buildTextBox.Text = parts.Length > 2 ? parts[2] : string.Empty;
				versionEditor.revisionTextBox.Text = parts.Length > 3 ? parts[3] : string.Empty;
			}
		}
	}
}
