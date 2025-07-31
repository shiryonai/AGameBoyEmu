namespace AGameBoyEmu.SoC
{
    public enum Register8
    {
        A, B, C, D, E, H, L
    }

    public enum InstructionType
    {
        ADD
        // TODO: Add other instruction types as needed
    }

    public struct Instruction
    {
        public InstructionType Type;
        public Register8 Target;

        public Instruction(InstructionType type, Register8 target)
        {
            Type = type;
            Target = target;
        }
    }
}