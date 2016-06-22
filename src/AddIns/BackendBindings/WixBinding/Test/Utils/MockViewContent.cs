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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Workbench;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Mock IViewContent class.
	/// </summary>
	public class MockViewContent : IViewContent
	{
		OpenedFile primaryFile;
		List<IViewContent> secondaryViews = new List<IViewContent>();
		
		public MockViewContent()
		{
			SetFileName("dummy.name");
		}
		
		public void SetFileName(string fileName)
		{
			primaryFile = new MockOpenedFile(fileName, false);
		}
		
		public void SetUntitledFileName(string fileName)
		{
			primaryFile = new MockOpenedFile(fileName, true);
		}
		
		#pragma warning disable 67
		public event EventHandler TabPageTextChanged;
		public event EventHandler Disposed;
		public event EventHandler IsDirtyChanged;
		public event EventHandler TitleNameChanged;
		public event EventHandler InfoTipChanged;
		#pragma warning restore 67
		
		public IList<OpenedFile> Files {
			get {
				throw new NotImplementedException();
			}
		}
		
		public OpenedFile PrimaryFile {
			get { return primaryFile; }
			set { primaryFile = value; }
		}
		
		public FileName PrimaryFileName {
			get { return primaryFile.FileName; }
		}
		
		public bool IsDisposed {
			get { return false; }
		}
		
		public ICollection<IViewContent> SecondaryViewContents {
			get {
				return secondaryViews;
			}
		}
		
		public void Save(OpenedFile file, Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public void Load(OpenedFile file, Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			throw new NotImplementedException();
		}
		
		public bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			throw new NotImplementedException();
		}
		
		public void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
		{
			throw new NotImplementedException();
		}
		
		public void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
		{
			throw new NotImplementedException();
		}
		
		public object Control {
			get {
				throw new NotImplementedException();
			}
		}
		
		public object InitiallyFocusedControl {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IWorkbenchWindow WorkbenchWindow {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string TabPageText {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string TitleName {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string InfoTip {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsReadOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool IsViewOnly {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool CloseWithSolution {
			get { throw new NotImplementedException(); }
		}
		
		public bool IsDirty {
			get {
				throw new NotImplementedException();
			}
		}
		
		public void RedrawContent()
		{
			throw new NotImplementedException();
		}
		
		public INavigationPoint BuildNavPoint()
		{
			throw new NotImplementedException();
		}
		
		public void Dispose()
		{
			throw new NotImplementedException();
		}
		
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(ITextEditor)) {
				var textEditorView = this as MockTextEditorViewContent;
				if (textEditorView != null) {
					return textEditorView.TextEditor;
				}
			}
			return null;
		}
	}
}
