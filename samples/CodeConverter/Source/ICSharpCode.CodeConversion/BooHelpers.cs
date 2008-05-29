using System.IO;
using System.Collections.Generic;
using System.Text;

// Boo conversion
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using NRefactoryToBooConverter;

namespace ICSharpCode.CodeConversion
{
    public class BooHelpers
    {
        private BooHelpers() { }

        public static ConverterSettings ApplySettings(string fileName, CompilerErrorCollection errors, CompilerWarningCollection warnings)
        {
            ConverterSettings settings = new ConverterSettings(fileName, errors, warnings);
            settings.SimplifyTypeNames = true;
            return settings;
        }

        public static string CreateBooCode(CompilerErrorCollection errors,
                CompilerWarningCollection warnings,
                Module module,
                IList<ICSharpCode.NRefactory.ISpecial> specials)
        {
            using (StringWriter w = new StringWriter())
            {
                foreach (CompilerError error in errors)
                {
                    w.WriteLine("ERROR: " + error.ToString());
                }
                if (errors.Count > 0)
                    w.WriteLine();
                foreach (CompilerWarning warning in warnings)
                {
                    w.WriteLine("# WARNING: " + warning.ToString());
                }

                if (warnings.Count > 0)
                    w.WriteLine();

                BooPrinterVisitorWithComments printer = new BooPrinterVisitorWithComments(specials, w);
                printer.OnModule(module);
                printer.Finish();
                return w.ToString();
            }
        }

        public static bool ConvertToBoo(string fileName,
                    string ProvidedSource,
                    out string ConvertedSource,
                    out string ErrorMessage)
        {
            ConvertedSource = ErrorMessage = "";

            CompilerErrorCollection errors = new CompilerErrorCollection();
            CompilerWarningCollection warnings = new CompilerWarningCollection();
            Module module;
            IList<ICSharpCode.NRefactory.ISpecial> specials;
            CompileUnit compileUnit = new CompileUnit();

            using (StringReader r = new StringReader(ProvidedSource))
            {
                // modified: removed fileName guessing
                module = Parser.ParseModule(compileUnit, r, BooHelpers.ApplySettings(fileName, errors, warnings), out specials);
            }

            if (module == null)
            {
                StringBuilder errorBuilder = new StringBuilder();
                foreach (CompilerError error in errors)
                {
                    errorBuilder.AppendLine(error.ToString());
                }
                if (warnings.Count > 0)
                {
                    foreach (CompilerWarning warning in warnings)
                    {
                        errorBuilder.AppendLine(warning.ToString());
                    }
                }
                ErrorMessage = errorBuilder.ToString();
                return false;
            }
            else
            {
                ConvertedSource = BooHelpers.CreateBooCode(errors, warnings, module, specials);
            }

            return true;
        }
    }
}
