using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IUserRepository repository, IMapper mapper, IPhotoService photoService) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await repository.GetMembersAsync();
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            var user = await repository.GetMemberAsync(username);
            if (user == null) return NotFound(user);

            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto dto)
        {
            var user = await repository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return BadRequest("Could not find user");

            mapper.Map(dto, user);

            if (await repository.SaveAllChanges()) return NoContent();

            return BadRequest("Failed to update the user");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await repository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return BadRequest("Cannot update user");

            var result = await photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);
            var photo = new Photo { Url = result.SecureUrl.AbsoluteUri, PublicId = result.PublicId };
            user.Photos.Add(photo);
            if (await repository.SaveAllChanges()) return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDto>(photo));

            return BadRequest("Error occured while adding photo");
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var user = await repository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return BadRequest("Could not find user");

            var photo = user.Photos.FirstOrDefault(photo => photo.Id == photoId);
            if (photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");

            var currentMain = user.Photos.FirstOrDefault(photo => photo.IsMain);
            if (currentMain != null) currentMain.IsMain = false;
            photo.IsMain = true;

            if (await repository.SaveAllChanges()) return NoContent();

            return BadRequest("Error occurred while setting main photo");
        }


        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await repository.GetUserByUsernameAsync(User.GetUsername());
            if(user == null) return BadRequest("User not found");
            var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);
            if(photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");
            if(photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error != null) return BadRequest(result.Error.Message);
            }
            user.Photos.Remove(photo);
            if (await repository.SaveAllChanges()) return Ok();

            return BadRequest("Error occurred while deleting photo");
        }
    }
}
