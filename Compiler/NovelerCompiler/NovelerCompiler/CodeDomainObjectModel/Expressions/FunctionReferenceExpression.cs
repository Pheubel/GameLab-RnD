namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	internal sealed record FunctionReferenceExpression(Expression TargetObject, string FunctionName, TypeReferenceCollection GenericTypeArguments) : Expression
	{
		public FunctionReferenceExpression(Expression TargetObject, string FunctionName) : this(TargetObject, FunctionName, new()) { }
	};
}
