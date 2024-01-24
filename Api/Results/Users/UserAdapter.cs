using Core;
using Mapster;

namespace Api.Results.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class UserAdapter : IRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<User, UserSimpleResult>()
                  .Map(d => d.Id, d => d.Id)
                  .Map(d => d.Email, d => d.Email)
                  .Map(d => d.Name, d => d.Name);

        }
    }
}
