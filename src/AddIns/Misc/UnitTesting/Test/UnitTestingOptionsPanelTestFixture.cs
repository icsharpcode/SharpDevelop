// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.UnitTesting;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace UnitTesting.Tests
{
	/// <summary>
	/// Tests the UnitTestingOptionsPanel.
	/// </summary>
	[TestFixture]
	public class UnitTestingOptionsPanelTestFixture
	{
		DerivedUnitTestingOptionsPanel panel;
		UnitTestingOptions options;
		CheckBox labelsCheckBox;
		CheckBox showLogoCheckBox;
		CheckBox showProgressCheckBox;
		CheckBox threadCheckBox;
		CheckBox shadowCopyCheckBox;
		CheckBox createXmlOutputFileCheckBox;
		
		[SetUp]
		public void SetUp()
		{
			Properties p = new Properties();
			options = new UnitTestingOptions(p);
			options.Labels = true;
			options.NoDots = false;
			options.NoShadow = false;
			options.NoThread = false;
			options.CreateXmlOutputFile = false;
			
			panel = new DerivedUnitTestingOptionsPanel(options);
			panel.LoadPanelContents();
			
			labelsCheckBox = (CheckBox)panel.ControlDictionary["labelsCheckBox"];
			showLogoCheckBox = (CheckBox)panel.ControlDictionary["showLogoCheckBox"];
			showProgressCheckBox = (CheckBox)panel.ControlDictionary["showProgressCheckBox"];
			threadCheckBox = (CheckBox)panel.ControlDictionary["threadCheckBox"];
			shadowCopyCheckBox = (CheckBox)panel.ControlDictionary["shadowCopyCheckBox"];			
			createXmlOutputFileCheckBox = (CheckBox)panel.ControlDictionary["createXmlOutputFileCheckBox"];			
		}
		
		[TearDown]
		public void TearDown()
		{
			panel.Dispose();
		}
		
		[Test]
		public void IsAbstractOptionPanel()
		{
			Assert.IsInstanceOf(typeof(AbstractOptionPanel), panel);
		}

		[Test]
		public void SetupFromManifestStreamResourceName()
		{
			Assert.AreEqual("ICSharpCode.UnitTesting.Resources.UnitTestingOptionsPanel.xfrm", panel.SetupFromManifestResourceName);
		}
		
		[Test]
		public void LabelsCheckBoxIsChecked()
		{
			Assert.IsTrue(labelsCheckBox.Checked);
		}
		
		[Test]
		public void LabelsSettingSaved()
		{
			labelsCheckBox.Checked = false;
			panel.StorePanelContents();
			Assert.IsFalse(options.Labels);
		}
		
		[Test]
		public void ShowLogoCheckBoxIsChecked()
		{
			Assert.IsTrue(showLogoCheckBox.Checked);
		}
		
		[Test]
		public void ShowLogoSettingSaved()
		{
			showLogoCheckBox.Checked = false;
			panel.StorePanelContents();
			Assert.IsTrue(options.NoLogo);
		}
		
		[Test]
		public void ShowProgressCheckBoxIsChecked()
		{
			Assert.IsTrue(showProgressCheckBox.Checked);
		}
		
		[Test]
		public void ShowProgressSettingSaved()
		{
			showProgressCheckBox.Checked = false;
			panel.StorePanelContents();
			Assert.IsTrue(options.NoDots);
		}

		[Test]
		public void ShadowCopyCheckBoxIsChecked()
		{
			Assert.IsTrue(shadowCopyCheckBox.Checked);
		}
		
		[Test]
		public void ShadowCopySettingSaved()
		{
			shadowCopyCheckBox.Checked = false;
			panel.StorePanelContents();
			Assert.IsTrue(options.NoShadow);
		}

		[Test]
		public void ThreadCheckBoxIsChecked()
		{
			Assert.IsTrue(threadCheckBox.Checked);
		}
		
		[Test]
		public void ThreadSettingSaved()
		{
			threadCheckBox.Checked = false;
			panel.StorePanelContents();
			Assert.IsTrue(options.NoThread);
		}
		
		[Test]
		public void CreateXmlOutputFileCheckBoxIsChecked()
		{
			Assert.IsFalse(createXmlOutputFileCheckBox.Checked);
		}
		
		[Test]
		public void CreateXmlOutputFileSettingSaved()
		{
			options.CreateXmlOutputFile = false;
			createXmlOutputFileCheckBox.Checked = true;
			panel.StorePanelContents();
			Assert.IsTrue(options.CreateXmlOutputFile);
		}		
	}
}
