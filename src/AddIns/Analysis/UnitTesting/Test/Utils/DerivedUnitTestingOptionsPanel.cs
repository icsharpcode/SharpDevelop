// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
