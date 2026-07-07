using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;

namespace FainEngine_v2.Utils;
public static class PNGWriter
{
    public static void WritePng(Stream stream, byte[] rgba, int width, int height, bool flipY = true)
    {
        if (rgba == null)
            throw new ArgumentNullException(nameof(rgba));

        if (rgba.Length != width * height * 4)
            throw new ArgumentException("RGBA buffer must be width * height * 4 bytes.");

        // -------------------------
        // Build IDAT (zlib stream)
        // -------------------------
        byte[] idat;

        using (var ms = new MemoryStream())
        {
            using (var zlib = new ZLibStream(ms, CompressionLevel.Optimal, leaveOpen: true))
            {
                int stride = width * 4;

                for (int y = 0; y < height; y++)
                {
                    int srcY = flipY ? (height - 1 - y) : y;
                    int rowIndex = srcY * stride;

                    // PNG filter type 0 (no filtering)
                    zlib.WriteByte(0);
                    zlib.Write(rgba, rowIndex, stride);
                }
            }

            idat = ms.ToArray();
        }

        using var bw = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true);

        // -------------------------
        // PNG signature
        // -------------------------
        bw.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

        // -------------------------
        // IHDR chunk
        // -------------------------
        WriteChunk(bw, "IHDR", BuildIHDR(width, height));

        // -------------------------
        // IDAT chunk
        // -------------------------
        WriteChunk(bw, "IDAT", idat);

        // -------------------------
        // IEND chunk
        // -------------------------
        WriteChunk(bw, "IEND", Array.Empty<byte>());
    }

    // =========================
    // IHDR (critical header)
    // =========================
    private static byte[] BuildIHDR(int width, int height)
    {
        byte[] data = new byte[13];

        WriteIntBE(data, 0, width);
        WriteIntBE(data, 4, height);

        data[8] = 8; // bit depth
        data[9] = 6; // RGBA
        data[10] = 0; // compression
        data[11] = 0; // filter
        data[12] = 0; // interlace

        return data;
    }

    // =========================
    // PNG chunk writer
    // =========================
    private static void WriteChunk(BinaryWriter bw, string type, byte[] data)
    {
        byte[] typeBytes = Encoding.ASCII.GetBytes(type);

        bw.Write(BinaryPrimitives.ReverseEndianness(data.Length));
        bw.Write(typeBytes);
        bw.Write(data);

        uint crc = Crc32(typeBytes, data);
        bw.Write(BinaryPrimitives.ReverseEndianness((int)crc));
    }

    // =========================
    // CRC32 (PNG required)
    // =========================
    private static uint Crc32(byte[] type, byte[] data)
    {
        uint crc = 0xffffffff;

        foreach (byte b in type)
            crc = Update(crc, b);

        foreach (byte b in data)
            crc = Update(crc, b);

        return ~crc;
    }

    private static uint Update(uint crc, byte b)
    {
        crc ^= b;

        for (int i = 0; i < 8; i++)
        {
            if ((crc & 1) != 0)
                crc = (crc >> 1) ^ 0xedb88320u;
            else
                crc >>= 1;
        }

        return crc;
    }

    // =========================
    // Big-endian int writer
    // =========================
    private static void WriteIntBE(byte[] buffer, int offset, int value)
    {
        buffer[offset + 0] = (byte)(value >> 24);
        buffer[offset + 1] = (byte)(value >> 16);
        buffer[offset + 2] = (byte)(value >> 8);
        buffer[offset + 3] = (byte)value;
    }
}