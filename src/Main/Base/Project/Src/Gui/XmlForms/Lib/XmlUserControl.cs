// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms
{
	/// <summary>
	/// The basic xml generated user control.
	/// </summary>
	public abstract class XmlUserControl : UserControl
	{
		protected XmlLoader xmlLoader;
		
		/// <summary>
		/// Gets the ControlDictionary for this user control.
		/// </summary>
		public Dictionary<string, Control> ControlDictionary {
			get {
				if (xmlLoader == null)
					return null;
				else
					return xmlLoader.ControlDictionary;
			}
		}
		
		public XmlUserControl()
		{
		}
		
//		/// <summary>
//		/// Creates a new instance
//		/// </summary>
//		/// <param name="fileName">
//		/// Name of the xml file which defines this user control.
//		/// </param>
//		public XmlUserControl(string fileName)
//		{
//			SetupFromXml(fileName);
//		}
		
		
		public T Get<T>(string name) where T: System.Windows.Forms.Control
		{
			return xmlLoader.Get<T>(name);
		}
//
//		protected void SetupFromXml(string fileName)
//		{
//			if (fileName == null) {
//				throw new System.ArgumentNullException("fileName");
//			}
//
//			using (Stream stream = File.OpenRead(fileName)) {
//				SetupFromXmlStream(stream);
//			}
//		}
		
		protected void SetupFromXmlResource(string resourceName)
		{
			Assembly caller = Assembly.GetCallingAssembly();
			resourceName = "Resources." + resourceName;
			SetupFromXmlStream(caller.GetManifestResourceStream(resourceName));
		}
		
		protected void SetupFromXmlStream(Stream stream)
		{
			if (stream == null) {
				throw new System.ArgumentNullException("stream");
			}
			
			SuspendLayout();
			xmlLoader = new XmlLoader();
			SetupXmlLoader();
			if (stream != null) {
				xmlLoader.LoadObjectFromStream(this, stream);
			}
			ResumeLayout(false);
		}
		
		protected virtual void SetupXmlLoader()
		{
		}
	}
}
