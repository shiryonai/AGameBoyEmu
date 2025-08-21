namespace AGameBoyEmu.SoC
{
    public enum Register8
    {
        A, B, C, D, E, H, L
    }

    public enum InstructionType
    {
        ADD,
        ADDHL,
        ADC,
        SUB,
        SBC,
        AND,
        OR,
        XOR,
        CP,
        INC,
        DEC,
        CCF,
        SCF,
        RRA,
                // TODO: add more instruction types
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