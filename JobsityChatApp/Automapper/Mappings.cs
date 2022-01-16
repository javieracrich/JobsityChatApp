using AutoMapper;
using JobsityChatApp.Core;
using JobsityChatApp.Data;

namespace JobsityChatApp.Automapper;

public class Mappings : Profile
{
    public Mappings()
    {
        CreateMap<Message, MessageDto>();
    }
}