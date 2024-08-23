using AutoMapper;
using Domain.dtos;
using Domain.models;

namespace Service.mappings
{
    public class TarefaMppingProfile : Profile
    {
        public TarefaMppingProfile()
        {
            CreateMap<Tarefa, TarefaRetornoDto>().ReverseMap();
            CreateMap<Tarefa, TarefaCreateDto>().ReverseMap();
        }
    }
}
