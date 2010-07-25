// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace NoGoop.Obj
{
	// Note this class is used only internally in TypeLibrary
	// Used to identify type libraries in the hash tables
	internal class TypeLibKey : IComparable
	{
		internal Guid           _guid;
		
		// The Major.Minor version of a type library, matches
		// the registry key
		
		internal String         _version;
		internal int            _majorVersion;
		internal int            _minorVersion;
		
		// Version can be null, to match on the any version
		internal TypeLibKey(Guid guid,
							String version)
		{
			_guid = guid;
			_version = version;
			try
			{
				if (_version != null)
					GetVersionNumbers();
			}
			catch (Exception ex)
			{
				throw new Exception("Error creating TypeLib key for: " +
									guid + " " + version, ex);
			}
		}
		
		protected void GetVersionNumbers()
		{
			try
			{
				int dot = _version.IndexOf('.');
				if (dot != -1)
				{
					_majorVersion = Convert.
						ToInt16(_version.Substring(0, dot));
					_minorVersion = Convert.
						ToInt16(_version.Substring(dot + 1));
				}
				else
				{
					_majorVersion = 
						Convert.ToInt16(_version);
					_minorVersion = 0;
				}
			}
			catch (Exception ex)
			{
				throw new 
					Exception("Error converting version string for: " 
							  + this, ex);
			}
		}
		
		public override bool Equals(Object other)
		{
			if (!(other is TypeLibKey))
				return false;
			TypeLibKey b = (TypeLibKey)other;
			bool guidEquals = b._guid.Equals(_guid);
			if (!guidEquals)
				return false;
			if (_version != null && b._version != null)
				return _version.Equals(b._version);
			// Our version specified and the other not, no match
			if (_version != null && b._version == null)
				return false;
			// We don't care about version, match any version, or
			// no version is specified for either one, that's a match
			return true;
		}
		
		public override int GetHashCode()
		{
			return _guid.GetHashCode();
		}
		
		public int CompareTo(Object other)
		{
			if (!(other is TypeLibKey))
				return -1;
			TypeLibKey b = (TypeLibKey)other;
			if (b != null)
			{
				int compVal = _guid.CompareTo(b._guid);
				if (compVal != 0)
					return compVal;
				if (_version != null && b._version != null)
					return _version.CompareTo(b._version);
				if ((_version != null && b._version == null) ||
					(_version == null && b._version != null))
					return -1;
				return 0;
			}
			return -1;
		}
		
		public override String ToString()
		{
			return _guid + " " + _version;
		}
	}
}
