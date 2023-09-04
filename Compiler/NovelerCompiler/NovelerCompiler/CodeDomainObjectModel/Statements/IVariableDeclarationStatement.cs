namespace Noveler.Compiler.CodeDomainObjectModel.Statements
{
    interface IVariableDeclarationStatement
	{
		string VariableName { get; }
		TypeReference TypeReference { get; }
		bool HasInitializationExpression { get; }
	}
}
