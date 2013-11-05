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
	[Obsolete("XML Forms are obsolete")]
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
		
		public T Get<T>(string name) where T: System.Windows.Forms.Control
		{
			return xmlLoader.Get<T>(name);
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
