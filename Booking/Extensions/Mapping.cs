using AutoMapper;
using Booking.Entities;
using Booking.Models;

namespace Booking.Extensions
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Book, BookVM>().ReverseMap();
            CreateMap<Book, BookRequest>().ReverseMap();
        }
    }
}
