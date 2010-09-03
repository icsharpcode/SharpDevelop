// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

/*
 * Currently known logged missing services for the Windows.Forms designer:
 * 
 * System.ComponentModel.ContainerFilterService
 * 		can modify (filter) a component collection
 * 		not needed
 * 
 * System.ComponentModel.TypeDescriptionProvider
 * 		can modify the type information of components at runtime
 * 		probably not needed
 * 
 * System.ComponentModel.Design.DesignerCommandSet
 * System.ComponentModel.Design.DesignerActionService
 * System.ComponentModel.Design.DesignerActionUIService
 * 		these seem to be added automatically when required
 * 		for managing designer verbs and smart tags
 * 
 * System.ComponentModel.Design.Serialization.ComponentCache
 * 		added automatically at some stage during code generation
 * 		by an internal code serializer class
 * 
 * System.Windows.Forms.Design.IMenuEditorService
 * 		required for editing .NET 1.x Menus (?), not supported by SharpDevelop
 * 
 * System.Windows.Forms.Design.ISelectionUIService
 * 		added automatically by ComponentDocumentDesigner or ComponentTray
 * 
 * System.Windows.Forms.Design.IWindowsFormsEditorService
 * 		provided by the PropertyGrid, the designer seems to find this somehow
 * 
 * 
 * System.ComponentModel.Design.IDesignerEventService
 * 		provided by the DesignSurfaceManager
 * 		(this is only logged as missing because the DesignSurfaceManager tries the external service provider first)
 * 
 * 
 * During unloading of the designer some standard services like IDesignerHost are
 * logged as missing as they have already been removed. This is probably expected.
 * 
*/


// Uncomment the following line to log all service requests
//#define WFDESIGN_LOG_SERVICE_REQUESTS


using System;
using System.ComponentModel.Design;

using ICSharpCode.Core;

namespace ICSharpCode.FormsDesigner.Services
{
	public sealed class DefaultServiceContainer : ServiceContainer
	{
		public DefaultServiceContainer()
			: base()
		{
		}
		
		public DefaultServiceContainer(IServiceContainer parent)
			: base(parent)
		{
		}
		
		#if WFDESIGN_LOG_SERVICE_REQUESTS
		public override object GetService(Type serviceType)
		{
			object service = base.GetService(serviceType);
			if (service == null) {
				LoggingService.InfoFormatted("request missing service : {0} from Assembly {1} is not available.", serviceType, serviceType.Assembly.FullName);
			} else {
				LoggingService.DebugFormatted("get service : {0} from Assembly {1}.", serviceType, serviceType.Assembly.FullName);
			}
			return service;
		}
		#endif
	}
}
