// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.FormsDesigner
{
	public interface IDesignerLoader
	{
		CodeCompileUnit Parse();
		void Write(CodeCompileUnit unit);
		CodeDomLocalizationModel GetLocalizationModel();
		bool IsReloadNeeded(bool value);
	}
}
