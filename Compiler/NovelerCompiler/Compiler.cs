using System.Text;

namespace Noveler
{
    public class Compiler
    {
        public static Result Compile(string script, out string result)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // as example reverse the input
            foreach (var c in script.Reverse())
            {
                stringBuilder.Append(c);
            }

            result = stringBuilder.ToString();
            return Result.Succes;
        }

        /// <summary>
        /// A list of possible result codes from compiling a script.
        /// </summary>
        public enum Result
        {
            Succes
        }
    }
}