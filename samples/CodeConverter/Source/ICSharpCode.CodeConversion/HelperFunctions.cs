using System;
using System.Collections.Generic;

// Conversion general
using ICSharpCode.NRefactory.PrettyPrinter;


namespace ICSharpCode.CodeConversion
{
    public class CodeConversionHelpers
    {
        private CodeConversionHelpers() { }

        public static string GetNRefactoryVersion()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(VBNetOutputVisitor));
            Version v = asm.GetName().Version;

            string s = string.Format("{0:0}.{1:0}.{2:0}.{3:0}",
                    v.Major, v.Minor, v.Build, v.Revision);

            return s;
        }

        public static IConvertCode InferConverter(string typeOfConversion)
        {
            Dictionary<string, Type> converters = new Dictionary<string, Type>()
            {
                { "cs2boo", typeof(ConvertCSharpToBoo) },
                { "vbnet2boo", typeof(ConvertVbNetToBoo) },
                { "cs2vbnet", typeof(ConvertCSharpSnippetToVbNet) },
                { "vbnet2cs", typeof(ConvertVbNetSnippetToCSharp) },
                { "vbnet2ruby", typeof(ConvertVbNetToRuby) },
                { "vbnet2python", typeof(ConvertVbNetToPython) },
                { "cs2ruby", typeof(ConvertCSharpToRuby) },
                { "cs2python", typeof(ConvertCSharpToPython) }
            };

            if (converters.ContainsKey(typeOfConversion))
            {
                Type t = converters[typeOfConversion];
                object o = Activator.CreateInstance(t, null);
                
                return (o as IConvertCode);
            }

            return null;
        }

        public static bool ConvertSnippet(string TypeOfConversion, string SourceCode, out string ConvertedCode, out string ErrorMessage)
        {
            ErrorMessage = ConvertedCode = "";
            string convertedSource = "", errorMessage = "";
            bool bSuccessfulConversion = false;

            IConvertCode currentConverter = InferConverter(TypeOfConversion);
            if (null == currentConverter) return false;

            try
            {
                bSuccessfulConversion = currentConverter.Convert(SourceCode,
                            out convertedSource,
                            out errorMessage);
            }
            catch (Exception ex)
            {
                bSuccessfulConversion = false;
                errorMessage = "Exception occured: " + ex.ToString() + "\r\n\r\nError Message:" + errorMessage;
            }

            if (bSuccessfulConversion)
            {
                ConvertedCode = convertedSource;
            }
            else
            {
                ErrorMessage = errorMessage;
            }

            return bSuccessfulConversion;
        }
    }
}
