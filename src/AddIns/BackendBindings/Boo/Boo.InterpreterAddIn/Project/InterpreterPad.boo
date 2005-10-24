// <file>
//     <copyright see="prj:///doc/copyright.txt">2004 Rodrigo B. de Oliveira; 2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

namespace Boo.InterpreterAddIn

import System
import System.Windows.Forms
import ICSharpCode.Core
import ICSharpCode.SharpDevelop.Gui

class InterpreterPad(AbstractPadContent):
"""Description of InterpreterPad"""
	ctl = InteractiveInterpreterControl()
	
	Control as Control:
		get:
			return ctl

