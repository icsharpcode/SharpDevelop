// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Base class that is used when creating dialogs from the Wix xml.
	/// </summary>
	public class DialogLoadingTestFixtureBase : IComponentCreator, IFileLoader
	{
		List <CreatedComponent> createdComponents = new List<CreatedComponent>();
		List <string> bitmapFileNamesRequested = new List<string>();

		public DialogLoadingTestFixtureBase()
		{
		}
		
		public IComponent CreateComponent(Type componentClass, string name)
		{
			CreatedComponent c = new CreatedComponent(componentClass.FullName, name);
			createdComponents.Add(c);
			
			object instance = componentClass.Assembly.CreateInstance(componentClass.FullName);
			Control control = instance as Control;
			return (IComponent)instance;
		}
		
		public Bitmap GetBitmap(string fileName)
		{
			bitmapFileNamesRequested.Add(fileName);
			return new Bitmap(10, 10);
		}
		
		/// <summary>
		/// Gets a list of the bitmap filenames requested through the GetBitmapFromFileName
		/// method.</summary>
		protected List<string> BitmapFileNamesRequested {
			get {
				return bitmapFileNamesRequested;
			}
		}
		
		/// <summary>
		/// Gets a list of the components created via the IComponentCreator.Create 
		/// method.</summary>
		protected List<CreatedComponent> CreatedComponents {
			get {
				return createdComponents;
			}
		}
	}
}
