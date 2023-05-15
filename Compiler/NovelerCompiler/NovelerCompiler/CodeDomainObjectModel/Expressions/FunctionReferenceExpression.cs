namespace Noveler.Compiler.CodeDomainObjectModel.Expressions
{
	internal sealed record FunctionReferenceExpression(Expression TargetObject, string FunctionName, TypeReferenceCollection GenericTypeArguments) : Expression
	{
		public FunctionReferenceExpression(Expression TargetObject, string FunctionName) : this(TargetObject, FunctionName, new()) { }

		public override IReadOnlyList<DomainObject> GetChildren()
		{
			var children = new List<DomainObject>
			{
				TargetObject,
				GenericTypeArguments
			};

			return children;
		}
	}
}
