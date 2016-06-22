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

//using System;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.ComponentModel.Design.Serialization;
//using System.Drawing;
//using System.Windows.Forms;
//
//using ICSharpCode.Scripting;
//using ICSharpCode.Scripting.Tests.Utils;
//using NUnit.Framework;
//
//namespace ICSharpCode.Scripting.Tests.Designer
//{
//	public abstract class GenerateUserControlWithNullPropertyValueTestsBase : GenerateDesignerCodeTestsBase
//	{
//		[TestFixtureSetUp]
//		public void SetUpFixture()
//		{
//			using (DesignSurface designSurface = new DesignSurface(typeof(UserControl))) {
//				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
//				IEventBindingService eventBindingService = new MockEventBindingService(host);
//				UserControl userControl = (UserControl)host.RootComponent;
//				userControl.ClientSize = new Size(200, 300);
//				
//				NullPropertyUserControl control = (NullPropertyUserControl)host.CreateComponent(typeof(NullPropertyUserControl), "userControl1");
//				control.Location = new Point(0, 0);
//				control.Size = new Size(10, 10);
//				userControl.Controls.Add(control);
//
//				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(userControl);
//				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
//				namePropertyDescriptor.SetValue(userControl, "MainControl");
//				
//				DesignerSerializationManager serializationManager = new DesignerSerializationManager(host);
//				using (serializationManager.CreateSession()) {
//					IScriptingCodeDomSerializer serializer = CreateSerializer();
//					generatedCode = serializer.GenerateInitializeComponentMethodBody(host, serializationManager);
//				}
//			}
//		}		
//	}
//}
