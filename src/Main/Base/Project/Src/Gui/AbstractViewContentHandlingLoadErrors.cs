// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This class handles errors in the Load method and prevents destroying invalid files.
	/// Scenario:
	/// open a .resx file both with the resource editor and the text editor. Add a string entry using the
	/// resource editor. Modify it using the text editor, and introduce invalid XML syntax (e.g. use a
	/// &lt; in the edited value. Close the text editor without saving the changes. The resource editor
	/// will show the load error. Close the resource editor, this time saving the changes.
	/// The resource editor is now expected to write the invalid file to disk.
	/// This class handles this scenario by displaying an error message for invalid files, and
	/// holding the invalid data that got copied from the text editor to the resource editor in memory.
	/// So saving during a load error works as expected.
	/// </summary>
	public abstract class AbstractViewContentHandlingLoadErrors : AbstractViewContent
	{
		ContentPresenter contentControl = new ContentPresenter();
		object userContent;
		
		protected AbstractViewContentHandlingLoadErrors()
		{
		}
		
		protected AbstractViewContentHandlingLoadErrors(OpenedFile file) : base(file)
		{
		}
		
		public sealed override object Control {
			get { return contentControl; }
		}
		
		protected object UserContent {
			get { return userContent; }
			set {
				if (userContent != value) {
					userContent = value;
					if (errorList.Count == 0) {
						contentControl.SetContent(userContent, this);
					}
				}
			}
		}
		
		public bool HasLoadError {
			get {
				return errorList.Count > 0;
			}
		}
		
		class LoadError
		{
			internal Exception exception;
			internal byte[] fileData;
			
			public LoadError(Exception exception, Stream stream)
			{
				this.exception = exception;
				stream.Position = 0;
				this.fileData = new byte[(int)stream.Length];
				int pos = 0;
				while (pos < fileData.Length) {
					int c = stream.Read(fileData, pos, fileData.Length - pos);
					if (c == 0) break;
					pos += c;
				}
			}
		}
		
		TextBox errorTextBox;
		
		void ShowError(Exception ex)
		{
			if (errorTextBox == null) {
				errorTextBox = new TextBox();
				errorTextBox.IsReadOnly = true;
				errorTextBox.Background = SystemColors.WindowBrush;
			}
			errorTextBox.Text = String.Concat(this.LoadErrorHeaderText, ex.ToString());
			contentControl.SetContent(errorTextBox, this);
		}
		
		Dictionary<OpenedFile, LoadError> errorList = new Dictionary<OpenedFile, LoadError>();
		
		/// <summary>
		/// Gets a text to be shown above the exception when a load error occurs.
		/// The default is an empty string.
		/// </summary>
		protected virtual string LoadErrorHeaderText {
			get { return String.Empty; }
		}
		
		public override sealed void Load(OpenedFile file, Stream stream)
		{
			try {
				LoadInternal(file, new UnclosableStream(stream));
				if (errorList.Count > 0) {
					errorList.Remove(file);
					if (errorList.Count == 0) {
						contentControl.SetContent(userContent, this);
					} else {
						ShowError(errorList.Values.First().exception);
					}
				}
			} catch (Exception ex) {
				errorList[file] = new LoadError(ex, stream);
				ShowError(ex);
			}
		}
		
		public override sealed void Save(OpenedFile file, Stream stream)
		{
			if (errorList.ContainsKey(file)) {
				byte[] data = errorList[file].fileData;
				stream.Write(data, 0, data.Length);
			} else {
				SaveInternal(file, stream);
			}
		}
		
		protected abstract void LoadInternal(OpenedFile file, Stream stream);
		protected abstract void SaveInternal(OpenedFile file, Stream stream);
	}
}
