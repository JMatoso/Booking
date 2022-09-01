namespace Booking.Extensions
{
    public class EnumParser<TType>
    {
        public static TType ParseToType(string value)
        {
            return (TType)Enum.Parse(typeof(TType), value, true);
        }
    }
}
