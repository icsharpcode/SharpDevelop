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
using System.IO;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// IViewContent implementation that opens a file as XDocument and allows editing it, while synchronizing changes with any open editor.
	/// </summary>
	public sealed class FakeXmlViewContent : IViewContent
	{
		public FakeXmlViewContent(string fileName)
		{
			this.PrimaryFile = SD.FileService.GetOrCreateOpenedFile(fileName);
			this.oldView = this.PrimaryFile.CurrentView;
			this.PrimaryFile.RegisterView(this);
			this.PrimaryFile.SwitchedToView(this);
		}
		
		IViewContent oldView;
		XDocument document;
		byte[] fileData;
		
		/// <summary>
		/// Gets the document.
		/// Can return null if there were load errors.
		/// </summary>
		public XDocument Document {
			get { return document; }
		}
		
		public void Dispose()
		{
			if (this.IsDisposed)
				return;
			this.IsDisposed = true;
			if (document != null) {
				this.PrimaryFile.MakeDirty();
				if (this.PrimaryFile.RegisteredViewContents.Count == 1)
					this.PrimaryFile.SaveToDisk();
			}
			this.PrimaryFile.UnregisterView(this);
			if (oldView != null)
				this.PrimaryFile.SwitchedToView(oldView);
			if (Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		
		void IViewContent.Save(OpenedFile file, Stream stream)
		{
			if (document != null)
				document.Save(stream, SaveOptions.DisableFormatting);
			else if (fileData != null)
				stream.Write(fileData, 0, fileData.Length);
		}
		
		void IViewContent.Load(OpenedFile file, Stream stream)
		{
			document = null;
			fileData = null;
			
			try {
				document = XDocument.Load(stream, LoadOptions.PreserveWhitespace);
			} catch (XmlException) {
				stream.Position = 0;
				fileData = new byte[(int)stream.Length];
				int pos = 0;
				while (pos < fileData.Length) {
					int c = stream.Read(fileData, pos, fileData.Length - pos);
					if (c == 0) break;
					pos += c;
				}
			}
		}
		
		#region IViewContent stub implementation
		event EventHandler IViewContent.TabPageTextChanged {
			add { }
			remove { }
		}
		
		event EventHandler IViewContent.TitleNameChanged {
			add { }
			remove { }
		}

		event EventHandler IViewContent.InfoTipChanged {
			add { }
			remove { }
		}
		
		public event EventHandler Disposed;
		
		event EventHandler ICanBeDirty.IsDirtyChanged {
			add { }
			remove { }
		}
		
		object IViewContent.Control {
			get {
				throw new NotImplementedException();
			}
		}
		
		object IViewContent.InitiallyFocusedControl {
			get {
				throw new NotImplementedException();
			}
		}
		
		IWorkbenchWindow IViewContent.WorkbenchWindow {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		string IViewContent.TabPageText {
			get {
				throw new NotImplementedException();
			}
		}
		
		string IViewContent.TitleName {
			get {
				throw new NotImplementedException();
			}
		}
		
		System.Collections.Generic.IList<OpenedFile> IViewContent.Files {
			get { return new [] { PrimaryFile }; }
		}
		
		public OpenedFile PrimaryFile { get; set; }
		
		ICSharpCode.Core.FileName IViewContent.PrimaryFileName {
			get { return PrimaryFile.FileName; }
		}
		
		public bool IsDisposed { get; private set; }
		
		bool IViewContent.IsReadOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		bool IViewContent.IsViewOnly {
			get {
				throw new NotImplementedException();
			}
		}

		string IViewContent.InfoTip {
			get {
				throw new NotImplementedException();
			}
		}
		
		bool IViewContent.CloseWithSolution {
			get {
				throw new NotImplementedException();
			}
		}
		
		System.Collections.Generic.ICollection<IViewContent> IViewContent.SecondaryViewContents {
			get {
				throw new NotImplementedException();
			}
		}
		
		bool ICanBeDirty.IsDirty {
			get {
				throw new NotImplementedException();
			}
		}
		
		INavigationPoint IViewContent.BuildNavPoint()
		{
			throw new NotImplementedException();
		}
		
		bool IViewContent.SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			return false;
		}
		
		bool IViewContent.SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			return false;
		}
		
		void IViewContent.SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			throw new NotImplementedException();
		}
		
		void IViewContent.SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			throw new NotImplementedException();
		}
		
		object IServiceProvider.GetService(Type serviceType)
		{
			return null;
		}
		#endregion
	}
}
