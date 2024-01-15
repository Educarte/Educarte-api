namespace Api.Infrastructure.ValueObjects
{
    public class SensitiveContent<T>
    {
        public T Value { get; set; }

        public static implicit operator SensitiveContent<T>(T value)
        {
            return new SensitiveContent<T>
            {
                Value = value,
            };
        }
    }
}
