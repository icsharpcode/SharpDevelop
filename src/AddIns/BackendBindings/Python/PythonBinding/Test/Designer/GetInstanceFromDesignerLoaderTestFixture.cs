// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the GetInstance method of the PythonDesignerLoader.
	/// </summary>
	[TestFixture]
	public class GetInstanceFromDesignerLoaderTestFixture
	{
		PythonDesignerLoader loader;
		MockDesignerLoaderHost host;
		ListViewItem listViewItem1;
		object instance;
		
		[SetUp]
		public void Init()
		{
			host = new MockDesignerLoaderHost();
			loader = new PythonDesignerLoader(new MockDesignerGenerator());
			loader.BeginLoad(host);
			
			DesignerSerializationManager designerSerializationManager = (DesignerSerializationManager)host.GetService(typeof(IDesignerSerializationManager));
			using (designerSerializationManager.CreateSession()) {	
				listViewItem1 = (ListViewItem)loader.CreateInstance(typeof(ListViewItem), new object[0], "listViewItem1", false);
				instance = loader.GetInstance("listViewItem1");
			}
		}
		
		
		[Test]
		public void GetListViewInstance()
		{
			Assert.AreEqual(listViewItem1, instance);
		}
	}
}
