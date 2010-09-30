// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Designer
{
	public abstract class GenerateUserControlWithNullPropertyValueTestsBase : GenerateDesignerCodeTestsBase
	{
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			using (DesignSurface designSurface = new DesignSurface(typeof(UserControl))) {
				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
				IEventBindingService eventBindingService = new MockEventBindingService(host);
				UserControl userControl = (UserControl)host.RootComponent;
				userControl.ClientSize = new Size(200, 300);
				
				NullPropertyUserControl control = (NullPropertyUserControl)host.CreateComponent(typeof(NullPropertyUserControl), "userControl1");
				control.Location = new Point(0, 0);
				control.Size = new Size(10, 10);
				userControl.Controls.Add(control);

				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(userControl);
				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
				namePropertyDescriptor.SetValue(userControl, "MainControl");
				
				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
				using (serializationManager.CreateSession()) {
					IScriptingCodeDomSerializer serializer = CreateSerializer();
					generatedCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager);
				}
			}
		}		
	}
}
