// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

using System.Diagnostics;

namespace HexEditor.View
{
	public class HexEditDisplayBinding : IDisplayBinding
	{
		string[] supportedBinaryFileNameExtensions;
		
		public HexEditDisplayBinding()
		{
		}
		
		bool IsBinaryFileName(string fileName)
		{
			if (fileName != null) {
				string extension = Path.GetExtension(fileName);
				foreach (string supportedExtension in GetSupportedBinaryFileExtensions()) {
					if (String.Compare(supportedExtension, extension, StringComparison.InvariantCultureIgnoreCase) == 0) {
						return true;
					}
				}
			}
			return false;
		}
		
		string[] GetSupportedBinaryFileExtensions()
		{
			if (supportedBinaryFileNameExtensions == null) {
				string extensionList = (string)AddInTree.BuildItem("/AddIns/HexEditor/Editor/SupportedFileExtensions", null);
				if (extensionList != null) {
					supportedBinaryFileNameExtensions = extensionList.Split(';');
				}
			}
			return supportedBinaryFileNameExtensions;
		}
		
		bool IsBinary(string fileName)
		{
			try {
				if (!File.Exists(fileName)) return false;
				
				BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
				byte[] data = reader.ReadBytes(1024);
				reader.Close();
				
				for (int i = 0; i < data.Length; i++) {
					if ((data[i] != 0xA) && (data[i] != 0xD) && (data[i] != 0x9)) {
						if (data[i] < 0x20) return true;
					}
				}
			} catch (IOException ex) {
				MessageService.ShowError(ex, ex.Message);
			} catch (Exception ex) {
				Debug.Print(ex.ToString());
			}
			return false;
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return IsBinaryFileName(fileName);
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new HexEditView(file);
		}
	}
}
