// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ClientOptions
	{
		public ClientOptions()
		{
			EnableDataBinding = true;
			GenerateSerializableTypes = true;
			Serializer = "Auto";
			UseSerializerForFaults = true;
			ReferenceAllAssemblies = true;
		}
		
		public bool GenerateAsynchronousMethods { get; set; }
		public bool EnableDataBinding { get; set; }
		public bool ImportXmlTypes { get; set; }
		public bool GenerateInternalTypes { get; set; }
		public bool GenerateMessageContracts { get; set; }
		public bool GenerateSerializableTypes { get; set; }
		public string Serializer { get; set; }
		public bool UseSerializerForFaults { get; set; }
		public bool ReferenceAllAssemblies { get; set; }
	}
}
