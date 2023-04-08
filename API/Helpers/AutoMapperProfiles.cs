using API.DTO;
using API.Entities;
using API.Extensions;
using AutoMapper;

//AutoMaps DTO class to AppUsers.
// Prevents tedious code writing for mapping entity to class.
//Not case sensitive in mapping class properties but sensitive to data types.
namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {   //Create mappin g from Appuser entity to MemberDto to display relevant user's info
            //It compares same property and maps accordingly. (Id in MemberDto maps to Id in AppUser)
            //Automap automatically detect GetAge method and calculate Age property.
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, //Configure individual property 'PhotoUrl'
                    opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x=> x.IsMain).Url))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
                //Spcify options to map from source (AppUser) Photos property where first/default matches IsMain = 'true' property
                //and returns its URL

            //Create mapping  from Photo entity to PhotoDto display relevant user's info
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>(); // map from memberupdateDto to app user 
            CreateMap<RegisterDto, AppUser>(); // map from registerDto to AppUser
            CreateMap<Message, MessageDto>() // map from message to messageDto, automapper cant by map url by default based on properties, thus need to specify
                .ForMember(d => d.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(x => x.IsMain).Url));
                
        }   
    }
}