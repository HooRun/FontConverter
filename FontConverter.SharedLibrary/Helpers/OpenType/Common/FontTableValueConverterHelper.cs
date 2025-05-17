namespace FontConverter.SharedLibrary.Helpers;

public static class FontTableValueConverterHelper
{
    public static string GetTableReadableTag(uint tag)
    {
        return new string(new[]
        {
            (char)((tag >> 24) & 0xFF),
            (char)((tag >> 16) & 0xFF),
            (char)((tag >> 08) & 0xFF),
            (char)((tag >> 00) & 0xFF)
        });
    }

    public static ushort ReadUInt16BigEndian(BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(2);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return BitConverter.ToUInt16(bytes, 0);
    }

    public static short ReadInt16BigEndian(BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(2);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        return BitConverter.ToInt16(bytes, 0);
    }

    public static uint ReadUInt32BigEndian(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(4);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }

    public static int ReadInt32BigEndian(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(4);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToInt32(bytes, 0);
    }

    public static ulong ReadUInt64BigEndian(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(8);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToUInt64(bytes, 0);
    }

    public static long ReadInt64BigEndian(BinaryReader reader)
    {
        var bytes = reader.ReadBytes(8);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(bytes);
        return BitConverter.ToInt64(bytes, 0);
    }

    public static double FixedToDouble(uint fixedVal)
    {
        ushort majorVersion = (ushort)(fixedVal >> 16);
        ushort minorVersion = (ushort)(fixedVal & 0xFFFF);
        return majorVersion + ((double)minorVersion / Math.Pow(10, minorVersion.ToString().Length));
    }

    public static DateTime ReadLongDateTime(long macTime)
    {
        // 1904/01/01
        var baseDate = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return baseDate.AddSeconds(macTime);
    }

    public static float ReadF2Dot14(BinaryReader reader)
    {
        short val = ReadInt16BigEndian(reader);
        return val / 16384f;
    }
}
