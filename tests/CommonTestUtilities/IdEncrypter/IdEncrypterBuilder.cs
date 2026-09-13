using Sqids;

namespace CommonTestUtilities.IdEncrypter
{
    public class IdEncrypterBuilder
    {
        public static SqidsEncoder<long> Build()
        {
            return new SqidsEncoder<long>(new()
            {
                MinLength = 3,
                Alphabet = "uq6JLH4g52ZEMN9kwsCDBePyQF8S3oWUpzdxivmnYbhI7rlT1OKVRac0XtfGjA"
            });
        }
    }
}
