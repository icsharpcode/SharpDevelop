using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.NRefactory;
using ICSharpCode.PythonBinding;

namespace ICSharpCode.CodeConversion
{
    public class PythonHelpers
    {
        public static bool Convert(SupportedLanguage inputLanguage, string ProvidedSource, out string ConvertedSource, out string ErrorMessage)
        {
            NRefactoryToPythonConverter converter = new
                    NRefactoryToPythonConverter(inputLanguage);

            string convertedCode = converter.Convert(ProvidedSource);

            ConvertedSource = convertedCode;
            ErrorMessage = "";

            return true;
        }
    }
}
