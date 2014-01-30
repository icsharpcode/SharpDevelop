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

using System;
using ICSharpCode.AddInManager2.Model;

namespace ICSharpCode.AddInManager2
{
    /// <summary>
    /// Container for public services of AddInManager AddIn.
    /// </summary>
    public class AddInManagerServices
    {
    	private class AddInManagerServiceContainer : IAddInManagerServices
    	{
			public IAddInManagerEvents Events
			{
				get;
				set;
			}
    		
			public IPackageRepositories Repositories
			{
				get;
				set;
			}
    		
			public IAddInSetup Setup
			{
				get;
				set;
			}
    		
			public INuGetPackageManager NuGet
			{
				get;
				set;
			}
    		
			public IAddInManagerSettings Settings
			{
				get;
				set;
			}
			
			public ISDAddInManagement SDAddInManagement
			{
				get;
				set;
			}
    	}
    	
    	private static AddInManagerServiceContainer _container;
    	
        static AddInManagerServices()
		{
        	_container = new AddInManagerServiceContainer();
        	_container.Settings = new AddInManagerSettings();
			_container.Events = new AddInManagerEvents();
			_container.Repositories = new PackageRepositories(_container.Events, _container.Settings);
			_container.SDAddInManagement = new SDAddInManagement();
			_container.NuGet = new NuGetPackageManager(_container.Repositories, _container.Events, _container.SDAddInManagement);
			_container.Setup = new AddInSetup(_container.Events, _container.NuGet, _container.SDAddInManagement);
		}

        public static IAddInManagerEvents Events
        {
            get
            {
                return _container.Events;
            }
        }

        public static IPackageRepositories Repositories
        {
            get
            {
                return _container.Repositories;
            }
        }

        public static IAddInSetup Setup
        {
            get
            {
                return _container.Setup;
            }
        }

        public static INuGetPackageManager NuGet
        {
            get
            {
                return _container.NuGet;
            }
        }
        
        public static IAddInManagerSettings Settings
        {
            get
            {
                return _container.Settings;
            }
        }
        
        public static IAddInManagerServices Services
        {
        	get
        	{
        		return _container;
        	}
        }
    }
}
