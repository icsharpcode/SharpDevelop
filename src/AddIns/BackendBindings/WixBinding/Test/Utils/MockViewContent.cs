// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

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
			SetName("dummy.name");
		}
		
		public void SetName(string fileName)
		{
			primaryFile = new MockOpenedFile(fileName, false);
		}
		
		public void SetUntitledName(string fileName)
		{
			primaryFile = new MockOpenedFile(fileName, true);
		}
		
		class MockOpenedFile : OpenedFile
		{
			public MockOpenedFile(string fileName, bool isUntitled)
			{
				base.FileName = fileName;
				base.IsUntitled = isUntitled;
			}
			
			public override IList<IViewContent> RegisteredViewContents {
				get {
					throw new NotImplementedException();
				}
			}
			
			public override void RegisterView(IViewContent view)
			{
			}
			
			public override void UnregisterView(IViewContent view)
			{
			}
		}
		
		#pragma warning disable 67
		public event EventHandler TabPageTextChanged;
		public event EventHandler Disposed;
		public event EventHandler IsDirtyChanged;
		public event EventHandler TitleNameChanged;
		#pragma warning restore 67
		
		public IList<OpenedFile> Files {
			get {
				return new OpenedFile[] { primaryFile };
			}
		}
		
		public OpenedFile PrimaryFile {
			get { return primaryFile; }
		}
		
		public string PrimaryFileName {
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
		
		public void Save(OpenedFile file, System.IO.Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public void Load(OpenedFile file, System.IO.Stream stream)
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
		
		public Control Control {
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
	}
}
