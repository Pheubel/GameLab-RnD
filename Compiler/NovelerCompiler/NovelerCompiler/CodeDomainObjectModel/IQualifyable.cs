using System.Text;

namespace Noveler.Compiler.CodeDomainObjectModel
{
    internal interface IQualifyable
    {
        string GetFullyQualifiedName();
        void GetFullyQualifiedName(StringBuilder stringBuilder);
        void GetFullyQualifiedName(StringBuilder stringBuilder, int index);
    }
}