using App.Metrics;
using App.Metrics.Counter;

namespace Booking.Metrics
{
    public class MetricRegistry
    {
        public static CounterOptions CreatedBooksCounter => new CounterOptions
        {
            Name = "Created Books",
            Context = "Booking.API",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions UpdatedBooksCounter => new CounterOptions
        {
            Name = "Updated Books",
            Context = "Booking.API",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions DeletedBooksCounter => new CounterOptions
        {
            Name = "Deleted Books",
            Context = "Booking.API",
            MeasurementUnit = Unit.Calls
        };

        public static CounterOptions CreatedDbContextCounter => new CounterOptions
        {
            Name = "Created Database Context",
            Context = "Booking.API",
            MeasurementUnit = Unit.Calls
        };
    }
}
