// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.UnitTesting;

namespace UnitTesting.Tests.Utils
{
	/// <summary>
	/// Utility class used to test the UnitTestingOptionsPanel class.
	/// </summary>
	public class DerivedUnitTestingOptionsPanel : UnitTestingOptionsPanel
	{
		string setupFromManifestResourceName = String.Empty;
		
		public DerivedUnitTestingOptionsPanel(UnitTestingOptions options) : base(options)
		{
		}
		
		/// <summary>
		/// Returns the resource name used to create the stream when
		/// initialising the XmlUserControl.
		/// </summary>
		public string SetupFromManifestResourceName {
			get { return setupFromManifestResourceName; }
		}
		
		/// <summary>
		/// Called in CompilingOptionsPanel.LoadPanelContents when
		/// initialising the XmlUserControl.
		/// </summary>
		protected override void SetupFromManifestResource(string resource)
		{
			setupFromManifestResourceName = resource;
			base.SetupFromManifestResource(resource);
		}				
	}
}
