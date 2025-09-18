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
        RLA,
        RRA,
        RLCA,
        RRCA,
        CPL,
        BIT,
        SET,
        RES,
        SRL,
        RL,
        RR,
        
        // TODO: add more instruction types
    }

    public struct Instruction
    {
        public InstructionType Type;
        public Register8 Target;
        public int Bit; // Bit stuff unsure yet

        public Instruction(InstructionType type, Register8 target)
        {
            Type = type;
            Target = target;
            Bit = 0; 
        }

        public Instruction(InstructionType type, Register8 target, int bit)
        {
            Type = type;
            Target = target;
            Bit = bit;
        }
    }
}