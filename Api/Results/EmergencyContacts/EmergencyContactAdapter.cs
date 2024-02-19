using Core;
using Mapster;

namespace Api.Results.EmergencyContacts
{
    /// <summary>
    /// 
    /// </summary>
    public class EmergencyContactAdapter : IRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<EmergencyContact, EmergencyContactResult>();
        }
    }
}
