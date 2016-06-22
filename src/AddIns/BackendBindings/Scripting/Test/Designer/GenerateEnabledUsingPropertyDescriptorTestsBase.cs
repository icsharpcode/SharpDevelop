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
//using ICSharpCode.Scripting;
//using NUnit.Framework;
//
//namespace ICSharpCode.Scripting.Tests.Designer
//{
//	public abstract class GenerateEnabledUsingPropertyDescriptorTestsBase : GenerateDesignerCodeTestsBase
//	{
//		[TestFixtureSetUp]
//		public void SetUpFixture()
//		{
//			using (DesignSurface designSurface = new DesignSurface(typeof(Form))) {
//				IDesignerHost host = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost));
//				Form form = (Form)host.RootComponent;			
//				form.ClientSize = new Size(284, 264);
//				form.AllowDrop = false;
//				form.Enabled = false;
//				
//				PropertyDescriptorCollection descriptors = TypeDescriptor.GetProperties(form);
//				PropertyDescriptor namePropertyDescriptor = descriptors.Find("Name", false);
//				namePropertyDescriptor.SetValue(form, "MainForm");
//
//				PropertyDescriptor enabledPropertyDescriptor = descriptors.Find("Enabled", false);
//				enabledPropertyDescriptor.SetValue(form, false);
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
