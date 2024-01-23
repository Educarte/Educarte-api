using Core;
using Mapster;

namespace Api.Results.Diary
{
    /// <summary>
    /// 
    /// </summary>
    public class DiaryAdapter : IRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Core.Diary, DiaryResult>();
            config.NewConfig<Core.Diary, DiarySimpleResult>();
        }
    }
}
