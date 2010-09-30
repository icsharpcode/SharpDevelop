// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Scripting;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Designer
{
	public abstract class GenerateEnabledUsingPropertyDescriptorTestsBase : GenerateDesignerCodeTestsBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				Form form = (Form)host.RootComponent;			
				form.ClientSize = new Size(284, 264);
				form.AllowDrop = false;
				form.Enabled = false;
				
				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(form, "MainForm");

				PropertyDescriptor enabledPropertyDescriptor = descriptors.Find("Enabled", false);
				enabledPropertyDescriptor.SetValue(form, false);
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					IScriptingCodeDomSerializer serializer = CreateSerializer();
					generatedCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager);
				}
			}
		}
	}
}
