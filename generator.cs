const string OUTPUT_PATH = "my_second_calculator.lua";

bool[] Int2Bits6(int x)
{
    byte[] bytes = BitConverter.GetBytes(x);
    bool[] bits = new bool[6];
    byte b = bytes[0];
    bits[0] = (b & 0x20) == 0x20;
    bits[1] = (b & 0x10) == 0x10;
    bits[2] = (b & 0x8) == 0x8;
    bits[3] = (b & 0x4) == 0x4;
    bits[4] = (b & 0x2) == 0x2;
    bits[5] = (b & 0x1) == 0x1;
    return bits;
};

Console.WriteLine("Starting");

File.WriteAllText(OUTPUT_PATH, @"local yes = {}
local no = {}

-- Get input from the user
local a = io.read(""*n"");
local operation = io.read(1);
local b = io.read(""*n"");

-- Convert int to binary
local int2bin = {");
string code = "";

// Integer to binary
{
    bool first = true;
    for (int i = 0; i <= 63; i++)
    {
        code += (first ? "" : ", ") + "[" + i + "] = {";

        bool[] bits = Int2Bits6(i);
        for (int j = 0; j < 5; j++)
        {
            code += bits[j] ? "yes, " : "no, ";
        }
        code += bits[5] ? "yes}" : "no}";

        first = false;
    }

    code += @"}

-- Convert binary to int
local bin2int = ";

    File.AppendAllText(OUTPUT_PATH, code);
    code = "";
}

// Binary to integer
{
    string branch(string binary)
    {
        if (binary.Length >= 12)
        {
            return Convert.ToInt32(binary, 2).ToString();
        }
        else
        {
            return "{[no] = " + branch(binary + '0') + ", [yes] = " + branch(binary + '1') + "}";
        }
    }

    File.AppendAllText(OUTPUT_PATH, branch(""));
}

// Binary full adder
{
    File.AppendAllText(OUTPUT_PATH, @"

local aBin = int2bin[a]
local bBin = int2bin[b]

--Binary full adder
local add = ");
    string branch(string binary)
    {
        if (binary.Length >= 3)
        {
            bool a = binary[0] == '1';
            bool b = binary[1] == '1';
            bool c = binary[2] == '1';
            bool out1 = (a ^ b) ^ c;
            //bool out2 = ((a ^ b) && c) || (a && b);
            bool out2 = (c && (a ^ b)) || (a && b);
            return "{" + (out1 ? "yes" : "no") + ", " + (out2 ? "yes" : "no") + "}";
        }
        else
        {
            return "{[no] = " + branch(binary + '0') + ", [yes] = " + branch(binary + '1') + '}';
        }
    }

    File.AppendAllText(OUTPUT_PATH, branch("") + "\n");
}

// Addition
{
    // Adder
    code = @"
if operation == ""+"" then
	-- Addition
	local pair1 = add[aBin[6]][bBin[6]][no]";

    for (int i = 2; i <= 6; i++)
    {
        code += $"\n\tlocal pair{i} = add[aBin[{7 - i}]][bBin[{7 - i}]][pair{i - 1}[2]]";
    }

    code += @"

	print(a .. "" + "" .. b .. "" = "" .. bin2int[no][no][no][no][no][pair6[2]]";

    for (int i = 6; i >= 1; i--)
    {
        code += $"[pair{i}[1]]";
    }

    code += ")";

    File.AppendAllText(OUTPUT_PATH, code);
    code = "";
}

// Subtraction
{
    File.AppendAllText(OUTPUT_PATH, @"
elseif operation == ""-"" then
	-- Subtraction
	-- Binary full subtractor
	local sub = ");
    string branch(string binary)
    {
        if (binary.Length >= 3)
        {
            bool a = binary[0] == '1';
            bool b = binary[1] == '1';
            bool c = binary[2] == '1';
            bool out1 = (a ^ b) ^ c;
            bool out2 = (!(a ^ b) && c) || (!a && b);
            return "{" + (out1 ? "yes" : "no") + ", " + (out2 ? "yes" : "no") + "}";
        }
        else
        {
            return "{[no] = " + branch(binary + '0') + ", [yes] = " + branch(binary + '1') + '}';
        }
    }

    File.AppendAllText(OUTPUT_PATH, branch(""));

    code = @"

	-- Subtract
	local pair1 = sub[aBin[6]][bBin[6]][no]";
    for (int i = 2; i <= 6; i++)
    {
        code += $"\n\tlocal pair{i} = sub[aBin[{7 - i}]][bBin[{7 - i}]][pair{i - 1}[2]]";
    }

    code += @"

	if pair6[2] == yes then
		-- Negative answer = 128 - result
		local pair1b = sub[no][pair1[1]][no]";
    for (int i = 2; i <= 6; i++)
        code += $"\n\t\tlocal pair{i}b = sub[no][pair{i}[1]][pair{i - 1}b[2]]";
    code += @"
		local pair7b = sub[no][pair6[2]][pair6b[2]]
		local pair8b = sub[yes][no][pair7b[2]]

		local answer = bin2int[no][no][no][pair8b[2]]";
    for (int i = 8; i >= 1; i--)
    {
        code += $"[pair{i}b[1]]";
    }

    code += @"

		print(a .. "" - "" .. b .. "" = -"" .. answer)
	else
		local answer = bin2int[no][no][no][no][no][pair6[2]]";

    for (int i = 6; i >= 1; i--)
    {
        code += $"[pair{i}[1]]";
    }

    code += @"

		print(a .. "" - "" .. b .. "" = "" .. answer)
	end";

    File.AppendAllText(OUTPUT_PATH, code);
    code = "";
}

// Multiplication
{
    File.AppendAllText(OUTPUT_PATH, @"
elseif operation == ""*"" then
	-- Multiplication
	-- Binary AND
	local andGate = {[no] = {[no] = no, [yes] = no}, [yes] = {[no] = no, [yes] = yes}}

	-- Multiply
	local digit12 = andGate[aBin[6]][bBin[6]]

	local temp1 = add[andGate[aBin[5]][bBin[6]]][andGate[aBin[6]][bBin[5]]][no]");
    
    for (int i = 5; i >= 1; i--)
    {
        code += $@"
	local temp{7 - i} = add[" + (i == 1 ? "no" : $"andGate[aBin[{i - 1}]][bBin[6]]") + $"][andGate[aBin[{i}]][bBin[5]]][temp{6 - i}[2]]";
    }

    code += "\n\tlocal digit11 = temp1[1]\n";

    for (int j = 4; j >= 1; j--)
    {
        for (int i = 6; i >= 1; i--)
        {
            code += $@"
	temp{7 - i} = add[" + (i == 1 ? "temp6[2]" : $"temp{8 - i}[1]") + $"][andGate[aBin[{i}]][bBin[{j}]]][" + (i == 6 ? "no" : $"temp{6 - i}[2]") + "]";
        }

        code += $@"
	local digit{6 + j} = temp1[1]
";
    }

    for (int i = 2; i <= 6; i++)
    {
        code += $@"
	local digit{8 - i} = temp{i}[1]";
    }

    code += @"
	local digit1 = temp6[2]

	print(bin2int";

    for (int i = 1; i <= 12; i++)
    {
        code += $"[digit{i}]";
    }

    File.AppendAllText(OUTPUT_PATH, code + ")");
}

File.AppendAllText(OUTPUT_PATH, @"
end");

Console.WriteLine("Done");
