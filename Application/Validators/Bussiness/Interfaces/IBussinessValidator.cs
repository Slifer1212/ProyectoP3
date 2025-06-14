
namespace Application.Validators.Bussiness.Interfaces;

public interface IBussinessValidator<TCreateEntity, TUpdateEntity , TType>
{
    Task<(bool IsValid, List<string> Errors)> ValidateCreateAsync(TCreateEntity dto);
    Task<(bool IsValid, List<string> Errors)> ValidateUpdateAsync(TUpdateEntity dto);
    Task<(bool IsValid, List<string> Errors)> ValidateDeleteAsync(TType id );
}