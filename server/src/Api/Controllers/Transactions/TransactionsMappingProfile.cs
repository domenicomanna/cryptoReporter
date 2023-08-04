using Api.Controllers.Transactions.Common;
using Api.Domain.Models;
using AutoMapper;

namespace Api.Controllers.Transactions;

public class TransactionsMappingProfile : Profile
{
    public TransactionsMappingProfile()
    {
        CreateMap<Transaction, TransactionDTO>()
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.Name));
    }
}
