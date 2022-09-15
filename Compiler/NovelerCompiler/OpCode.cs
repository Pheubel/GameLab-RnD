namespace Noveler.Compiler
{
    public enum OpCode : byte
    {
        // basic numeric operations

        Int32Add,
        Int32Subtract,
        Int32Multiply,
        Int32DivRem,
        Int32Negate,

        Int64Add,
        Int64Subtract,
        Int64Multiply,
        Int64DivRem,
        Int64Negate,

        Float32Add,
        Float32Subtract,
        Float32Multiply,
        Float32Divide,

        Float64Add,
        Float64Subtract,
        Float64Multiply,
        Float64Divide,

        // bitwise operations

        Int32And,
        Int32Or,
        Int32Xor,
        Int32Not,

        Int32LeftShift,
        Int32RightShift,

        Int64And,
        Int64Or,
        Int64Xor,
        Int64Not,

        Int64LeftShift,
        Int64RightShift,

        // casting

        Cast32To64,
        Cast64To32,

        // memory management

        Load32,
        Load64,
        Load32ToStack,
        Load64ToStack,

        Load32Constant,
        Load64Constant,
        Load32ConstantToStack,
        Load64ConstantToStack,

        Store32,
        Store64,
        Store32FromStack,
        Store64FromStack,

        Push32,
        Push64,

        Pop32,
        Pop64,

        Move32,
        Move64,

        // logic operations

        // interop functions

        /// <summary> 
        /// Calls a function from outside the Noveler runtime using the stack as argument data.
        /// </summary>
        /// <remarks>
        /// Register one detemines how many bytes are passed, register two determines how many bytes are returned.
        /// </remarks>
        CallExternalFunction,



        // intrinsics
    }
}