// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Windows.Forms;

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
		Panel panel = new Panel();
		Control userControl;
		
		protected AbstractViewContentHandlingLoadErrors()
		{
		}
		
		protected AbstractViewContentHandlingLoadErrors(OpenedFile file) : base(file)
		{
		}
		
		public sealed override Control Control {
			get { return panel; }
		}
		
		protected Control UserControl {
			get { return userControl; }
			set {
				if (userControl != value) {
					if (errorList.Count == 0 && userControl != null) {
						panel.Controls.Remove(userControl);
					}
					userControl = value;
					if (userControl != null) {
						userControl.Dock = DockStyle.Fill;
						if (errorList.Count == 0) {
							panel.Controls.Add(userControl);
						}
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
				errorTextBox.Multiline = true;
				errorTextBox.ScrollBars = ScrollBars.Both;
				errorTextBox.ReadOnly = true;
				errorTextBox.BackColor = SystemColors.Window;
				errorTextBox.Dock = DockStyle.Fill;
			}
			errorTextBox.Text = String.Concat(this.LoadErrorHeaderText, ex.ToString());
			panel.Controls.Clear();
			panel.Controls.Add(errorTextBox);
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
						panel.Controls.Clear();
						if (userControl != null) {
							panel.Controls.Add(userControl);
						}
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
