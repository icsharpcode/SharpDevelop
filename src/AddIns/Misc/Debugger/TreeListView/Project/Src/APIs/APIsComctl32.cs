using System;
using System.Runtime.InteropServices;

namespace System.Runtime.InteropServices.APIs
{
	public class APIsComctl32
	{
		#region GetMajorVersion
		public static int GetMajorVersion()
		{
			APIsStructs.DLLVERSIONINFO2 pdvi = new APIsStructs.DLLVERSIONINFO2();
			pdvi.info1.cbSize = Marshal.SizeOf(typeof(APIsStructs.DLLVERSIONINFO2));
			DllGetVersion(ref pdvi);
			return pdvi.info1.dwMajorVersion;
		}
		#endregion
		#region DllGetVersion
		[DllImport("Comctl32.dll", CharSet=CharSet.Auto)]
		static private extern int DllGetVersion(
			ref APIsStructs.DLLVERSIONINFO2 pdvi);
		#endregion
	}
}
