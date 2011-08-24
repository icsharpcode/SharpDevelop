// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
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
		
		public bool CanCreateContentForFile(string fileName)
		{
			return string.Equals(Path.GetExtension(fileName), FileExtension, StringComparison.OrdinalIgnoreCase);
		}
		
		public ICSharpCode.SharpDevelop.Gui.IViewContent CreateContentForFile(OpenedFile file)
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
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			return false;
		}
		
		public double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType)
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
