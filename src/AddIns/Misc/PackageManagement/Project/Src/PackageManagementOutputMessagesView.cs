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
		
		public PackageManagementOutputMessagesView()
			: this(new PackageManagementCompilerMessageView())
		{
		}
		
		public PackageManagementOutputMessagesView(ICompilerMessageView compilerMessageView)
		{
			CreatePackageManagementMessageCategoryIfNoneExists(compilerMessageView);
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
		
		public void Clear()
		{
			messageViewCategory.Clear();
		}
		
		public void Log(MessageLevel level, string message, params object[] args)
		{
			string formattedMessage = String.Format(message, args);
			messageViewCategory.AppendLine(formattedMessage);
		}
	}
}
