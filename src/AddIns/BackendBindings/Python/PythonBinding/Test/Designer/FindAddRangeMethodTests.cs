// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the AddRange or Add method that takes an array of items can be determined.
	/// </summary>
	[TestFixture]
	public class FindAddRangeMethodTests
	{
		[Test]
		public void MenuStripItemsAddRangeMethod()
		{
			using (MenuStrip menuStrip = new MenuStrip()) {
				MethodInfo expectedMethodInfo = FindMethod(menuStrip.Items, "AddRange", typeof(ToolStripItem[]));
				Assert.IsNotNull(expectedMethodInfo);
				
				Assert.AreSame(expectedMethodInfo, PythonDesignerComponent.GetAddRangeSerializationMethod(menuStrip.Items));
			}
		}
		
		[Test]
		public void GetArrayParameterType()
		{
			using (MenuStrip menuStrip = new MenuStrip()) {
				MethodInfo methodInfo = FindMethod(menuStrip.Items, "AddRange", typeof(ToolStripItem[]));
				Assert.AreEqual(typeof(ToolStripItem), PythonDesignerComponent.GetArrayParameterType(methodInfo));
			}
		}
		
		[Test]
		public void GetArrayParameterTypeFromMethodWithNoParameters()
		{
			MethodInfo methodInfo = typeof(String).GetMethod("Clone");
			Assert.IsNull(PythonDesignerComponent.GetArrayParameterType(methodInfo));
		}
		
		[Test]
		public void GetArrayParameterTypeWithNullMethodInfo()
		{
			Assert.IsNull(PythonDesignerComponent.GetArrayParameterType(null));
		}
		
		/// <summary>
		/// Form.Controls.AddRange() method should not be returned since it is marked with
		/// DesignerSerializationVisibility.Hidden.
		/// </summary>
		[Test]
		public void FormControlsAddRangeMethodNotFound()
		{
			using (Form form = new Form()) {
				Assert.IsNull(PythonDesignerComponent.GetAddRangeSerializationMethod(form.Controls));
			}
		}
		
		[Test]
		public void FormControlsAddMethod()
		{
			using (Form form = new Form()) {
				MethodInfo expectedMethodInfo = FindMethod(form.Controls, "Add", typeof(Control));
				Assert.IsNotNull(expectedMethodInfo);
				
				Assert.AreSame(expectedMethodInfo, PythonDesignerComponent.GetAddSerializationMethod(form.Controls));
			}
		}
		
		static MethodInfo FindMethod(object obj, string methodName, Type parameterType)
		{
			foreach (MethodInfo methodInfo in obj.GetType().GetMethods()) {
				if (methodInfo.Name == methodName) {
					ParameterInfo[] parameters = methodInfo.GetParameters();
					if (parameters.Length == 1) {
						ParameterInfo param = parameters[0];
						if (param.ParameterType == parameterType) {
							return methodInfo;
						}
					}
				}
			}
			return null;
		}
	}
}
