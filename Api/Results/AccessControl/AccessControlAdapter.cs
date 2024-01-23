using Core;
using Mapster;

namespace Api.Results.AccessControl
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessControlAdapter : IRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Core.AccessControl, AccessControlResult>();
        }
    }
}
