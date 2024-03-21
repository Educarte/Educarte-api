using Core;
using Mapster;

namespace Api.Results.Adresses
{
    /// <summary>
    /// 
    /// </summary>
    public class Adressdapter : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Address, AddressResult>();
        }
    }
}
