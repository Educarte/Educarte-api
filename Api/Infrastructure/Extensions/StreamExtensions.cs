namespace Api.Infrastructure.Extensions
{
    public static class StreamExtensions
    {
        public static byte[] ToArray(this Stream stream)
        {
            if (stream is MemoryStream memoryStream)
                return memoryStream.ToArray();

            using (memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
