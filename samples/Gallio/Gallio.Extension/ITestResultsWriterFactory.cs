// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Gallio.Extension
{
	public interface ITestResultsWriterFactory
	{
		ITestResultsWriter Create(string fileName);
	}
}
