// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Runtime.InteropServices;
using Microsoft.Win32;

using CORDBLib_1_0;

using NoGoop.Win32;
using NoGoop.Util;

namespace NoGoop.Debug
{
	public class EnumHolder : IEnumerator
	{
		protected ICorDebugAppDomainEnum    _debugEnum;
		protected int                       _position;
		protected uint                      _count;
		protected ICorDebugAppDomain        _current;

		public EnumHolder(ICorDebugAppDomainEnum debugEnum)
		{
			_debugEnum = debugEnum;
			_count = _debugEnum.GetCount();
		}

		public Object Current {
			get {
				return _current;
			}
		}

		public bool MoveNext()
		{
			uint gotNum = _debugEnum.Next(1, out _current);
			_position += 1;
			if (_position > _count)
				return false;
			return true;
		}

		public void Reset()
		{
			_debugEnum.Reset();
			_position = -1;
		}
	}
}
