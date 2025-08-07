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
                    byte value = GetRegisterValue(instruction.Target);
                    ADD(value);
                    break;
                case InstructionType.ADDHL:
                    ushort hlValue = GetRegisterValue(instruction.Target);
                    ADDHL(hlValue);
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
        
        
    }
}
