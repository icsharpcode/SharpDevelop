#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Irony.CompilerServices {

  public class GrammarErrorException : Exception {
    public GrammarErrorException(string message) : base(message) { }
    public GrammarErrorException(string message, Exception inner) : base(message, inner) { }

  }//class

  public class CompilerException : Exception {
    public CompilerException(string message) : base(message) { }
    public CompilerException(string message, Exception inner) : base(message, inner) { }

  }//class

}//namespace
