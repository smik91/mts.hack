using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Midiot.BL.Interfaces.User;
using Midiot.BL.Models.UserProfile;
using Midiot.Common.Helpers;
using Midiot.Data;

namespace Midiot.BL.Services.User;

public class UserProfileService : IUserProfileService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserExistenceService _userExistenceService;

    public UserProfileService(AppDbContext context, IMapper mapper, IUserExistenceService userExistenceService)
    {
        _context = context;
        _mapper = mapper;
        _userExistenceService = userExistenceService;
    }

    public async Task<GetProfileModel> GetProfileAsync(Guid userId)
    {
        var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);

        var userProfile = _mapper.Map<GetProfileModel>(user);

        return userProfile;
    }

    public async Task UpdateProfileAsync(UpdateProfileModel model, Guid userId)
    {
        var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        ValidationHelper.EnsureEntityFound(user);
        await _userExistenceService.EnsureUserDoesNotExist(model.Name);

        _mapper.Map(model, user);
        await _context.SaveChangesAsync();
    }
}
