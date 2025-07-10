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


    }
}
