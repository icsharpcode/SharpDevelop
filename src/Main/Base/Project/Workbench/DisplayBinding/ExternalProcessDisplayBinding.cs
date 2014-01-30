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
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Display binding for opening a file in an external process.
	/// </summary>
	[TypeConverter(typeof(ExternalProcessDisplayBindingConverter))]
	public sealed class ExternalProcessDisplayBinding : IDisplayBinding
	{
		public string FileExtension { get; set; }
		public string CommandLine { get; set; }
		public string Title { get; set; }
		public string Id { get; set; }
		
		public bool CanCreateContentForFile(FileName fileName)
		{
			return string.Equals(Path.GetExtension(fileName), FileExtension, StringComparison.OrdinalIgnoreCase);
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			if (file.IsDirty) {
				// TODO: warn user that the file must be saved
			}
			try {
				string cmd;
				if (CommandLine.Contains("%1"))
					cmd = CommandLine.Replace("%1", file.FileName);
				else
					cmd = CommandLine + " \"" + file.FileName + "\"";
				StartCommandLine(cmd, Path.GetDirectoryName(file.FileName));
			} catch (Exception ex) {
				MessageService.ShowError(ex.Message);
			}
			return null;
		}
		
		static void StartCommandLine(string cmd, string workingDir)
		{
			LoggingService.Debug("ExternalProcessDisplayBinding> " + cmd);
			cmd = cmd.Trim();
			if (cmd.Length == 0) return;
			ProcessStartInfo info = new ProcessStartInfo();
			if (cmd[0] == '"') {
				int pos = cmd.IndexOf('"', 1);
				info.FileName = cmd.Substring(1, pos - 1);
				info.Arguments = cmd.Substring(pos + 1).TrimStart();
			} else {
				int pos = cmd.IndexOf(' ', 0);
				info.FileName = cmd.Substring(0, pos);
				info.Arguments = cmd.Substring(pos + 1);
			}
			info.WorkingDirectory = workingDir;
			Process.Start(info);
		}
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return false;
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return double.NegativeInfinity;
		}
	}
	
	sealed class ExternalProcessDisplayBindingConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;
			else
				return base.CanConvertFrom(context, sourceType);
		}
		
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(string))
				return true;
			else
				return base.CanConvertTo(context, destinationType);
		}
		
		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string)) {
				ExternalProcessDisplayBinding binding = (ExternalProcessDisplayBinding)value;
				return binding.Id + "|" + binding.FileExtension + "|" + binding.Title + "|" + binding.CommandLine;
			} else {
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string) {
				string[] values = value.ToString().Split('|');
				if (values.Length == 4) {
					return new ExternalProcessDisplayBinding {
						Id = values[0],
						FileExtension = values[1],
						Title = values[2],
						CommandLine = values[3]
					};
				} else {
					return null;
				}
			} else {
				return base.ConvertFrom(context, culture, value);
			}
		}
	}
}
