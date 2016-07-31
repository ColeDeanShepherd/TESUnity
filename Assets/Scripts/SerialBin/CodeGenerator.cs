using System.Text;

namespace SerialBin
{
	using AST;

	public class CodeGenerator
	{
		public string GenerateCode(FormatSpecification formatSpecification)
		{
			codeBuilder = new StringBuilder();
			codeBuilder.Append("asdf");

			return codeBuilder.ToString();
		}

		private StringBuilder codeBuilder;
	}
}