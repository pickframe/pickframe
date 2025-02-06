namespace Tests.Builders;

public class VideoTestBuilder
{
    public static MemoryStream Instancia()
    {
        MemoryStream memStream = new MemoryStream();

        using (FileStream fileStream = File.OpenRead("test.mp4"))
        {
            memStream.SetLength(fileStream.Length);
            fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
        }

        return memStream;
    }
}
