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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace WixBinding.Tests.Gui
{
	/// <summary>
	/// Tests the SetupDialogListView.AddDialogs method making sure the 
	/// array of dialog ids are added to the list view.
	/// </summary>
	[TestFixture]
	public class AddDialogsToSetupDialogListTestFixture
	{
		int nodesAdded;
		string welcomeDialogText;
		string progressDialogText;
		string welcomeDialogId;
		string progressDialogId;
		string wixDocumentFileName;
		string welcomeDialogFileName;
		string progressDialogFileName;
		string xmlErrorDialogText;
		int xmlErrorDialogErrorLine;
		int xmlErrorDialogErrorColumn;
		Color xmlErrorDialogTextColour;
		Color xmlErrorDialogTextBackColour;
		
		string errorDialogText;
		int errorDialogErrorLine;
		int errorDialogErrorColumn;
		Color errorDialogTextColour;
		Color errorDialogTextBackColour;
		
		bool hasErrors;
		bool hasErrorsAtStart;

		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			List<string> dialogs = new List<string>();
			dialogs.Add("WelcomeDialog");
			dialogs.Add("ProgressDialog");
			wixDocumentFileName = @"C:\Projects\Test\setup.wxs";
			using (SetupDialogListView control = new SetupDialogListView()) {
				control.AddDialogs(wixDocumentFileName, new ReadOnlyCollection<string>(dialogs));
				
				hasErrorsAtStart = control.HasErrors;
				XmlException xmlEx = new XmlException("Error occurred", null, 10, 5);
				control.AddError(wixDocumentFileName, xmlEx);
				Exception ex = new Exception("Error");
				control.AddError(wixDocumentFileName);
				nodesAdded = control.Items.Count;
				
				SetupDialogListViewItem welcomeDialogListItem = (SetupDialogListViewItem)control.Items[0];
				welcomeDialogText = welcomeDialogListItem.Text;
				welcomeDialogId = welcomeDialogListItem.Id;
				welcomeDialogFileName = welcomeDialogListItem.FileName;

				SetupDialogListViewItem progressDialogListItem = (SetupDialogListViewItem)control.Items[1];
				progressDialogText = progressDialogListItem.Text;
				progressDialogId = progressDialogListItem.Id;
				progressDialogFileName = progressDialogListItem.FileName;
				
				SetupDialogErrorListViewItem xmlErrorDialogListItem = (SetupDialogErrorListViewItem)control.Items[2];
				xmlErrorDialogText = xmlErrorDialogListItem.Text;
				xmlErrorDialogErrorLine = xmlErrorDialogListItem.Line;
				xmlErrorDialogErrorColumn = xmlErrorDialogListItem.Column;
				xmlErrorDialogTextColour = xmlErrorDialogListItem.ForeColor;
				xmlErrorDialogTextBackColour = xmlErrorDialogListItem.BackColor;

				SetupDialogErrorListViewItem errorDialogListItem = (SetupDialogErrorListViewItem)control.Items[3];
				errorDialogText = errorDialogListItem.Text;
				errorDialogErrorLine = errorDialogListItem.Line;
				errorDialogErrorColumn = errorDialogListItem.Column;
				errorDialogTextColour = errorDialogListItem.ForeColor;				
				errorDialogTextBackColour = errorDialogListItem.BackColor;
				
				hasErrors = control.HasErrors;
			}
		}
		
		[Test]
		public void TwoDialogsAdded()
		{
			Assert.AreEqual(4, nodesAdded);
		}
		
		[Test]
		public void WelcomeDialogFileName()
		{
			Assert.AreEqual(wixDocumentFileName, welcomeDialogFileName);
		}
		
		[Test]
		public void ProgressDialogFileName()
		{
			Assert.AreEqual(wixDocumentFileName, progressDialogFileName);
		}
		
		[Test]
		public void WelcomeDialogItemText()
		{
			Assert.AreEqual("WelcomeDialog", welcomeDialogText);
		}
		
		[Test]
		public void WelcomeDialogItemId()
		{
			Assert.AreEqual("WelcomeDialog", welcomeDialogId);
		}

		[Test]
		public void ProgressDialogItemText()
		{
			Assert.AreEqual("ProgressDialog", progressDialogText);
		}
		
		[Test]
		public void ProgressDialogItemId()
		{
			Assert.AreEqual("ProgressDialog", progressDialogId);
		}
		
		[Test]
		public void XmlErrorDialogItemText()
		{
			Assert.AreEqual(Path.GetFileName(wixDocumentFileName), xmlErrorDialogText);
		}
		
		[Test]
		public void XmlErrorDialogItemTextColour()
		{
			Assert.AreEqual(Color.White, xmlErrorDialogTextColour);
		}
		
		[Test]
		public void XmlErrorDialogItemTextBackColour()
		{
			Assert.AreEqual(Color.Red, xmlErrorDialogTextBackColour);
		}
		
		[Test]
		public void XmlErrorDialogItemErrorLine()
		{
			Assert.AreEqual(10, xmlErrorDialogErrorLine);
		}
		
		[Test]
		public void XmlErrorDialogItemErrorColumn()
		{
			Assert.AreEqual(5, xmlErrorDialogErrorColumn);
		}
		
		[Test]
		public void ErrorDialogItemText()
		{
			Assert.AreEqual(Path.GetFileName(wixDocumentFileName), errorDialogText);
		}
		
		[Test]
		public void ErrorDialogItemTextColour()
		{
			Assert.AreEqual(Color.White, errorDialogTextColour);
		}
		
		[Test]
		public void ErrorDialogItemTextBackColour()
		{
			Assert.AreEqual(Color.Red, errorDialogTextBackColour);
		}
		
		[Test]
		public void ErrorDialogItemErrorLine()
		{
			Assert.AreEqual(0, errorDialogErrorLine);
		}
		
		[Test]
		public void ErrorDialogItemErrorColumn()
		{
			Assert.AreEqual(0, errorDialogErrorColumn);
		}
		
		[Test]
		public void HasErrors()
		{
			Assert.IsTrue(hasErrors, "SetupDialogListView.HasErrors should be true");
		}
		
		[Test]
		public void HasErrorsAtStart()
		{
			Assert.IsFalse(hasErrorsAtStart, "SetupDialogListView.HasErrors should be false at the start");
		}
	}
}
