namespace AGameBoyEmu.Tests;

using Xunit;
using AGameBoyEmu.SoC;

public class CpuInstructionTests
{
    [Fact]
    public void ADD_AddsValueAndSetsFlags()
    {
        var cpu = new CPU { A = 1 };
        cpu.Execute(new Instruction(InstructionType.ADD, Register8.B));
        // B is 0 by default, so result is 1
        Assert.Equal(1, cpu.A);
        Assert.False((cpu.F & 0x80) != 0); // Zero flag
    }

    [Fact]
    public void ADDHL_AddsToHLAndPreservesZ()
    {
        var cpu = new CPU { H = 0x12, L = 0x34, B = 0x00, C = 0x01, F = 0x80 }; // Z flag set
        cpu.Execute(new Instruction(InstructionType.ADDHL, Register8.BC));
        Assert.Equal(0x1235, cpu.HL);
        Assert.True((cpu.F & 0x80) != 0); // Z flag preserved
    }

    [Fact]
    public void ADC_AddsWithCarry()
    {
        var cpu = new CPU { A = 1, B = 2, F = 0x10 }; // Carry set
        cpu.Execute(new Instruction(InstructionType.ADC, Register8.B));
        Assert.Equal(4, cpu.A); // 1 + 2 + 1
    }

    [Fact]
    public void SUB_SubtractsValueAndSetsFlags()
    {
        var cpu = new CPU { A = 2, B = 1 };
        cpu.Execute(new Instruction(InstructionType.SUB, Register8.B));
        Assert.Equal(1, cpu.A);
        Assert.False((cpu.F & 0x80) != 0); // Zero flag
    }

    [Fact]
    public void SBC_SubtractsWithCarry()
    {
        var cpu = new CPU { A = 3, B = 1, F = 0x10 }; // Carry set
        cpu.Execute(new Instruction(InstructionType.SBC, Register8.B));
        Assert.Equal(1, cpu.A); // 3 - 1 - 1
    }

    [Fact]
    public void AND_PerformsBitwiseAnd()
    {
        var cpu = new CPU { A = 0b10101010, B = 0b11001100 };
        cpu.Execute(new Instruction(InstructionType.AND, Register8.B));
        Assert.Equal(0b10001000, cpu.A);
        Assert.True((cpu.F & 0x20) != 0); // Half-carry always set
    }

    [Fact]
    public void OR_PerformsBitwiseOr()
    {
        var cpu = new CPU { A = 0b10101010, B = 0b11001100 };
        cpu.Execute(new Instruction(InstructionType.OR, Register8.B));
        Assert.Equal(0b11101110, cpu.A);
        Assert.False((cpu.F & 0x20) != 0); // Half-carry cleared
    }

    [Fact]
    public void XOR_PerformsBitwiseXor()
    {
        var cpu = new CPU { A = 0b10101010, B = 0b11001100 };
        cpu.Execute(new Instruction(InstructionType.XOR, Register8.B));
        Assert.Equal(0b01100110, cpu.A);
    }

    [Fact]
    public void CP_ComparesAndSetsFlags()
    {
        var cpu = new CPU { A = 2, B = 2 };
        cpu.Execute(new Instruction(InstructionType.CP, Register8.B));
        Assert.Equal(2, cpu.A); // A is unchanged
        Assert.True((cpu.F & 0x80) != 0); // Zero flag set
    }

    [Fact]
    public void INC_IncrementsRegister()
    {
        var cpu = new CPU { B = 1 };
        cpu.Execute(new Instruction(InstructionType.INC, Register8.B));
        Assert.Equal(2, cpu.B);
    }

    [Fact]
    public void DEC_DecrementsRegister()
    {
        var cpu = new CPU { B = 2 };
        cpu.Execute(new Instruction(InstructionType.DEC, Register8.B));
        Assert.Equal(1, cpu.B);
    }

    [Fact]
    public void CCF_ComplementsCarryFlag()
    {
        var cpu = new CPU { F = 0x10 }; // Carry set
        cpu.Execute(new Instruction(InstructionType.CCF, Register8.A));
        Assert.False((cpu.F & 0x10) != 0); // Carry cleared
    }

    [Fact]
    public void SCF_SetsCarryFlag()
    {
        var cpu = new CPU { F = 0x00 };
        cpu.Execute(new Instruction(InstructionType.SCF, Register8.A));
        Assert.True((cpu.F & 0x10) != 0); // Carry set
    }

    [Fact]
    public void RLA_RotatesLeftThroughCarry()
    {
        var cpu = new CPU { A = 0b10000000, F = 0x10 }; // Carry set
        cpu.Execute(new Instruction(InstructionType.RLA, Register8.A));
        Assert.Equal(1, cpu.A); // 0b00000001
        Assert.True((cpu.F & 0x10) != 0); // Carry set
    }

    [Fact]
    public void RRA_RotatesRightThroughCarry()
    {
        var cpu = new CPU { A = 0b00000001, F = 0x10 }; // Carry set
        cpu.Execute(new Instruction(InstructionType.RRA, Register8.A));
        Assert.Equal(0b10000000, cpu.A);
        Assert.True((cpu.F & 0x10) != 0); // Carry set
    }

    [Fact]
    public void RLCA_RotatesLeftCircular()
    {
        var cpu = new CPU { A = 0b10000001 };
        cpu.Execute(new Instruction(InstructionType.RLCA, Register8.A));
        Assert.Equal(0b00000011, cpu.A);
        Assert.True((cpu.F & 0x10) != 0); // Carry set
    }

    [Fact]
    public void RRCA_RotatesRightCircular()
    {
        var cpu = new CPU { A = 0b00000001 };
        cpu.Execute(new Instruction(InstructionType.RRCA, Register8.A));
        Assert.Equal(0b10000000, cpu.A);
        Assert.True((cpu.F & 0x10) != 0); // Carry set
    }

    [Fact]
    public void CPL_ComplementsA()
    {
        var cpu = new CPU { A = 0b10101010 };
        cpu.Execute(new Instruction(InstructionType.CPL, Register8.A));
        Assert.Equal(0b01010101, cpu.A);
        Assert.True((cpu.F & 0x60) != 0); // N and H set
    }

    [Fact]
    public void BIT_TestsBit()
    {
        var cpu = new CPU { B = 0b00010000 };
        cpu.Execute(new Instruction(InstructionType.BIT, Register8.B, 4));
        Assert.False((cpu.F & 0x80) != 0); // Zero flag cleared (bit 4 is set)
        cpu.Execute(new Instruction(InstructionType.BIT, Register8.B, 5));
        Assert.True((cpu.F & 0x80) != 0); // Zero flag set (bit 5 is clear)
    }

    [Fact]
    public void SET_SetsBit()
    {
        var cpu = new CPU { B = 0b00000000 };
        cpu.Execute(new Instruction(InstructionType.SET, Register8.B, 3));
        Assert.Equal(0b00001000, cpu.B);
    }

    [Fact]
    public void RES_ResetsBit()
    {
        var cpu = new CPU { B = 0b00001111 };
        cpu.Execute(new Instruction(InstructionType.RES, Register8.B, 2));
        Assert.Equal(0b00001011, cpu.B);
    }

    [Fact]
    public void SRL_ShiftsRightLogical()
    {
        var cpu = new CPU { B = 0b00000011 };
        cpu.Execute(new Instruction(InstructionType.SRL, Register8.B));
        Assert.Equal(0b00000001, cpu.B);
        Assert.True((cpu.F & 0x10) != 0); // Carry set (bit 0 was 1)
    }

    [Fact]
    public void RL_RotatesLeftThroughCarry()
    {
        var cpu = new CPU { B = 0b10000000, F = 0x10 }; // Carry set
        cpu.Execute(new Instruction(InstructionType.RL, Register8.B));
        Assert.Equal(1, cpu.B);
        Assert.True((cpu.F & 0x10) != 0); // Carry set
    }

    [Fact]
    public void RR_RotatesRightThroughCarry()
    {
        var cpu = new CPU { B = 0b00000001, F = 0x10 }; // Carry set
        cpu.Execute(new Instruction(InstructionType.RR, Register8.B));
        Assert.Equal(0b10000000, cpu.B);
        Assert.True((cpu.F & 0x10) != 0); // Carry set
    }

    [Fact]
    public void RLC_RotatesLeftCircular()
    {
        var cpu = new CPU { B = 0b10000001 };
        cpu.Execute(new Instruction(InstructionType.RLC, Register8.B));
        Assert.Equal(0b00000011, cpu.B);
        Assert.True((cpu.F & 0x10) != 0); // Carry set
    }

    [Fact]
    public void RRC_RotatesRightCircular()
    {
        var cpu = new CPU { B = 0b00000001 };
        cpu.Execute(new Instruction(InstructionType.RRC, Register8.B));
        Assert.Equal(0b10000000, cpu.B);
        Assert.True((cpu.F & 0x10) != 0); // Carry set
    }

    [Fact]
    public void SLA_ShiftsLeftArithmetic()
    {
        var cpu = new CPU { B = 0b01000000 };
        cpu.Execute(new Instruction(InstructionType.SLA, Register8.B));
        Assert.Equal(0b10000000, cpu.B);
        Assert.False((cpu.F & 0x10) != 0); // Carry not set
    }

    [Fact]
    public void SRA_ShiftsRightArithmetic()
    {
        var cpu = new CPU { B = 0b10000001 };
        cpu.Execute(new Instruction(InstructionType.SRA, Register8.B));
        Assert.Equal(0b11000000, cpu.B); // Bit 7 preserved
        Assert.True((cpu.F & 0x10) != 0); // Carry set (bit 0 was 1)
    }

    [Fact]
    public void SWAP_SwapsNibbles()
    {
        var cpu = new CPU { B = 0b10110001 };
        cpu.Execute(new Instruction(InstructionType.SWAP, Register8.B));
        Assert.Equal(0b00011011, cpu.B);
        Assert.False((cpu.F & 0x80) != 0); // Zero flag not set
    }
}