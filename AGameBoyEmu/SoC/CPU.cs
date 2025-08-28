using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGameBoyEmu.SoC
{
    public class CPU
    {
        // CPU Registers
        public byte A, B, C, D, E, F, H, L;
        public ushort SP; // Stack Pointer
        public ushort PC; // Program Counter

        // Combined registers

        public ushort BC
        {
            get => (ushort)((B << 8) | C);
            set
            {
                B = (byte)(value >> 8);
                C = (byte)(value & 0xFF);
            }
        }
        public ushort DE
        {
            get => (ushort)((D << 8) | E);
            set
            {
                D = (byte)(value >> 8);
                E = (byte)(value & 0xFF);
            }
        }
        public ushort HL
        {
            get => (ushort)((H << 8) | L);
            set
            {
                H = (byte)(value >> 8);
                L = (byte)(value & 0xFF);
            }
        }

        public ushort AF
        {
            get => (ushort)((A << 8) | F);
            set
            {
                A = (byte)(value >> 8);
                F = Flags.FromByte((byte)(value & 0xFF)).ToByte();
            }
        }

        // Flags Register
        public struct Flags
        {
            public bool zero;
            public bool subtract;
            public bool halfCarry;
            public bool carry;

            // Convert Flags register to byte 
            public byte ToByte()
            {
                byte value = 0;
                if (zero) value |= (1 << 7);
                if (subtract) value |= (1 << 6);
                if (halfCarry) value |= (1 << 5);
                if (carry) value |= (1 << 4);
                return value;
            }

            // Create Flags register from previous byte 
            public static Flags FromByte(byte value)
            {
                return new Flags
                {
                    zero = (value & (1 << 7)) != 0,
                    subtract = (value & (1 << 6)) != 0,
                    halfCarry = (value & (1 << 5)) != 0,
                    carry = (value & (1 << 4)) != 0
                };
            }
        }

        public void Execute(Instruction instruction)
        {
            switch (instruction.Type)
            {
                case InstructionType.ADD:
                    byte addVal = GetRegisterValue(instruction.Target);
                    ADD(addVal);
                    break;
                case InstructionType.ADDHL:
                    ushort addhlVal = GetRegisterValue(instruction.Target);
                    ADDHL(addhlVal);
                    break;
                case InstructionType.ADC:
                    byte adcVal = GetRegisterValue(instruction.Target);
                    ADC(adcVal);
                    break;
                case InstructionType.SUB:
                    byte subVal = GetRegisterValue(instruction.Target);
                    SUB(subVal);
                    break;
                case InstructionType.SBC:
                    byte sbcVal = GetRegisterValue(instruction.Target);
                    SBC(sbcVal);
                    break;
                case InstructionType.AND:
                    byte andVal = GetRegisterValue(instruction.Target);
                    AND(andVal);
                    break;
                case InstructionType.OR:
                    byte orVal = GetRegisterValue(instruction.Target);
                    OR(orVal);
                    break;
                case InstructionType.XOR:
                    byte xorVal = GetRegisterValue(instruction.Target);
                    XOR(xorVal);
                    break;
                case InstructionType.CP:
                    byte cpVal = GetRegisterValue(instruction.Target);
                    CP(cpVal);
                    break;
                case InstructionType.INC:
                    byte incVal = GetRegisterValue(instruction.Target);
                    INC(incVal);
                    break;
                case InstructionType.DEC:
                    byte decVal = GetRegisterValue(instruction.Target);
                    DEC(decVal);
                    break;
                case InstructionType.CCF:
                    CCF();
                    break;
                case InstructionType.SCF:
                    SCF();
                    break;
                case InstructionType.RLA:
                    RLA();
                    break;
                case InstructionType.RRA:
                    RRA();
                    break;
                case InstructionType.RLCA:
                    RLCA();
                    break;
                case InstructionType.RRCA:
                    RRCA();
                    break;
                case InstructionType.CPL:
                    CPL();
                    break;
                    // To do: add more cases for other instructions
            }
        }

        private byte GetRegisterValue(Register8 reg)
        {
            return reg switch
            {
                Register8.A => A,
                Register8.B => B,
                Register8.C => C,
                Register8.D => D,
                Register8.E => E,
                Register8.H => H,
                Register8.L => L,
                _ => 0
            };
        }

        private void ADD(byte value)
        {
            int result = A + value;

            // Set flags
            Flags flags = new Flags();
            flags.zero = (byte)result == 0;
            flags.subtract = false;
            flags.carry = result > 0xFF;
            flags.halfCarry = ((A & 0xF) + (value & 0xF)) > 0xF;

            F = flags.ToByte(); // Update F register
            A = (byte)result;
        }

        // Add to HL
        private void ADDHL(ushort value)
        {
            int result = HL + value;

            // Start from current flags to preserve Z
            Flags flags = Flags.FromByte(F);
            flags.subtract = false;
            flags.carry = result > 0xFFFF;
            flags.halfCarry = ((HL & 0xFFF) + (value & 0xFFF)) > 0xFFF;

            F = flags.ToByte();
            HL = (ushort)result;
        }

        // Add with Carry
        private void ADC(byte value)
        {
            int result = A + value + (F & 0x10); // Add carry flag if set

            Flags flags = new Flags();
            flags.zero = (byte)result == 0;
            flags.subtract = false;
            flags.carry = result > 0xFF;
            flags.halfCarry = ((A & 0xF) + (value & 0xF) + ((F & 0x10) >> 4)) > 0xF;

            F = flags.ToByte();
            A = (byte)result;
        }

        private void SUB(byte value)
        {
            int result = A - value;

            Flags flags = new Flags();
            flags.zero = (byte)result == 0;
            flags.subtract = true;
            flags.carry = result < 0;
            flags.halfCarry = ((A & 0xF) - (value & 0xF)) < 0;

            F = flags.ToByte();
            A = (byte)result;
        }

        // Subtract with Carry
        private void SBC(byte value)
        {
            int result = A - value - ((F & 0x10) >> 4);

            Flags flags = new Flags();
            flags.zero = (byte)result == 0;
            flags.subtract = true;
            flags.carry = result < 0;
            flags.halfCarry = ((A & 0xF) - (value & 0xF) - ((F & 0x10) >> 4)) < 0;

            F = flags.ToByte();
            A = (byte)result;
        }

        private void AND(byte value)
        {
            int result = A & value;

            Flags flags = new Flags();
            flags.zero = (byte)result == 0;
            flags.subtract = false;
            flags.carry = false;
            flags.halfCarry = true;

            F = flags.ToByte();
            A = (byte)result;
        }
        private void OR(byte value)
        {
            int result = A | value;

            Flags flags = new Flags();
            flags.zero = (byte)result == 0;
            flags.subtract = false;
            flags.carry = false;
            flags.halfCarry = false;

            F = flags.ToByte();
            A = (byte)result;
        }

        private void XOR(byte value)
        {
            int result = A ^ value;

            Flags flags = new Flags();
            flags.zero = (byte)result == 0;
            flags.subtract = false;
            flags.carry = false;
            flags.halfCarry = false;

            F = flags.ToByte();
            A = (byte)result;
        }

        // Compare
        private void CP(byte value)
        {
            int result = A - value;

            Flags flags = new Flags();
            flags.zero = (byte)result == 0;
            flags.subtract = true;
            flags.carry = result < 0;
            flags.halfCarry = ((A & 0xF) - (value & 0xF)) < 0;

            F = flags.ToByte();
        }

        // Increment
        private void INC(byte value)
        {
            int result = value + 1;

            // Preserve the carry flag
            Flags flags = Flags.FromByte(F);
            flags.zero = (byte)result == 0;
            flags.subtract = false;
            flags.halfCarry = (value & 0xF) + 1 > 0xF;

            F = flags.ToByte();
        }

        private void DEC(byte value)
        {
            int result = value - 1;

            Flags flags = Flags.FromByte(F);
            flags.zero = (byte)result == 0;
            flags.subtract = true;
            flags.halfCarry = (value & 0xF) - 1 < 0;

            F = flags.ToByte();
        }

        // Toggle Carry Flag
        private void CCF()
        {
            Flags flags = Flags.FromByte(F);
            flags.carry = !flags.carry;
            flags.subtract = false;
            flags.halfCarry = false;

            F = flags.ToByte();
        }

        // Set Carry Flag to True
        private void SCF()
        {
            Flags flags = Flags.FromByte(F);
            flags.carry = true;
            flags.subtract = false;
            flags.halfCarry = false;

            F = flags.ToByte();
        }

        // Bit rotate left A register with carry
        private void RLA()
        {
            Flags flags = Flags.FromByte(F);
            bool prevCarry = flags.carry;
            bool newCarry = (A & 0x80) != 0;

            A = (byte)(A << 1);
            if (prevCarry)
            {
                A |= 0x01;
            }

            flags.zero = false;
            flags.subtract = false;
            flags.halfCarry = false;
            flags.carry = newCarry;

            F = flags.ToByte();
        }

        // Bit rotate right A register with carry
        private void RRA()
        {
            Flags flags = Flags.FromByte(F);
            bool prevCarry = flags.carry;
            bool newCarry = (A & 0x01) != 0;

            A = (byte)(A >> 1);
            if (prevCarry)
            {
                A |= 0x80;
            }

            flags.zero = false;
            flags.subtract = false;
            flags.halfCarry = false;
            flags.carry = newCarry;

            F = flags.ToByte();
        }

        // Bit rotate left A register without carry
        private void RLCA()
        {
            Flags flags = Flags.FromByte(F);
            bool newCarry = (A & 0x80) != 0;

            A = (byte)(A << 1);
            if (newCarry)
            {
                A |= 0x01;
            }

            flags.zero = false;
            flags.subtract = false;
            flags.halfCarry = false;
            flags.carry = newCarry;

            F = flags.ToByte();
        }

        // Bit rotate right A register without carry
        private void RRCA()
        {
            Flags flags = Flags.FromByte(F);
            bool newCarry = (A & 0x01) != 0;

            A = (byte)(A >> 1);
            if (newCarry)
            {
                A |= 0x80;
            }

            flags.zero = false;
            flags.subtract = false;
            flags.halfCarry = false;
            flags.carry = newCarry;

            F = flags.ToByte();
        }

        // Toggle A register bits
        private void CPL()
        {
            Flags flags = Flags.FromByte(F);
            A = (byte)~A;

            flags.subtract = true;
            flags.halfCarry = true;

            F = flags.ToByte();
        }
        


    }
}
