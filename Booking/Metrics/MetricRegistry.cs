using App.Metrics;
using App.Metrics.Counter;

namespace Booking.Metrics
{
    public class MetricRegistry
    {
        public static CounterOptions CreatedBooksCounter => new()
        {
            Name = "Created Books",
            Context = "Booking.API",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions UpdatedBooksCounter => new()
        {
            Name = "Updated Books",
            Context = "Booking.API",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions DeletedBooksCounter => new()
        {
            Name = "Deleted Books",
            Context = "Booking.API",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions CreatedDbContextCounter => new()
        {
            Name = "Created Database Context",
            Context = "Booking.API",
            MeasurementUnit = Unit.Calls
        };
    }
}
