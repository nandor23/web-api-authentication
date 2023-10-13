using System.ComponentModel.DataAnnotations;
using Dtos.Requests;
using Models;
using Riok.Mapperly.Abstractions;

namespace Mappers;

[Mapper]
public partial class UserMapper
{
    public User SignUpDtoToUser(SignUpRequestDto signUpRequestDto)
    {
        var user = MapSignUpDtoToUser(signUpRequestDto);
        user.Email = user.Email!.ToLowerInvariant();
        return user;
    }

    public User LoginDtoToUser(LoginRequestDto loginRequestDto)
    {
        var user = MapLoginDtoToUser(loginRequestDto);
        if (new EmailAddressAttribute().IsValid(loginRequestDto.UserNameOrEmail))
        {
            user.Email = loginRequestDto.UserNameOrEmail.ToLowerInvariant();
        }
        else
        {
            user.UserName = loginRequestDto.UserNameOrEmail;
        }
        return user;
    }

    private partial User MapSignUpDtoToUser(SignUpRequestDto signUpRequestDto);
    private partial User MapLoginDtoToUser(LoginRequestDto loginRequestDto);

}