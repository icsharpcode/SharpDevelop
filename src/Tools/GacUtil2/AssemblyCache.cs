//	GacUtil2
//	Copyright (c) 2004, Christoph Wille
//	All rights reserved.
//	
//	Redistribution and use in source and binary forms, with or without modification, are 
//	permitted provided that the following conditions are met:
//	
//	- Redistributions of source code must retain the above copyright notice, this list 
//	  of conditions and the following disclaimer.
//	
//	- Redistributions in binary form must reproduce the above copyright notice, this list
//	  of conditions and the following disclaimer in the documentation and/or other materials 
//	  provided with the distribution.
//	
//	- Neither the name of the <ORGANIZATION> nor the names of its contributors may be used to 
//	  endorse or promote products derived from this software without specific prior written 
//	  permission.
//	
//	THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS 
//	OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
//	AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
//	CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
//	DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
//	DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
//	IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
//	OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// http://support.microsoft.com/default.aspx?scid=KB;EN-US;Q317540&
// this was used to make changes to FusionNative.cs where I felt it was necessary

using System;
using System.Text;
using MSjogren.Fusion.Native;
using System.Security.Permissions;
using System.Reflection;

namespace GacUtil2
{
	public enum UninstallDisposition
	{
		UnInstalled, // - The assembly files have been removed from the GAC.
		InUse, // - An application is using the assembly. This value is returned on Microsoft Windows 95 and Microsoft Windows 98.
		AlreadyUnInstalled, // - The assembly does not exist in the GAC.
		DeletePending, // - Not used.
		HasInstallReferences, // - The assembly has not been removed from the GAC because another application reference exists.
		ReferenceNotFound // - The reference that is specified in pRefData is not found in the GAC.
	}
	
	public class AssemblyCache
	{
		private AssemblyCache()
		{
		}
		
		public static void Install(string AssemblyPath)
		{
			try
			{
			    SecurityPermission unmanagedPermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
			    unmanagedPermission.Demand();
				
			    IAssemblyCache theCache = null;
				
				// NOTE: we are dealing with HRESULT's here
			    int retVal = FusionApi.CreateAssemblyCache(ref theCache, 0);
			    if (0 == retVal)
			    {
			          retVal = theCache.InstallAssembly(0, AssemblyPath, null);
			    }
			    
			    if (0 == retVal)
			    {
			          return;
			    }
			    throw new ApplicationException("Installation in GAC failed for assembly: " + AssemblyPath);
			}
			catch (Exception e)
			{
			    throw e;
			}
		}

		public static void Uninstall(string AssemblyPath)
		{
			try
			{
			    SecurityPermission unmanagedPermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
			    unmanagedPermission.Demand();
				
				AssemblyName an = AssemblyName.GetAssemblyName(AssemblyPath);
				StringBuilder stb = new StringBuilder();
				stb.Append(an.Name);
				
				/*
				stb.Append(", Version=");
				stb.Append(an.Version.ToString());
				
				byte[] pt = an.GetPublicKeyToken();
				stb.Append(", PublicKeyToken=");
				for (int i=0;i<pt.GetLength(0);i++)
					stb.AppendFormat("{0:x}", pt[i]); 
				*/
				
				string VersionedName = stb.ToString();

				Console.WriteLine(VersionedName);
				
			    IAssemblyCache theCache = null;
			    int retVal = FusionApi.CreateAssemblyCache(ref theCache, 0);
				uint iDisposition = 0;
				
			    if (0 == retVal)
			    {
                  	retVal = theCache.UninstallAssembly(0, VersionedName, null, out iDisposition);
			    }
			    
			    if (0 == retVal)
			    {
			    	// simply return on success
					return;
			    }
			    
				switch((UninstallDisposition)iDisposition)
				{
					case UninstallDisposition.UnInstalled:
						// Assembly was removed from GAC
						break;
					case UninstallDisposition.InUse:
						throw new ApplicationException("An application is using " + AssemblyPath + " so it could not be uninstalled.");
					case UninstallDisposition.AlreadyUnInstalled:
						// Assembly is not in the assembly cache
						throw new ApplicationException(AssemblyPath + " is not in the GAC.");
					case UninstallDisposition.DeletePending:
						//Not used.
						break;
					case UninstallDisposition.HasInstallReferences:
						throw new ApplicationException(AssemblyPath + " was not uninstalled from the GAC because another reference exists to it.");
					case UninstallDisposition.ReferenceNotFound:
						//Problem where a reference doesn't exist to the pointer.
						break;
				}

			    throw new ApplicationException("Removal from GAC failed for assembly: " + AssemblyPath);
			}
			catch (Exception e)
			{
			    throw e;
			}
		}
 	}
}
