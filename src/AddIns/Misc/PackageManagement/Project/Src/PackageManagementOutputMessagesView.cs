// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementOutputMessagesView : IPackageManagementOutputMessagesView
	{
		public static readonly string CategoryName = "PackageManagement";
		
		IMessageViewCategory messageViewCategory;
		
		public PackageManagementOutputMessagesView(IPackageManagementEvents packageManagementEvents)
			: this(new PackageManagementCompilerMessageView(), packageManagementEvents)
		{
		}
		
		public PackageManagementOutputMessagesView(
			ICompilerMessageView compilerMessageView,
			IPackageManagementEvents packageManagementEvents)
		{
			CreatePackageManagementMessageCategoryIfNoneExists(compilerMessageView);
			packageManagementEvents.PackageOperationMessageLogged += PackageOperationMessageLogged;
		}
		
		void CreatePackageManagementMessageCategoryIfNoneExists(ICompilerMessageView compilerMessageView)
		{
			messageViewCategory = compilerMessageView.GetExisting(CategoryName);
			if (messageViewCategory == null) {
				CreatePackageManagementMessageCategory(compilerMessageView);
			}
		}
		
		void CreatePackageManagementMessageCategory(ICompilerMessageView compilerMessageView)
		{
			messageViewCategory = compilerMessageView.Create(CategoryName, "Packages");
		}
		
		void PackageOperationMessageLogged(object sender, PackageOperationMessageLoggedEventArgs e)
		{
			string formattedMessage = e.Message.ToString();
			messageViewCategory.AppendLine(formattedMessage);			
		}
		
		public void Clear()
		{
			messageViewCategory.Clear();
		}
	}
}
