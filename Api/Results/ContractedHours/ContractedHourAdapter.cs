using Core;
using Mapster;

namespace Api.Results.ContractedHours
{
    /// <summary>
    /// 
    /// </summary>
    public class ContractedHourAdapter : IRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ContractedHour, ContractedHourResult>();
        }
    }
}
