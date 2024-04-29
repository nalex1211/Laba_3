using AutoMapper;
using Laba_3.Dto;
using Laba_3.Models;

namespace Laba_3.Helper;

public class MappingProfiles : Profile
{
	public MappingProfiles()
	{
		CreateMap<Comment, CommentDto>();
	}
}
