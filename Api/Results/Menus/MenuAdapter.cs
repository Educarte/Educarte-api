using Core;
using Mapster;

namespace Api.Results.Menus
{
    /// <summary>
    /// 
    /// </summary>
    public class MenuAdapter : IRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Menu, MenuResult>();
        }
    }
}
