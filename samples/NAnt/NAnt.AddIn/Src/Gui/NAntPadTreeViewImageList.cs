// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.NAnt.Gui
{
	/// <summary>
	/// Represents the images that are used in the 
	/// <see cref="NAntPadTreeView"/>.
	/// </summary>
	public class NAntPadTreeViewImageList
	{
		/// <summary>
		/// The NAnt build file image index.
		/// </summary>
		public static int BuildFileImage = 0;
		
		/// <summary>
		/// The NAnt build target image index.
		/// </summary>
		public static int TargetImage = 1;
		
		/// <summary>
		/// The NAnt default build target image index.
		/// </summary>
		public static int DefaultTargetImage = 2;
		
		/// <summary>
		/// The error icon displayed when the build file has errors.
		/// </summary>
		public static int BuildFileErrorImage = 3;
		
		/// <summary>
		/// The error icon displayed as the first target when the build file has errors.
		/// </summary>
		public static int TargetErrorImage = 4;
		
		NAntPadTreeViewImageList()
		{
		}
		
		/// <summary>
		/// Creates an image list to be used in the 
		/// <see cref="NAntPadTreeView"/>.</summary>
		public static ImageList GetImageList()
		{
			ImageList imageList = new ImageList();
			imageList.Images.Add(IconService.GetBitmap("NAnt.AddIn.Icons.16x16.BuildFile"));
			imageList.Images.Add(IconService.GetBitmap("NAnt.AddIn.Icons.16x16.BuildTarget"));
			imageList.Images.Add(IconService.GetBitmap("NAnt.AddIn.Icons.16x16.DefaultBuildTarget"));
			imageList.Images.Add(IconService.GetBitmap("NAnt.AddIn.Icons.16x16.BuildFileError"));
			imageList.Images.Add(IconService.GetBitmap("NAnt.AddIn.Icons.16x16.BuildTargetError"));
			
			return imageList;
		}
	}
}
