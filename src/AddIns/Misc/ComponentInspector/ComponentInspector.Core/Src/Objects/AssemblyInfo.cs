// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Reflection;
using NoGoop.Util;

namespace NoGoop.Obj
{
	// NOTE - this class is used only internally by TypeLibrary
	// Tracks name information associated with assemblies that 
	// belong to this type library
	internal class AssemblyInfo
	{
		internal String             _name;
		
		// The unqualified file name - used to save the assembly
		internal String             _fileName;
		internal String             _url;
		
		// A converted assembly has a single namespace that
		// normally corresponds to the name of the typelib.  Sometimes,
		// in the case of a primary interop assy this is different.
		// We use this namespace name to look up any classes or
		// interfaces we need to find in this assembly
		internal String             _nameSpaceName;
		
		internal AssemblyInfo()
		{
		}
		
		internal void SetName(String dir,
							  String baseName)
		{
			_name = baseName;
			_fileName = _name + ".dll";
			_url = "file:///" 
				+ dir.Replace("\\", "/")
				+ "/" + _fileName;
		}
		
		internal void SetNamespace(Assembly assy)
		{
			// This takes forever in the case of some types,
			// the time is to load the module.  And when this
			// assembly is used, you will have to pay that time
			// at some point.
				
			// When I tested this, the time was spent on the
			// assy.GetModules() call.  But this assembly (MSHTML)
			// should only have one module.  I did not look into making
			// that faster.
			// FIXME - that's something to continue to research
				
			TraceUtil.WriteLineInfo
				(this, DateTime.Now + " AssyInfo - GetTypes() start");
			Type[] types = assy.GetTypes();
			TraceUtil.WriteLineInfo
				(this, DateTime.Now + " AssyInfo - GetTypes() end");
			if (types.Length > 0)
				_nameSpaceName = types[0].Namespace;
			else
				_nameSpaceName = "<NoTypesFound>";
			TraceUtil.WriteLineInfo(this, "Assembly " + assy 
									+ " ns: " + _nameSpaceName);
		}
	}
}
