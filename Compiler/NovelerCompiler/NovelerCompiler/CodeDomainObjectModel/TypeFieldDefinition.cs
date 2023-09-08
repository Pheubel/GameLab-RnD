using Noveler.Compiler.CodeDomainObjectModel.Expressions;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal sealed record TypeFieldDefinition(TypeReference FieldType, string FieldName, int OffsetInBytes)
    {
        public Expression? InitializationExpression { get; }
        public bool HasInitializer => InitializationExpression != null;

        public TypeFieldDefinition(TypeReference FieldType, string FieldName, int OffsetInBytes, Expression InitializationExpression) :
            this(FieldType, FieldName, OffsetInBytes)
        {
            this.InitializationExpression = InitializationExpression;
        }
    };
}
