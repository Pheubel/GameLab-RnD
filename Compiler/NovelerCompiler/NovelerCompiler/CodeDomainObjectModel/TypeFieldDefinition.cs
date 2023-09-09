using Noveler.Compiler.CodeDomainObjectModel.Expressions;
using System.Text;
using System.Xml.Linq;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record TypeFieldDefinition(string FieldName, TypeDefinition FieldType, int OffsetInBytes, Expression? InitializationExpression)
    {
        public bool HasInitializer => InitializationExpression != null;

        public string GetFullyQualifiedName()
        {
            StringBuilder sb = new();
            GetFullyQualifiedName(sb);

            return sb.ToString();
        }

        public void GetFullyQualifiedName(StringBuilder stringBuilder)
        {
            // this is tail recursable, but is this the best it can be?
            stringBuilder.Insert(0, $"::{FieldName}");
            FieldType.GetFullyQualifiedName(stringBuilder);
        }
    };
}
