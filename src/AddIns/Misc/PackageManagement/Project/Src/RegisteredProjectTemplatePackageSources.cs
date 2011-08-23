// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RegisteredProjectTemplatePackageSources
	{
		RegisteredPackageSourceSettings registeredPackageSourceSettings;
		
		public RegisteredProjectTemplatePackageSources()
			: this(new PackageManagementPropertyService(), new SettingsFactory())
		{
		}
		
		public RegisteredProjectTemplatePackageSources(
			IPropertyService propertyService,
			ISettingsFactory settingsFactory)
		{
			GetRegisteredPackageSources(propertyService, settingsFactory);
		}
		
		void GetRegisteredPackageSources(IPropertyService propertyService, ISettingsFactory settingsFactory)
		{
			ISettings settings = CreateSettings(propertyService, settingsFactory);
			PackageSource defaultPackageSource = CreateDefaultPackageSource(propertyService);
			registeredPackageSourceSettings = new RegisteredPackageSourceSettings(settings, defaultPackageSource);
		}
		
		ISettings CreateSettings(IPropertyService propertyService, ISettingsFactory settingsFactory)
		{
			var settingsFileName = new ProjectTemplatePackagesSettingsFileName(propertyService);
			return settingsFactory.CreateSettings(settingsFileName.Directory);
		}
		
		PackageSource CreateDefaultPackageSource(IPropertyService propertyService)
		{
			var defaultPackageSource = new DefaultProjectTemplatePackageSource(propertyService);
			return defaultPackageSource.PackageSource;
		}
		
		public RegisteredPackageSources PackageSources {
			get { return registeredPackageSourceSettings.PackageSources; }
		}
	}
}
