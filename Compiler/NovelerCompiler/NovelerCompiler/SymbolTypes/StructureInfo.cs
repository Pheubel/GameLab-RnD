using Microsoft.CodeAnalysis;

namespace Noveler.Compiler.SymbolTypes
{
	public delegate void OnTypeDefined();

	internal sealed record StructureInfo(string Name, Optional<FieldInfo[]> FieldInfo)
	{
		public int SizeInBytes { get; set; }
		public bool IsFullyDefined { get; set; }
		public event OnTypeDefined? HandleTypeDefined;

		public StructureInfo(string Name, int SizeInBytes) :
			this(Name, default(Optional<FieldInfo[]>))
		{
			this.SizeInBytes = SizeInBytes;
			IsFullyDefined = true;
		}
	}
}
