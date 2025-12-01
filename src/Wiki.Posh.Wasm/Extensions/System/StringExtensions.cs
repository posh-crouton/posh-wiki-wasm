namespace Wiki.Posh.Wasm.Extensions.System;

public static class StringExtensions
{
    public static Stream ToStream(this string source)
    {
        Stream ret = new MemoryStream();
        using var  writer = new StreamWriter(ret);
        writer.Write(source);
        writer.Flush();
        ret.Position = 0;
        return ret;
    }
}